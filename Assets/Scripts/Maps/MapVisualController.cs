using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoguelikeMap
{
    [Serializable]
    public struct NodeTypeVisual
    {
        public NodeType Type;
        public Sprite Sprite;
        public Color Tint;
    }

    [Serializable]
    public struct NodeTypeLayerPool
    {
        public NodeType Type;
        [Tooltip("Camada mínima (inclusive) em que este pool se aplica. Camada 0 é o nó inicial.")]
        public int MinLayer;
        [Tooltip("Camada máxima (inclusive) em que este pool se aplica.")]
        public int MaxLayer;
        public Level.LevelPool Pool;
    }
    [ExecuteAlways]
    public class MapVisualController : MonoBehaviour
    {
        [Header("Geração")]
        public int levelIndex = 0;
        public int globalSeed = 12345;
        [Tooltip("Total de nós no grafo, incluindo o nó inicial e o boss")]
        public int totalNodes = 20;
        public int minNodesPerLayer = 2;
        public int maxNodesPerLayer = 4;

        [Header("Background")]
        public SpriteRenderer backgroundPrefab;
        public float backgroundPadding = 2f;

        [Header("Nós")]
        public GameObject nodePrefab; 
        public List<NodeTypeVisual> nodeVisuals = new List<NodeTypeVisual>();
        public float nodeScale = 1.5f;
        [Tooltip("Sprite usado em qualquer nó bloqueado (cadeado). Arraste aqui uma única vez.")]
        public Sprite lockedNodeSprite;

        [Header("Fases por tipo de nó e camada")]
        [Tooltip("Cada entrada cobre um Tipo + faixa de camadas. Ao clicar num nó, o GameManager sorteia uma fase dentro do LevelPool correspondente e carrega a cena. Entradas não podem se sobrepor para o mesmo tipo.")]
        public List<NodeTypeLayerPool> nodeLayerPools = new List<NodeTypeLayerPool>();

        [Header("Linhas")]
        [Tooltip("Material das linhas PERCORRIDAS (sólidas). Se vazio, usa um material branco gerado automaticamente.")]
        public Material lineMaterial;
        [Tooltip("Material das linhas NÃO percorridas (pontilhadas). Se vazio, gera um dash automático em runtime.")]
        public Material dashedLineMaterial;
        public Color lineColor = Color.white;
        [Tooltip("Cor das linhas depois de percorridas")]
        public Color traversedLineColor = Color.green;
        public float lineWidth = 0.08f;
        [Tooltip("Quantos 'dashes' por unidade de mundo na linha pontilhada")]
        public float dashTilingPerUnit = 1f;

        private Material runtimeDashedMaterial;
        private Material runtimeSolidMaterial;
        private Dictionary<string, LineRenderer> spawnedLines;
        private readonly Dictionary<string, Material> lineMaterialInstances = new Dictionary<string, Material>();
        private readonly Dictionary<string, bool> lineTraversedState = new Dictionary<string, bool>();

        [Header("Câmera")]
        public Camera targetCamera;   
        public bool fitCameraToMap = true;
        public float cameraPadding = 0.3f;
        [Range(0.4f, 1f)]
        [Tooltip("Quanto da tela o grafo deve preencher. 0.8 = grafo ocupa 80%, sobrando 20% de respiro nas bordas.")]
        public float viewportFillPercent = 0.8f;

        // --- Estado de progressão (em memória, reseta ao recarregar a cena) ---
        private MapData currentMap;
        private Dictionary<int, GameObject> spawnedNodes;
        private readonly List<int> visitedPath = new List<int>();

        private void OnEnable() => Rebuild();

        [ContextMenu("Rebuild Map")]
        public void Rebuild()
        {
            Clear();
            if (nodePrefab == null) return;

            var map = MapGenerator.Generate(levelIndex, globalSeed, totalNodes, minNodesPerLayer, maxNodesPerLayer);
            var bounds = ComputeBounds(map);

            SpawnBackground(map, bounds);
            var spawned = SpawnNodes(map);
            spawnedLines = SpawnConnections(map);

            currentMap = map;
            spawnedNodes = spawned;

            visitedPath.Clear();
            if (Application.isPlaying && GameManager.Instance != null && GameManager.Instance.MapProgress.Count > 0)
                visitedPath.AddRange(GameManager.Instance.MapProgress);

            RefreshMapVisuals();

            if (fitCameraToMap) FitCamera(bounds);
        }
        private void FitCamera(Bounds bounds)
        {
            var cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null || !cam.orthographic) return;

            float nodeRadius = GetNodeVisualRadius();
            float extraPadding = cameraPadding + nodeRadius;

            float paddedWidth = bounds.size.x + extraPadding * 2f;
            float paddedHeight = bounds.size.y + extraPadding * 2f;

            float sizeByHeight = paddedHeight / 2f;
            float sizeByWidth = paddedWidth / (2f * cam.aspect);
            float fitSize = Mathf.Max(sizeByHeight, sizeByWidth);

            cam.orthographicSize = fitSize / Mathf.Clamp(viewportFillPercent, 0.1f, 1f);

            var camPos = cam.transform.position;
            cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, camPos.z);
        }
        private float GetNodeVisualRadius()
        {
            if (nodePrefab == null) return 0f;
            var sr = nodePrefab.GetComponentInChildren<SpriteRenderer>();
            if (sr == null || sr.sprite == null) return 0f;

            var extents = sr.sprite.bounds.extents; // metade do tamanho, em escala 1
            return Mathf.Max(extents.x, extents.y) * nodeScale;
        }

        private void Clear()
        {
            DestroyAllLineMaterialInstances();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i).gameObject;
                #if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(child);
                else Destroy(child);
                #else
                Destroy(child);
                #endif
            }
        }

        private void DestroyAllLineMaterialInstances()
        {
            foreach (var mat in lineMaterialInstances.Values)
            {
                if (mat == null) continue;
                #if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(mat);
                else Destroy(mat);
                #else
                Destroy(mat);
                #endif
            }
            lineMaterialInstances.Clear();
            lineTraversedState.Clear();
        }

        private void SpawnBackground(MapData map, Bounds bounds)
        {
            if (backgroundPrefab == null) return;

            var bg = Instantiate(backgroundPrefab, transform);
            bg.name = "Background";
            bg.transform.position = new Vector3(bounds.center.x, bounds.center.y, 0.1f); // atrás dos nós

            var spriteSize = bg.sprite.bounds.size;
            var targetSize = new Vector2(bounds.size.x, bounds.size.y) + Vector2.one * backgroundPadding * 2f;
            bg.transform.localScale = new Vector3(
                targetSize.x / Mathf.Max(spriteSize.x, 0.001f),
                targetSize.y / Mathf.Max(spriteSize.y, 0.001f),
                1f);
            bg.sortingOrder = -10;
        }

       private Dictionary<int, GameObject> SpawnNodes(MapData map)
    {
    var spawned = new Dictionary<int, GameObject>();
    foreach (var node in map.Nodes)
        {
        var go = Instantiate(nodePrefab, node.Position, Quaternion.identity, transform);
        go.name = $"Node_{node.Id}_{node.Type}";
        go.transform.localScale = Vector3.one * nodeScale;

        var visual = nodeVisuals.FirstOrDefault(v => v.Type == node.Type);
        var sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            if (visual.Sprite != null) sr.sprite = visual.Sprite;
            sr.color = visual.Tint == default ? Color.white : visual.Tint;
            sr.sortingOrder = 1;
        }

        var clickHandler = go.GetComponentInChildren<MapNodeClickHandler>();
        if (clickHandler != null)
        {
            clickHandler.NodeId = node.Id;
            clickHandler.Type = node.Type;
            clickHandler.Controller = this;
            clickHandler.Initialize(sr);
        }

        spawned[node.Id] = go;
        }
    return spawned;
    }

        private Dictionary<string, LineRenderer> SpawnConnections(MapData map)
        {
            var lines = new Dictionary<string, LineRenderer>();

            foreach (var node in map.Nodes)
            {
                foreach (var targetId in node.ConnectionsToNextLayer)
                {
                    var lineGO = new GameObject($"Line_{node.Id}_{targetId}");
                    lineGO.transform.SetParent(transform);

                    var lr = lineGO.AddComponent<LineRenderer>();
                    lr.startWidth = lr.endWidth = lineWidth;
                    lr.positionCount = 2;
                    lr.sortingOrder = 0;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, node.Position);
                    lr.SetPosition(1, map.Nodes.First(n => n.Id == targetId).Position);

                    lines[LineKey(node.Id, targetId)] = lr;
                }
            }

            return lines;
        }

        private static string LineKey(int fromId, int toId) => $"{fromId}_{toId}";

        private Bounds ComputeBounds(MapData map)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var n in map.Nodes)
            {
                min = Vector2.Min(min, n.Position);
                max = Vector2.Max(max, n.Position);
            }
            var b = new Bounds();
            b.SetMinMax(new Vector3(min.x, min.y, 0), new Vector3(max.x, max.y, 0));
            return b;
        }

        // --- Progressão ---

        public NodeProgressState GetNodeState(int nodeId)
        {
            if (currentMap == null) return NodeProgressState.Locked;
            if (visitedPath.Contains(nodeId)) return NodeProgressState.Visited;

            if (visitedPath.Count == 0)
            {
                var startNode = currentMap.Nodes.First(n => n.Layer == 0);
                return nodeId == startNode.Id ? NodeProgressState.Available : NodeProgressState.Locked;
            }

            int currentId = visitedPath[visitedPath.Count - 1];
            var currentNode = currentMap.Nodes.FirstOrDefault(n => n.Id == currentId);
            if (currentNode != null && currentNode.ConnectionsToNextLayer.Contains(nodeId))
                return NodeProgressState.Available;

            return NodeProgressState.Locked;
        }

        public bool TrySelectNode(int nodeId)
        {
            var state = GetNodeState(nodeId);
            if (state != NodeProgressState.Available)
            {
                Debug.Log($"Nó {nodeId} não pode ser selecionado agora (estado: {state}).");
                return false;
            }

            visitedPath.Add(nodeId);
            if (Application.isPlaying && GameManager.Instance != null)
                GameManager.Instance.SetMapProgress(visitedPath);

            RefreshMapVisuals();
            OnNodeSelected(nodeId);
            return true;
        }

        public int GetCurrentNodeId() => visitedPath.Count > 0 ? visitedPath[visitedPath.Count - 1] : -1;

        private void OnNodeSelected(int nodeId)
        {
            var node = currentMap?.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null) return;

            var matches = nodeLayerPools
                .Where(s => s.Type == node.Type && node.Layer >= s.MinLayer && node.Layer <= s.MaxLayer)
                .ToList();

            if (matches.Count == 0)
            {
                Debug.LogWarning($"Nenhum LevelPool configurado para {node.Type} na camada {node.Layer}.");
                return;
            }
            if (matches.Count > 1)
            {
                Debug.LogWarning($"Mais de um LevelPool cobre {node.Type} na camada {node.Layer} — usando o primeiro encontrado. Confira sobreposições em Node Layer Pools.");
            }

            var pool = matches[0].Pool;
            if (pool == null)
            {
                Debug.LogWarning($"A entrada para {node.Type} na camada {node.Layer} não tem um LevelPool atribuído.");
                return;
            }

            GameManager.Instance.StartLevel(pool);
        }

        private void RefreshMapVisuals()
        {
            RefreshAllNodeStates();
            RefreshAllLineStates();
        }

        private void RefreshAllNodeStates()
        {
            if (currentMap == null || spawnedNodes == null) return;

            foreach (var node in currentMap.Nodes)
            {
                if (!spawnedNodes.TryGetValue(node.Id, out var go)) continue;
                var clickHandler = go.GetComponentInChildren<MapNodeClickHandler>();
                if (clickHandler != null)
                    clickHandler.SetProgressState(GetNodeState(node.Id));
            }
        }

        private void RefreshAllLineStates()
        {
            if (currentMap == null || spawnedLines == null) return;

            foreach (var node in currentMap.Nodes)
            {
                foreach (var targetId in node.ConnectionsToNextLayer)
                {
                    string key = LineKey(node.Id, targetId);
                    if (!spawnedLines.TryGetValue(key, out var lr)) continue;
                    ApplyLineVisual(key, lr, IsEdgeTraversed(node.Id, targetId));
                }
            }
        }

        private bool IsEdgeTraversed(int fromId, int toId)
        {
            int fromIndex = visitedPath.IndexOf(fromId);
            if (fromIndex < 0 || fromIndex + 1 >= visitedPath.Count) return false;
            return visitedPath[fromIndex + 1] == toId;
        }

        private void ApplyLineVisual(string key, LineRenderer lr, bool traversed)
        {
            bool hasInstance = lineMaterialInstances.TryGetValue(key, out var instance) && instance != null;
            bool stateChanged = !lineTraversedState.TryGetValue(key, out var previousTraversed) || previousTraversed != traversed;

            if (!hasInstance || stateChanged)
            {
                var baseMat = traversed
                    ? (lineMaterial != null ? lineMaterial : GetRuntimeSolidMaterial())
                    : (dashedLineMaterial != null ? dashedLineMaterial : GetRuntimeDashedMaterial());

                if (hasInstance)
                {
                    #if UNITY_EDITOR
                    if (!Application.isPlaying) DestroyImmediate(instance);
                    else Destroy(instance);
                    #else
                    Destroy(instance);
                    #endif
                }

                instance = new Material(baseMat);
                lineMaterialInstances[key] = instance;
                lineTraversedState[key] = traversed;
                lr.sharedMaterial = instance;
            }

            lr.startColor = lr.endColor = traversed ? traversedLineColor : lineColor;
            lr.textureMode = traversed ? LineTextureMode.Stretch : LineTextureMode.Tile;

            if (!traversed)
            {
                float length = Vector3.Distance(lr.GetPosition(0), lr.GetPosition(1));
                instance.mainTextureScale = new Vector2(Mathf.Max(1f, length * dashTilingPerUnit), 1f);
            }
        }

        private Material GetRuntimeSolidMaterial()
        {
            if (runtimeSolidMaterial == null)
                runtimeSolidMaterial = new Material(Shader.Find("Sprites/Default"));
            return runtimeSolidMaterial;
        }

        private Material GetRuntimeDashedMaterial()
        {
            if (runtimeDashedMaterial == null)
            {
                var tex = new Texture2D(4, 1, TextureFormat.RGBA32, false);
                tex.wrapMode = TextureWrapMode.Repeat;
                tex.filterMode = FilterMode.Point;
                tex.SetPixels(new[]
                {
                    Color.white, Color.white,
                    new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 0f)
                });
                tex.Apply();

                runtimeDashedMaterial = new Material(Shader.Find("Sprites/Default"));
                runtimeDashedMaterial.mainTexture = tex;
            }
            return runtimeDashedMaterial;
        }
    }
}