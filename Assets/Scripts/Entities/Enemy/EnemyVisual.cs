using UnityEngine;

namespace Entities
{
    public struct TargetedEnemy : IEvent 
    {
        public EnemyVisual Target;
        public SpriteRenderer TargetSpriteRenderer;
    }
    
    [RequireComponent(typeof(Entity), typeof(SpriteRenderer))]
    public class EnemyVisual : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        private EnemyDataSO enemyDataSO;
        
        private SpriteRenderer spriteRenderer;
        private bool idleToggle = true;
        private bool holdPose = false;

        [SerializeField] private EnemyIconGrid healthGrid;
        [SerializeField] private EnemyIconGrid ammoGrid;
        [SerializeField] private SpriteRenderer targetRenderer;

        private EventBinding<Tick> _tickBinding;
        private EventBinding<ActionExecutedEvent> _actionBinding;
        
        private EventBinding<TargetedEnemy> _targetedBinding;
        private EventBinding<ResetConditions> _resetBinding;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (targetRenderer != null) targetRenderer.enabled = false;
        }

        public void InitializeData(EnemyDataSO data)
        {
            enemyDataSO = data;
        }

        private void OnEnable()
        {
            _tickBinding = new EventBinding<Tick>(OnTick);
            EventBus<Tick>.Register(_tickBinding);

            _actionBinding = new EventBinding<ActionExecutedEvent>(OnActionExecuted);
            EventBus<ActionExecutedEvent>.Register(_actionBinding);

            _targetedBinding = new EventBinding<TargetedEnemy>(OnTargeted);
            EventBus<TargetedEnemy>.Register(_targetedBinding);

            _resetBinding = new EventBinding<ResetConditions>(OnReset);
            EventBus<ResetConditions>.Register(_resetBinding);

            Entity.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            EventBus<Tick>.Deregister(_tickBinding);
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);
            EventBus<TargetedEnemy>.Deregister(_targetedBinding);
            EventBus<ResetConditions>.Deregister(_resetBinding);

            Entity.OnDeath -= HandleDeath;
        }

        private void OnTargeted(TargetedEnemy eventData)
        {
            if (targetRenderer != null)
            {
                targetRenderer.enabled = (eventData.Target == this);
            }
        }

        private void OnReset()
        {
            if (targetRenderer != null) targetRenderer.enabled = false;
        }

        private void OnTick()
        {
            if (holdPose)
            {
                holdPose = false;
                return;
            }

            idleToggle = !idleToggle;
            spriteRenderer.sprite = idleToggle ? enemyDataSO.idle1 : enemyDataSO.idle2;
        }

        private void OnActionExecuted(ActionExecutedEvent eventData)
        {
            if (eventData.Caster != Entity) return;

            spriteRenderer.sprite = eventData.SkillType switch
            {
                SkillType.Attack => enemyDataSO.attack,
                SkillType.Defend => enemyDataSO.defend,
                SkillType.Reload => enemyDataSO.reload,
                _ => spriteRenderer.sprite
            };

            holdPose = true;
        }

        private void HandleDeath()
        {
            if (enemyDataSO.death != null)
            {
                spriteRenderer.sprite = enemyDataSO.death;
            }

            if (healthGrid != null) healthGrid.gameObject.SetActive(false);
            if (ammoGrid != null) ammoGrid.gameObject.SetActive(false);
            if (targetRenderer != null) targetRenderer.enabled = false;

            EventBus<Tick>.Deregister(_tickBinding);
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);
            EventBus<TargetedEnemy>.Deregister(_targetedBinding);
        }
        
        public void SelectedAsTarget()
        {
            EventBus<TargetedEnemy>.Raise(new TargetedEnemy { Target = this, TargetSpriteRenderer = targetRenderer });
        }
    }
}