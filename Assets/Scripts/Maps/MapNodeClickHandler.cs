using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap
{
    public class MapNodeClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public int NodeId;
        public NodeType Type;

        [HideInInspector] public MapVisualController Controller;

        private SpriteRenderer nodeRenderer;
        private Sprite originalSprite;
        private NodeProgressState currentState = NodeProgressState.Locked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Controller == null)
            {
                Debug.LogWarning($"Nó {NodeId} sem Controller atribuído.");
                return;
            }
            Controller.TrySelectNode(NodeId);
        }

        public void Initialize(SpriteRenderer renderer)
        {
            nodeRenderer = renderer;
            originalSprite = renderer != null ? renderer.sprite : null;
        }

        public void SetProgressState(NodeProgressState state)
        {
            currentState = state;
            if (nodeRenderer == null) return;

            bool locked = state == NodeProgressState.Locked;
            nodeRenderer.sprite = (locked && Controller != null && Controller.lockedNodeSprite != null)
                ? Controller.lockedNodeSprite
                : originalSprite;
        }
    }
}