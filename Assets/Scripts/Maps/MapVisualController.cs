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

        [Header("Linhas")]
        public Material lineMaterial;
        public Color lineColor = Color.white;
        public float lineWidth = 0.08f;

        [Header("Câmera")]
        public Camera targetCamera;   
        public bool fitCameraToMap = true;
        public float cameraPadding = 0.3f;
        [Range(0.4f, 1f)]
        [Tooltip("Quanto da tela o grafo deve preencher. 0.8 = grafo ocupa 80%, sobrando 20% de respiro nas bordas.")]
        public float viewportFillPercent = 0.8f;

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
            SpawnConnections(map, spawned);

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
                spawned[node.Id] = go;
            }
            return spawned;
        }

        private void SpawnConnections(MapData map, Dictionary<int, GameObject> spawnedNodes)
        {
            foreach (var node in map.Nodes)
            {
                foreach (var targetId in node.ConnectionsToNextLayer)
                {
                    var lineGO = new GameObject($"Line_{node.Id}_{targetId}");
                    lineGO.transform.SetParent(transform);

                    var lr = lineGO.AddComponent<LineRenderer>();
                    lr.material = lineMaterial;
                    lr.startColor = lr.endColor = lineColor;
                    lr.startWidth = lr.endWidth = lineWidth;
                    lr.positionCount = 2;
                    lr.sortingOrder = 0;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, node.Position);
                    lr.SetPosition(1, map.Nodes.First(n => n.Id == targetId).Position);
                }
            }
        }

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
    }
    public class MapNodeClickHandler : MonoBehaviour
    {
        public int NodeId;
        public NodeType Type;

        private void OnMouseDown()
        {
            Debug.Log($"Clicou no nó {NodeId} ({Type})");
        }
    }
}