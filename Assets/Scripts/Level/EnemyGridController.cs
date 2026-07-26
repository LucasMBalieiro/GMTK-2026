using Entities;
using Level;
using UnityEngine;

namespace Managers
{
    public class EnemyGridController : MonoBehaviour
    {
        private LevelData enemiesToLoad;
        [SerializeField] private CombatantAIController combatantAIController;
        [SerializeField] private Entity player;
        

        private void Start()
        {
            enemiesToLoad = GameManager.Instance.LevelData;

            foreach (var enemySO in enemiesToLoad.enemies)
            {
                var enemy = Instantiate(combatantAIController, transform);
                enemy.Initialize(enemySO, player);
            }
        }
    }
}