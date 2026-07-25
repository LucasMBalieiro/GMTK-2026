using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class CombatantAIController  : MonoBehaviour
    {
        [SerializeField] private Entity playerTarget;
        
        private Entity self;

        [Header("Enemy Skills")]
        private Skill attack;
        private Skill defend;
        private Skill reload;

        private EventBinding<RequestNextActionEvent> _requestActionBinding;
        
        private int consecutiveDefends = 0;

        private void Awake()
        {
            self = GetComponent<Entity>();
            
            self.IsPlayer = false;
            
            attack = new Skill(SkillType.Attack, playerTarget, 1);
            reload = new Skill(SkillType.Reload, self, 1);
            defend = new Skill(SkillType.Defend, self, 0);
        }

        private void Start()
        {
            DecideNextMove();
        }

        private void OnEnable()
        {
            _requestActionBinding = new EventBinding<RequestNextActionEvent>(OnActionRequested);
            EventBus<RequestNextActionEvent>.Register(_requestActionBinding);
        }

        private void OnDisable()
        {
            EventBus<RequestNextActionEvent>.Deregister(_requestActionBinding);
        }

        private void OnActionRequested()
        {
            if (self.IsDead) return;

            DecideNextMove();
        }

        private void DecideNextMove()
        {
            Skill chosenSkill = null;
            
            bool hasAmmo = self.CurrentAmmo > 0;
            bool isFullAmmo = self.CurrentAmmo >= self.data.maxAmmo;
            
            bool canDefend = consecutiveDefends < self.data.stats.defencesInSequence;
            
            if (!hasAmmo)
            {
                chosenSkill = canDefend ? AIUtils.GetRandom(defend, reload) : reload;
            }
            else if (isFullAmmo)
            {
                chosenSkill = canDefend ? AIUtils.GetRandom(defend, attack) : attack;
            }
            else
            {
                if (canDefend)
                {
                    chosenSkill = AIUtils.GetRandom(defend, reload, attack);
                }
                else
                {
                    chosenSkill = AIUtils.GetRandom(reload, attack);
                }
            }
            
            if (chosenSkill.type == SkillType.Defend)
            {
                consecutiveDefends++;
            }
            else
            {
                consecutiveDefends = 0;
            }
            
            EventBus<AddSkillEvent>.Raise(new AddSkillEvent 
            { 
                caster = self, 
                skill = chosenSkill 
            });
        }
    }
}