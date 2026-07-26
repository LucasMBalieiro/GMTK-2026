using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Entities
{
    public class AttackTargeting : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Entity playerEntity;
        
        [SerializeField] private TextMeshProUGUI text;
    
        [SerializeField] private Sprite crosshair;

        [SerializeField] private SpriteRenderer gunRenderer;
        [SerializeField] private Sprite regularGun;
        [SerializeField] private Sprite highlightedGun;
        
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
            if (playerEntity.CurrentAmmo <= 0) return;
            
            if (crosshair) image.sprite = crosshair;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
            Cursor.visible = false;
            image.raycastTarget = false;
        
            originalPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (playerEntity.CurrentAmmo <= 0) return;
            
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (playerEntity.CurrentAmmo <= 0) return;
            
            transform.position = originalPosition;
            image.sprite = startingSprite;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
            Cursor.visible = true;
            
            Vector2 mousePosition2D = mainCamera.ScreenToWorldPoint(eventData.position);
        
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
        
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject);
                
                if (hit.collider.TryGetComponent(out EnemyVisual enemy))
                {
                    if (!enemy.Entity.IsDead)
                    {
                        enemy.SelectedAsTarget();
                        playerController.Attack(enemy.Entity);
                    }
                }
            }
            
            image.raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (playerEntity.CurrentAmmo > 0) 
            {
                gunRenderer.sprite = highlightedGun;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gunRenderer.sprite = regularGun;
        }
    }
}