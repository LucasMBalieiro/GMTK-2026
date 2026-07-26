using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class EnemyIconGrid : MonoBehaviour
    {
        [SerializeField] private Sprite fullSprite;
        [SerializeField] private Sprite emptySprite;
        
        [SerializeField] private GameObject containerPrefab;
        private List<SpriteRenderer> containers = new List<SpriteRenderer>();
        
        [SerializeField] private bool isHealthContainer;
        
        [SerializeField] private Entity entity;
        
        private EventBinding<ResetConditions> onReset;
        private int maxContainers;
        
        
        private void Start()
        {
            maxContainers = isHealthContainer ? entity.data.maxHealth : entity.data.maxAmmo;
            
            for (int i = 0; i < maxContainers; i++)
            {
                var container = Instantiate(containerPrefab, transform);
                containers.Add(container.GetComponent<SpriteRenderer>());
            }
            
            InitialUI();
        }

        private void OnEnable()
        {
            onReset = new EventBinding<ResetConditions>(UpdateUI);
            EventBus<ResetConditions>.Register(onReset);
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

