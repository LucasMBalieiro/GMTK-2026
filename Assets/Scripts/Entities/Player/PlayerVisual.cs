using System;
using TMPro;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class PlayerVisual : MonoBehaviour
    {
        private Entity player;

        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI bulletText;
        
        private EventBinding<ResetConditions>  resetBinding;
        
        private void Awake()
        {
            player = GetComponent<Entity>();
        }

        private void OnEnable()
        {
            resetBinding = new EventBinding<ResetConditions>(RefreshUI);
            EventBus<ResetConditions>.Register(resetBinding);

            InitialUI();
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(resetBinding);
        }

        private void InitialUI()
        {
            healthText.text = $"Health: {player.data.maxHealth}/{player.data.maxHealth}";
            bulletText.text = $"Bullet {player.data.startingAmmo}/{player.data.maxAmmo}";
        }

        private void RefreshUI()
        {
            healthText.text = $"Health: {player.CurrentHealth}/{player.data.maxHealth}";
            bulletText.text = $"Bullet {player.CurrentAmmo}/{player.data.maxAmmo}";
        }
    }
}