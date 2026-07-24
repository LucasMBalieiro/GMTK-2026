using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityData data;
        
        public int CurrentHealth { get; private set; }
        public int CurrentAmmo { get; private set; }
        
        private bool canHeal = true;
        private bool isDefending = false;

        private void Start()
        {
            CurrentHealth = data.maxHealth;
            CurrentAmmo = data.startingAmmo;
        }

        public void Defend()
        {
            Debug.Log($"{gameObject.name} raises their shield.");
            isDefending = true;
        }

        public void TakeDamage(int amount)
        {
            if(isDefending) return;

            Debug.Log($"{gameObject.name} took {amount} damage");
            CurrentHealth -= amount;
            canHeal = false;
        }

        public void Reload(int amount)
        {
            Debug.Log($"{gameObject.name} reload");
            CurrentAmmo = Mathf.Min(CurrentAmmo + amount, data.maxAmmo);
        }

        public void Heal(int amount)
        {
            if (canHeal)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + amount, data.maxHealth);
                Debug.Log($"{gameObject.name} healed for {amount}.");
            }
            else
            {
                Debug.Log($"{gameObject.name} cant heal");
            }
        }

        public void ResetConditions()
        {
            canHeal = true;
            isDefending = false;
        }
    }
}