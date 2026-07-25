using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class PlayerController : MonoBehaviour
    {
        private Entity self;

        //TODO: Fazer uma versão unica disso no GameManager
        [SerializeField] private Stats PlayerStats;
        
        private Skill reload;
        private Skill attack;
        private Skill defend;
        private Skill heal;
        
        private void Awake()
        {
            self = GetComponent<Entity>();
            self.IsPlayer = true;
            
            SetSkills();
        }

        private void SetSkills()
        {
            defend = new Skill(SkillType.Defend, self, 0);
            attack = new Skill(SkillType.Attack, PlayerStats.attackDamage);
            reload = new Skill(SkillType.Reload, self, PlayerStats.reloadAmount);
            heal = new Skill(SkillType.Heal, self, self.data.maxHealth);
            
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