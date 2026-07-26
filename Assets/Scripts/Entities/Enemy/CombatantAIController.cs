using AudioSystem;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class CombatantAIController  : MonoBehaviour
    {
        private Entity playerTarget;
        private EnemyDataSO enemyDataSO;
        private EnemyVisual visual;

        [SerializeField] private SoundData reloadSound;
        
        
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
            visual = GetComponent<EnemyVisual>();
            self.IsPlayer = false;
        }

        public void Initialize(EnemyDataSO data, Entity player)
        {
            enemyDataSO = data;
            playerTarget = player;
            
            self.InitializeData(enemyDataSO.data);
            visual.InitializeData(enemyDataSO);
            
            attack = new Skill(SkillType.Attack, playerTarget, self.data.attackDamage);
            reload = new Skill(SkillType.Reload, self, self.data.reloadAmount);
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
            
            bool canDefend = consecutiveDefends < self.data.defencesInSequence;
            
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