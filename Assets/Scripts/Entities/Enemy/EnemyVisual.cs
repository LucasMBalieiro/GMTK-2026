using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class EnemyVisual : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        [SerializeField] private EnemyIconGrid healthGrid;
        [SerializeField] private EnemyIconGrid bulletGrid;

        private void Awake()
        {
            Entity =  GetComponent<Entity>();
        }
        
        public void SelectedAsTarget()
        {
            Debug.Log($"{Entity.name}Selected as target");
        }
    }
}