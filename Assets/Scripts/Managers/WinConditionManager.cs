using Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class WinConditionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Entity player;
        [SerializeField] private EnemyGridController gridController;

        [SerializeField] private ModalScriptableObject winMessage;
        [SerializeField] private ModalScriptableObject loseMessage;
        
        private void OnEnable()
        {
            player.OnDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            player.OnDeath -= HandlePlayerDeath;
            
            foreach (var enemy in gridController.SpawnedEnemies)
            {
                enemy.OnDeath -= EvaluateWinCondition;
            }
        }
        
        public void InitializeEnemies()
        {
            foreach (var enemy in gridController.SpawnedEnemies)
            {
                enemy.OnDeath += EvaluateWinCondition;
            }
        }

        private void HandlePlayerDeath()
        {
            EventBus<PauseMetronome>.Raise(new PauseMetronome());
            ModalManager.Instance.ShowModal(loseMessage, onConfirm: () => {SceneManager.LoadScene("MainMenu");});
        }

        private void EvaluateWinCondition()
        {
            bool allEnemiesDead = true;

            foreach (var enemy in gridController.SpawnedEnemies)
            {
                if (!enemy.IsDead)
                {
                    allEnemiesDead = false;
                    break;
                }
            }

            if (allEnemiesDead)
            {
                EventBus<PauseMetronome>.Raise(new PauseMetronome());
                ModalManager.Instance.ShowModal(winMessage, onConfirm: () => {SceneManager.LoadScene("MapScene");});
            }
        }
    }
}