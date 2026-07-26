using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [Serializable]
    public struct LevelPhase
    {
        [Tooltip("Nome exato da cena (deve estar em Build Settings > Scenes In Build)")]
        public string SceneName;
        public LevelData Data;
    }

    [CreateAssetMenu(fileName = "LevelPool", menuName = "Level/LevelPool")]
    public class LevelPool : ScriptableObject
    {
        [Tooltip("Fases possíveis deste nível. Uma será sorteada ao iniciar.")]
        public List<LevelPhase> phases = new List<LevelPhase>();
    }
}