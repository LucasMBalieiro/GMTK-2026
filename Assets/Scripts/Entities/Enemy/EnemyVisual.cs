using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity), typeof(SpriteRenderer))]
    public class EnemyVisual : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        private EnemyDataSO enemyDataSO;
        
        private SpriteRenderer _spriteRenderer;
        private bool _isIdle1 = true;
        private int _holdPoseTicks = 0;

        private EventBinding<Tick> _tickBinding;
        private EventBinding<ActionExecutedEvent> _actionBinding;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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
        }

        private void OnDisable()
        {
            EventBus<Tick>.Deregister(_tickBinding);
            EventBus<ActionExecutedEvent>.Deregister(_actionBinding);
        }

        private void OnTick()
        {
            if (_holdPoseTicks > 0)
            {
                _holdPoseTicks--;
                return;
            }

            _isIdle1 = !_isIdle1;
            _spriteRenderer.sprite = _isIdle1 ? enemyDataSO.idle1 : enemyDataSO.idle2;
        }

        private void OnActionExecuted(ActionExecutedEvent eventData)
        {
            if (eventData.Caster != Entity) return;
            
            if (eventData.SkillType == SkillType.Attack)
            {
                _spriteRenderer.sprite = enemyDataSO.attack;
            }
            else if (eventData.SkillType == SkillType.Defend)
            {
                _spriteRenderer.sprite = enemyDataSO.defend;
            }
            
            _holdPoseTicks = 1;
        }
        
        public void SelectedAsTarget()
        {
            Debug.Log($"{Entity.name} Selected as target");
        }
    }
}