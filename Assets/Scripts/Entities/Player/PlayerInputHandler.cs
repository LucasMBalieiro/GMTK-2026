using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private Entity self;
        
        [SerializeField] private Skill reload;
        [SerializeField] private Skill attack;
        [SerializeField] private Skill defend;
        [SerializeField] private Skill heal;
        
        private void Awake()
        {
            self = GetComponent<Entity>();
            
            reload.target = self;
            defend.target = self;
            heal.target = self;
        }

        public void Reload()
        {
            Orchestrator.Instance.AddSkill(self, reload);
        }

        public void Defend()
        {
            Orchestrator.Instance.AddSkill(self, defend);
        }

        public void Heal()
        {
            Orchestrator.Instance.AddSkill(self, heal);
        }

        public void Attack(Entity target)
        {
            attack.target = target;
            Orchestrator.Instance.AddSkill(self, attack);
        }
    }
}