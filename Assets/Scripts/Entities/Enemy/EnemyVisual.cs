using UnityEngine;

namespace Entities
{
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

        private EventBinding<Tick> _tickBinding;
        private EventBinding<ActionExecutedEvent> _actionBinding;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
            spriteRenderer = GetComponent<SpriteRenderer>();
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

            Entity.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            EventBus<Tick>.Deregister(_tickBinding);
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);

            Entity.OnDeath -= HandleDeath;
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

            healthGrid.gameObject.SetActive(false);
            ammoGrid.gameObject.SetActive(false);

            EventBus<Tick>.Deregister(_tickBinding);
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);
        }
        
        public void SelectedAsTarget()
        {
            Debug.Log($"{Entity.name} Selected as target");
        }
    }
}