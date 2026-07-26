using UnityEngine;


namespace Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Entity/Level Data")]
    public class LevelData : ScriptableObject
    {
        public EnemyDataSO[] enemies;
        public int coinsOnWin;
    }
}

