using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Entity))]
    public class EnemyVisual : MonoBehaviour
    {
        public Entity Entity { get; private set; }

        private void Awake()
        {
            Entity =  GetComponent<Entity>();
        }
        
        public void SelectedAsTarget()
        {

        }
    }
}