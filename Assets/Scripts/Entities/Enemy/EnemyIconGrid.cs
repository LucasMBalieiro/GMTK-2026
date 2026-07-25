using UnityEngine;

namespace Entities
{
    public class EnemyIconGrid : MonoBehaviour
    {
        [SerializeField] private Sprite fullSprite;
        [SerializeField] private Sprite emptySprite;
        
        [SerializeField] private SpriteRenderer[] containers;
        [SerializeField] private bool isHealthContainer;
        
        [SerializeField] private Entity entity;
        
        private EventBinding<ResetConditions> onReset;
        private int maxContainers;

        private void Awake()
        {
            maxContainers = isHealthContainer ? entity.data.maxHealth : entity.data.maxAmmo;
            
            for (int i = 0; i < maxContainers; i++)
            {
                containers[i].gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            onReset = new EventBinding<ResetConditions>(UpdateUI);
            EventBus<ResetConditions>.Register(onReset);
            
            InitialUI();
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(onReset);
        }

        private void InitialUI()
        {
            int currentValue = isHealthContainer ? entity.data.maxHealth : entity.data.startingAmmo;

            for (int i = 0; i < currentValue; i++)
            {
                containers[i].sprite = fullSprite;
            }
            
            for (int i = currentValue; i < maxContainers; i++)
            {
                containers[i].sprite = emptySprite;
            }
        }

        private void UpdateUI()
        {
            int currentValue = isHealthContainer ? entity.CurrentHealth : entity.CurrentAmmo;

            for (int i = 0; i < currentValue; i++)
            {
                containers[i].sprite = fullSprite;
            }
            
            for (int i = currentValue; i < maxContainers; i++)
            {
                containers[i].sprite = emptySprite;
            }
        }
    }
}

