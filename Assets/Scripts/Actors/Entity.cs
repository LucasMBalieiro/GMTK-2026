using UnityEngine;

namespace Actors
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private BaseComponents baseComponents;

        private bool canHeal = true;
        private bool isDefending = false;

        public virtual void Defend()
        {
            Debug.Log("Defend");
            isDefending = true;
        }

        public virtual void TakeDamage()
        {
            if(isDefending) Debug.Log("Defended damage");
            else
            {
                Debug.Log("TakeDamage");
                canHeal = false;
            }
        }

        public virtual void Reload()
        {
            Debug.Log("Reload");
        }

        public virtual void Heal()
        {
            Debug.Log(canHeal ? "Heal" : "Heal Failed");
        }

        public virtual void ResetConditions()
        {
            Debug.Log("ResetConditions");
            canHeal = true;
        }
    }
}