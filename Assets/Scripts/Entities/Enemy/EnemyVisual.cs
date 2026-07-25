using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity), typeof(SpriteRenderer))]
    public class EnemyVisual : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        [Header("Sprites")]
        [SerializeField] private Sprite idle1;
        [SerializeField] private Sprite idle2;
        [SerializeField] private Sprite attack;
        [SerializeField] private Sprite defend;
        
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
            // If an action just played on the tempo reset, hold the pose for this tick
            if (_holdPoseTicks > 0)
            {
                _holdPoseTicks--;
                return;
            }

            // Otherwise, loop the idle animation
            _isIdle1 = !_isIdle1;
            _spriteRenderer.sprite = _isIdle1 ? idle1 : idle2;
        }

        private void OnActionExecuted(ActionExecutedEvent eventData)
        {
            // Only react if this specific enemy is the one who casted the skill
            if (eventData.Caster != Entity) return;
            
            if (eventData.SkillType == SkillType.Attack)
            {
                _spriteRenderer.sprite = attack;
            }
            else if (eventData.SkillType == SkillType.Defend)
            {
                _spriteRenderer.sprite = defend;
            }
            
            // Tell the Tick method to skip the next idle flip so we can see the action
            _holdPoseTicks = 1;
        }
        
        public void SelectedAsTarget()
        {
            Debug.Log($"{Entity.name} Selected as target");
        }
    }
}