using System;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class PlayerController : MonoBehaviour
    {
        private Entity self;
        
        private EntityData playerStats;

        public event Action UpdateVisual;
        
        private Skill reload;
        private Skill attack;
        private Skill defend;
        private Skill heal;
        
        private void Awake()
        {
            self = GetComponent<Entity>();
            self.IsPlayer = true;
            playerStats = GameManager.Instance.PlayerStats;
            
            self.InitializeData(playerStats);
            
            SetSkills();
        }

        private void Start()
        {
            // Moved here so PlayerVisual's OnEnable has time to subscribe first!
            UpdateVisual?.Invoke();
        }

        private void SetSkills()
        {
            defend = new Skill(SkillType.Defend, self, 0);
            attack = new Skill(SkillType.Attack, playerStats.attackDamage);
            reload = new Skill(SkillType.Reload, self, playerStats.reloadAmount);
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