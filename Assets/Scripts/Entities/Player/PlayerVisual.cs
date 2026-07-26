using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        [Header("Defend Animation")]
        [SerializeField] private SpriteRenderer defendRenderer; 
        [SerializeField] private Sprite[] defendSprites;

        [Header("Reload Animation")]
        [SerializeField] private SpriteRenderer reloadRenderer;
        [SerializeField] private SpriteRenderer gunRenderer;
        
        [SerializeField] private Sprite[] reloadSprites;

        [Space]
        [SerializeField] private float framesPerSecond = 12f;

        private EventBinding<ActionExecutedEvent> _actionBinding;
        
        private List<SpriteRenderer> spawnedHearts = new List<SpriteRenderer>();
        private List<SpriteRenderer> spawnedBullets = new List<SpriteRenderer>();
        
        private EventBinding<ResetConditions> resetBinding;
        
        private int previousHealth;
        private float damageAlpha = 0f;
        private float healAlpha = 0f;
        [SerializeField] private float fadeSpeed = 2f; 
        
        // 1. Add a CancellationTokenSource to track the active animation
        private CancellationTokenSource _animationCts;

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
            
            _actionBinding = new EventBinding<ActionExecutedEvent>(OnActionExecuted);
            EventBus<ActionExecutedEvent>.Register(_actionBinding);

            controller.UpdateVisual += InitialUI;
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(resetBinding); 
            
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);
            controller.UpdateVisual -= InitialUI; 
            
            // Clean up the token when the component is disabled
            _animationCts?.Cancel();
            _animationCts?.Dispose();
        }
        
        private void OnActionExecuted(ActionExecutedEvent eventData)
        {
            if (eventData.Caster != player) return;

            SpriteRenderer targetRenderer = null;
            Sprite[] actionSequence = null;

            switch (eventData.SkillType)
            {
                case SkillType.Defend:
                    targetRenderer = defendRenderer;
                    actionSequence = defendSprites;
                    break;
                case SkillType.Reload:
                    targetRenderer = reloadRenderer;
                    actionSequence = reloadSprites;
                    break;
            }

            if (targetRenderer != null && actionSequence != null && actionSequence.Length > 0)
            {
                // 2. Cancel the previous animation if it is still running
                _animationCts?.Cancel();
                _animationCts = new CancellationTokenSource();
                
                // Link our animation cancellation with Unity's destruction lifecycle
                CancellationToken linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    _animationCts.Token, 
                    this.GetCancellationTokenOnDestroy()
                ).Token;

                PlayActionSequence(targetRenderer, actionSequence, targetRenderer == reloadRenderer, linkedToken).Forget(); 
            }
        }

        private async UniTaskVoid PlayActionSequence(SpriteRenderer targetRenderer, Sprite[] sprites, bool isGun, CancellationToken token)
        {
            try
            {
                if(isGun) gunRenderer.enabled = false;
                targetRenderer.enabled = true;

                float frameDelay = 1f / framesPerSecond;

                for (int i = 0; i < sprites.Length; i++)
                {
                    targetRenderer.sprite = sprites[i];
                    
                    // 3. Pass the token into the delay
                    await UniTask.Delay(TimeSpan.FromSeconds(frameDelay), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                // 4. Silently catch the cancellation so it doesn't throw a console error when interrupted
            }
            finally
            {
                // 5. The finally block guarantees the renderers are reset, even if the animation was cancelled mid-way!
                if (targetRenderer != null) targetRenderer.enabled = false;
                if (isGun && gunRenderer != null) gunRenderer.enabled = true;
            }
        }

        private void InitialUI()
        {
            // 1. Destroy the actual GameObjects in the scene first
            foreach (var heart in spawnedHearts)
            {
                if (heart != null) Destroy(heart.gameObject);
            }
            foreach (var bullet in spawnedBullets)
            {
                if (bullet != null) Destroy(bullet.gameObject);
            }

            // 2. Now it is safe to clear the reference lists
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