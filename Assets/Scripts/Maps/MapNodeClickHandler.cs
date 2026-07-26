using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap
{
    public class MapNodeClickHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int NodeId;
        public NodeType Type;

        [HideInInspector] public MapVisualController Controller;

        [Header("Hover feedback (só ocorre em nós Available)")]
        [SerializeField] private float hoverScaleMultiplier = 1.15f;
        [SerializeField] private Color hoverOutlineColor = Color.white;
        [Tooltip("Escala do halo em relação ao ícone. >1 cria o efeito de borda ao redor.")]
        [SerializeField] private float hoverOutlineScale = 1.25f;

        private SpriteRenderer nodeRenderer;
        private SpriteRenderer outlineRenderer;
        private Sprite originalSprite;
        private Vector3 baseScale = Vector3.one;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentState != NodeProgressState.Available) return;
            SetHoverVisual(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetHoverVisual(false);
        }

        public void Initialize(SpriteRenderer renderer)
        {
            nodeRenderer = renderer;
            originalSprite = renderer != null ? renderer.sprite : null;
            baseScale = renderer != null ? renderer.transform.localScale : Vector3.one;

            CreateOutlineRenderer();
        }

        public void SetProgressState(NodeProgressState state)
        {
            currentState = state;
            if (nodeRenderer == null) return;

            bool locked = state == NodeProgressState.Locked;
            nodeRenderer.sprite = (locked && Controller != null && Controller.lockedNodeSprite != null)
                ? Controller.lockedNodeSprite
                : originalSprite;

            if (state != NodeProgressState.Available)
                SetHoverVisual(false);
        }

        private void SetHoverVisual(bool hovering)
        {
            if (nodeRenderer == null) return;

            nodeRenderer.transform.localScale = hovering ? baseScale * hoverScaleMultiplier : baseScale;
            if (outlineRenderer != null)
                outlineRenderer.enabled = hovering;
        }

        private void CreateOutlineRenderer()
        {
            if (nodeRenderer == null || outlineRenderer != null) return;

            var outlineGO = new GameObject("HoverOutline");
            outlineGO.transform.SetParent(nodeRenderer.transform, false);
            outlineGO.transform.localPosition = Vector3.zero;
            outlineGO.transform.localScale = Vector3.one * hoverOutlineScale;

            outlineRenderer = outlineGO.AddComponent<SpriteRenderer>();
            outlineRenderer.sprite = originalSprite;
            outlineRenderer.color = hoverOutlineColor;
            outlineRenderer.sortingOrder = nodeRenderer.sortingOrder - 1;
            outlineRenderer.enabled = false;
        }
    }
}