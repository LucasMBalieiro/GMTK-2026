using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    [RequireComponent(typeof(Entity), typeof(PlayerController))]
    public class PlayerVisual : MonoBehaviour
    {
        private Entity player;
        private PlayerController controller;

        [SerializeField] private SpriteRenderer heartPrefab;
        [SerializeField] private SpriteRenderer bulletPrefab;

        [SerializeField] private Transform heartParent;
        [SerializeField] private Transform bulletParent;
        
        [SerializeField] private Sprite heartFull;
        [SerializeField] private Sprite heartEmpty;
        [SerializeField] private Sprite bulletFull;
        [SerializeField] private Sprite bulletEmpty;

        [SerializeField] private Image healingOverlay;
        [SerializeField] private Image damagedOverlay;

        [Header("Defense overlay")]
        [SerializeField] private Button defendButton;
        [SerializeField] private Image defendIconSpriteRenderer;
        [SerializeField] private Sprite canDefendSprite;
        [SerializeField] private Sprite cantDefendSprite;
        
        [Header("Heal overlay")]
        [SerializeField] private Button healButton;
        [SerializeField] private Image healIconSpriteRenderer;
        [SerializeField] private Sprite canHealSprite;
        [SerializeField] private Sprite cantHealSprite;
        
        [Header("Reload overlay")]
        [SerializeField] private Button reloadButton;
        
        private List<SpriteRenderer> spawnedHearts = new List<SpriteRenderer>();
        private List<SpriteRenderer> spawnedBullets = new List<SpriteRenderer>();
        
        private EventBinding<ResetConditions> resetBinding;
        
        private int previousHealth;
        private float damageAlpha = 0f;
        private float healAlpha = 0f;
        [SerializeField] private float fadeSpeed = 2f; 
        
        private void Awake()
        {
            player = GetComponent<Entity>();
            controller = GetComponent<PlayerController>(); 
            
            SetOverlayAlpha(damagedOverlay, 0f);
            SetOverlayAlpha(healingOverlay, 0f);
        }

        private void OnEnable()
        {
            resetBinding = new EventBinding<ResetConditions>(RefreshUI); 
            EventBus<ResetConditions>.Register(resetBinding); 

            controller.UpdateVisual += InitialUI;
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(resetBinding); 
            controller.UpdateVisual -= InitialUI; 
        }

        private void InitialUI()
        {
            spawnedHearts.Clear();
            spawnedBullets.Clear();

            for (int i = 0; i < player.data.maxHealth; i++)
            {
                SpriteRenderer heart = Instantiate(heartPrefab, heartParent);
                heart.transform.localPosition = new Vector3(23f + (i * 14f), 0f, 0f); 
                spawnedHearts.Add(heart);
            }

            for (int i = 0; i < player.data.maxAmmo; i++)
            {
                SpriteRenderer bullet = Instantiate(bulletPrefab, bulletParent);
                bullet.transform.localPosition = new Vector3(0f + (i * 14f), 0f, 0f); 
                spawnedBullets.Add(bullet);
            }

            previousHealth = player.data.maxHealth;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (player.CurrentHealth < previousHealth)
            {
                damageAlpha = 1f; 
            }
            else if (player.CurrentHealth > previousHealth)
            {
                healAlpha = 1f; 
            }
            previousHealth = player.CurrentHealth;

            for (int i = 0; i < spawnedHearts.Count; i++)
            {
                spawnedHearts[i].sprite = i < player.CurrentHealth ? heartFull : heartEmpty;
            }

            for (int i = 0; i < spawnedBullets.Count; i++)
            {
                spawnedBullets[i].sprite = i < player.CurrentAmmo ? bulletFull : bulletEmpty;
            }
            
            bool canHeal = !player.IsDead && player.CurrentHealth < player.data.maxHealth;
            if (healButton != null) healButton.interactable = canHeal;
            if (healIconSpriteRenderer != null) healIconSpriteRenderer.sprite = canHeal ? canHealSprite : cantHealSprite;

            bool canDefend = !player.IsDead;
            if (defendButton != null) defendButton.interactable = canDefend;
            if (defendIconSpriteRenderer != null) defendIconSpriteRenderer.sprite = canDefend ? canDefendSprite : cantDefendSprite;

            bool canReload = !player.IsDead && player.CurrentAmmo < player.data.maxAmmo;
            if (reloadButton != null) reloadButton.interactable = canReload;
        }

        private void Update()
        {
            if (damageAlpha > 0f)
            {
                damageAlpha -= Time.deltaTime * fadeSpeed;
                SetOverlayAlpha(damagedOverlay, Mathf.Max(0f, damageAlpha));
            }

            if (healAlpha > 0f)
            {
                healAlpha -= Time.deltaTime * fadeSpeed;
                SetOverlayAlpha(healingOverlay, Mathf.Max(0f, healAlpha));
            }
        }

        private void SetOverlayAlpha(Image overlay, float alpha)
        {
            if (!overlay) return;
            Color c = overlay.color;
            c.a = alpha;
            overlay.color = c;
        }
    }
}