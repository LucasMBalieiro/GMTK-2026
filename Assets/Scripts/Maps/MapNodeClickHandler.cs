using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap
{
    public class MapNodeClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public int NodeId;
        public NodeType Type;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Clicou no nó {NodeId} ({Type})");
        }
    }
}