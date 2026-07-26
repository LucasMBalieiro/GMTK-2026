using System.Collections.Generic;
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
        
        // 1. Add a reference to the new WinConditionManager
        [SerializeField] private WinConditionManager winManager;
        
        private static readonly Vector3 FirstPos = new Vector3(0, -20, 0);
        private static readonly Vector3 SecondPos = new Vector3(-90, -10, 0);
        private static readonly Vector3 ThirdPos = new Vector3(90, -10, 0);
        
        private readonly Vector3[] positions = new []{FirstPos, SecondPos, ThirdPos};
        
        // 2. Create a public list to hold the spawned Entity references
        public List<Entity> SpawnedEnemies { get; private set; } = new List<Entity>();

        private void Start()
        {
            enemiesToLoad = GameManager.Instance.LevelData;
            var i = 0;

            foreach (var enemySO in enemiesToLoad.enemies) //[cite: 23]
            {
                var enemy = Instantiate(combatantAIController, transform); //[cite: 23]
                enemy.transform.position = positions[i]; //[cite: 23]
                i++;
                
                enemy.Initialize(enemySO, player); //[cite: 23]
                
                // 3. Track the spawned entity
                SpawnedEnemies.Add(enemy.GetComponent<Entity>());
            }
            
            // 4. Notify the WinConditionManager that spawning is complete
            if (winManager != null) winManager.InitializeEnemies();
        }
    }
}