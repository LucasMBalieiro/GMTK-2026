using UnityEngine;


namespace Entities
{
    [CreateAssetMenu(fileName = "EntityData", menuName = "Entity/EntityData")]
    public class EntityData : ScriptableObject
    {
        public int maxHealth;
        public int maxAmmo;
        public int startingAmmo;
    }
}
