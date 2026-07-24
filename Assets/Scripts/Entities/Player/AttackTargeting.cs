using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Entities
{
    public class AttackTargeting : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private PlayerInputHandler playerInputHandler;

        [SerializeField] private TextMeshProUGUI text;
    
        [SerializeField] private Sprite crosshair;
        private Sprite startingSprite;
    
        private Vector3 originalPosition;
        private Camera mainCamera;
        private Image image;
    
        private void Start()
        {
            mainCamera = Camera.main;
            image = GetComponent<Image>();
            startingSprite = image.sprite;
        }
    
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(crosshair) image.sprite = crosshair;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            Cursor.visible = false;
        
            originalPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //TODO: tentar um lerp talvez
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = originalPosition;
            image.sprite = startingSprite;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
            Cursor.visible = true;
        
            Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out EnemyVisual enemy))
                {
                    if (!enemy.Entity.IsDead)
                    {
                        enemy.SelectedAsTarget();
                        playerInputHandler.Attack(enemy.Entity);
                    }
                }
            }
        }
    }
}


