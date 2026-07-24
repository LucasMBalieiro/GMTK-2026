using System;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        public EntityData Data { get; private set; }

        private int CurrentHealth { get; set; }
        private int CurrentAmmo { get; set; }
        public bool IsPlayer { get; set; } 
        public bool IsDead => CurrentHealth <= 0;
        
        private bool canHeal = true;
        private bool isDefending = false;
        
        public event Action OnDeath;
        private EventBinding<ResetConditions> resetBinding;

        private void Start()
        {
            CurrentHealth = Data.maxHealth;
            CurrentAmmo = Data.startingAmmo;
        }

        private void OnEnable()
        {
            resetBinding = new EventBinding<ResetConditions>(ResetConditions);
            EventBus<ResetConditions>.Register(resetBinding);
        }

        private void OnDisable()
        {
            EventBus<ResetConditions>.Deregister(resetBinding);
        }

        public void Defend()
        {
            Debug.Log($"{gameObject.name} defend");
            isDefending = true;
        }

        public void TakeDamage(int amount)
        {
            if(isDefending) return;

            Debug.Log($"{gameObject.name} took {amount} damage");
            
            CurrentHealth -= amount;
            
            if (IsDead)
            {
                //TODO: Handle deaths
                OnDeath?.Invoke();
            }
            canHeal = false;
        }

        public void Reload(int amount)
        {
            Debug.Log($"{gameObject.name} reload");
            CurrentAmmo = Mathf.Min(CurrentAmmo + amount, Data.maxAmmo);
        }

        public void Heal(int amount)
        {
            if (canHeal)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + amount, Data.maxHealth);
                Debug.Log($"{gameObject.name} healed {amount}.");
            }
            else
            {
                Debug.Log($"{gameObject.name} heal canceled");
            }
        }

        private void ResetConditions()
        {
            canHeal = true;
            isDefending = false;
        }
    }
}