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
        
        private static readonly Vector3 FirstPos = new Vector3(0, -20, 0);
        private static readonly Vector3 SecondPos = new Vector3(-90, -10, 0);
        private static readonly Vector3 ThirdPos = new Vector3(90, -10, 0);
        
        private readonly Vector3[] positions = new []{FirstPos, SecondPos, ThirdPos};
        

        private void Start()
        {
            enemiesToLoad = GameManager.Instance.LevelData;
            var i = 0;

            foreach (var enemySO in enemiesToLoad.enemies)
            {
                var enemy = Instantiate(combatantAIController, transform);
                enemy.transform.position = positions[i];
                i++;
                
                enemy.Initialize(enemySO, player);
            }
        }
    }
}