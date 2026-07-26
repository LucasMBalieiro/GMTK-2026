using System;
using TMPro;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity), typeof(PlayerController))]
    public class PlayerVisual : MonoBehaviour
    {
        private Entity player;
        private PlayerController controller;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI bulletText;
        
        private EventBinding<ResetConditions> resetBinding;
        
        private void Awake()
        {
            player = GetComponent<Entity>();
            controller = GetComponent<PlayerController>();
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
            if (healthText == null || bulletText == null)
            {
                Debug.LogError($"[{gameObject.name}] UI Text references are missing in the Inspector!", this);
                return;
            }

            healthText.text = $"Health: {player.data.maxHealth}/{player.data.maxHealth}";
            bulletText.text = $"Bullet {player.data.startingAmmo}/{player.data.maxAmmo}";
        }

        private void RefreshUI()
        {
            if (healthText == null || bulletText == null) return;
            
            healthText.text = $"Health: {player.CurrentHealth}/{player.data.maxHealth}";
            bulletText.text = $"Bullet {player.CurrentAmmo}/{player.data.maxAmmo}";
        }
    }
}