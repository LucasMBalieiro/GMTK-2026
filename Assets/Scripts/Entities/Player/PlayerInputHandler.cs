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
            self.IsPlayer = true;
            
            reload.target = self;
            defend.target = self;
            heal.target = self;
        }

        public void Reload()
        {
            EventBus<AddSkillEvent>.Raise(new AddSkillEvent { caster = self, skill = reload });
        }

        public void Defend()
        {
            EventBus<AddSkillEvent>.Raise(new AddSkillEvent { caster = self, skill = defend });
        }

        public void Heal()
        {
            EventBus<AddSkillEvent>.Raise(new AddSkillEvent { caster = self, skill = heal });
        }

        public void Attack(Entity target)
        {
            attack.target = target;
            EventBus<AddSkillEvent>.Raise(new AddSkillEvent { caster = self, skill = attack });
        }
    }
}