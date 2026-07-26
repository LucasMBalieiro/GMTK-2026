using System;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        public EntityData data;

        public int CurrentHealth { get; private set; }
        public int CurrentAmmo { get; private set; }
        public bool IsPlayer { get; set; } 
        public bool IsDead => CurrentHealth <= 0;
        
        private bool canHeal = true;
        private bool isDefending = false;
        
        private EventBinding<ResetConditions> resetBinding;
        
        public void InitializeData(EntityData newData)
        {
            data = newData;
            CurrentHealth = data.maxHealth;
            CurrentAmmo = data.startingAmmo;
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
            }
            canHeal = false;
        }

        public void Reload(int amount)
        {
            Debug.Log($"{gameObject.name} reload");
            CurrentAmmo = Mathf.Min(CurrentAmmo + amount, data.maxAmmo);
        }
        
        public void ConsumeAmmo(int amount)
        {
            CurrentAmmo = Mathf.Max(0, CurrentAmmo - amount);
            Debug.Log($"{gameObject.name} consumed {amount} ammo. Ammo left: {CurrentAmmo}");
        }

        public void Heal(int amount)
        {
            if (canHeal)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + amount, data.maxHealth);
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