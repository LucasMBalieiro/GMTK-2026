using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoguelikeMap
{
    public enum NodeType
    {
        Combat,
        Shop,
        Boss
    }

    public enum NodeProgressState
    {
        Locked,
        Available,
        Visited
    }

    [Serializable]
    public class MapNode
    {
        public int Id;
        public int Layer;
        public int IndexInLayer;
        public NodeType Type;
        public Vector2 Position;
        public List<int> ConnectionsToNextLayer = new List<int>();
    }

    [Serializable]
    public class MapData
    {
        public int Seed;
        public int LevelIndex;
        public List<MapNode> Nodes = new List<MapNode>();
        public List<List<int>> Layers = new List<List<int>>(); 
    }
    public static class MapGenerator
    {
        private const float ShopChance = 0.2f;   
        private const float LayerSpacing = 2.5f;  
        private const float NodeSpacing = 2f;   
        public static int GenerateLevelSeed(int levelIndex, int globalSeed)
        {
            unchecked
            {
                uint h = (uint)globalSeed;
                h ^= (uint)levelIndex + 0x9E3779B9 + (h << 6) + (h >> 2);
                h ^= h >> 16;
                h *= 0x85ebca6b;
                h ^= h >> 13;
                h *= 0xc2b2ae35;
                h ^= h >> 16;
                return (int)h;
            }
        }
        public static MapData Generate(int levelIndex, int globalSeed, int totalNodes = 20,
        int minNodesPerLayer = 2, int maxNodesPerLayer = 4)
        {
            int seed = GenerateLevelSeed(levelIndex, globalSeed);
            var rng = new System.Random(seed);

            var data = new MapData { Seed = seed, LevelIndex = levelIndex };
            int nodeIdCounter = 0;
            int layer = 0;

            var startNode = new MapNode
            {
                Id = nodeIdCounter++,
                Layer = layer,
                IndexInLayer = 0,
                Type = NodeType.Combat,
                Position = new Vector2(layer * LayerSpacing, 0)
            };
            data.Nodes.Add(startNode);
            data.Layers.Add(new List<int> { startNode.Id });
            layer++;

            int remaining = Mathf.Max(0, totalNodes - 2);
            while (remaining > 0)
            {
                int maxForLayer = Mathf.Min(maxNodesPerLayer, remaining);
                int minForLayer = Mathf.Min(minNodesPerLayer, maxForLayer);
                int count = Mathf.Max(1, rng.Next(minForLayer, maxForLayer + 1));

                var layerIds = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    var node = new MapNode
                    {
                        Id = nodeIdCounter++,
                        Layer = layer,
                        IndexInLayer = i,
                        Type = PickNodeType(rng),
                        Position = new Vector2(
                            layer * LayerSpacing,
                            (i - (count - 1) / 2f) * NodeSpacing)
                    };
                    data.Nodes.Add(node);
                    layerIds.Add(node.Id);
                }
                data.Layers.Add(layerIds);
                remaining -= count;
                layer++;
            }

            var bossNode = new MapNode
            {
                Id = nodeIdCounter++,
                Layer = layer,
                IndexInLayer = 0,
                Type = NodeType.Boss,
                Position = new Vector2(layer * LayerSpacing, 0)
            };
            data.Nodes.Add(bossNode);
            data.Layers.Add(new List<int> { bossNode.Id });
            for (int l = 0; l < data.Layers.Count - 1; l++)
            {
                ConnectLayers(data, l, rng);
            }

            return data;
        }

        private static NodeType PickNodeType(System.Random rng)
        {
            return rng.NextDouble() < ShopChance ? NodeType.Shop : NodeType.Combat;
        }
        private static void ConnectLayers(MapData data, int layerIndex, System.Random rng)
        {
            var current = data.Layers[layerIndex].Select(id => data.Nodes[id]).ToList();
            var next = data.Layers[layerIndex + 1].Select(id => data.Nodes[id]).ToList();

            var reached = new HashSet<int>();

            foreach (var node in current)
            {
                int connections = rng.NextDouble() < 0.3 ? 2 : 1; 
                var candidates = next
                    .OrderBy(n => Mathf.Abs(n.IndexInLayer - node.IndexInLayer))
                    .Take(Mathf.Min(connections + 1, next.Count))
                    .OrderBy(_ => rng.Next())
                    .Take(connections)
                    .ToList();

                foreach (var target in candidates)
                {
                    node.ConnectionsToNextLayer.Add(target.Id);
                    reached.Add(target.Id);
                }
            }

            foreach (var orphan in next.Where(n => !reached.Contains(n.Id)))
            {
                var closest = current
                    .OrderBy(n => Mathf.Abs(n.IndexInLayer - orphan.IndexInLayer))
                    .First();
                closest.ConnectionsToNextLayer.Add(orphan.Id);
            }
        }
    }
    public class MapView : MonoBehaviour
    {
        public GameObject nodePrefab;
        public LineRenderer linePrefab;
        public int levelIndex = 0;
        public int globalSeed = 12345;

        private void Start()
        {
            var map = MapGenerator.Generate(levelIndex, globalSeed);
            var spawned = new Dictionary<int, GameObject>();

            foreach (var node in map.Nodes)
            {
                var go = Instantiate(nodePrefab, node.Position, Quaternion.identity, transform);
                go.name = $"Node_{node.Id}_{node.Type}";
                spawned[node.Id] = go;
            }

            foreach (var node in map.Nodes)
            {
                foreach (var targetId in node.ConnectionsToNextLayer)
                {
                    var line = Instantiate(linePrefab, transform);
                    line.positionCount = 2;
                    line.SetPosition(0, node.Position);
                    line.SetPosition(1, map.Nodes.First(n => n.Id == targetId).Position);
                }
            }
        }
    }
}