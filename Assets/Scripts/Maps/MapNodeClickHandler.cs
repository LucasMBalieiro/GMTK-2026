using UnityEngine;

namespace RoguelikeMap
{
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