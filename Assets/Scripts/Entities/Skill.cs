using System;
using UnityEngine;

namespace Entities
{
    public enum SkillType
    {
        Defend,
        Attack,
        Reload,
        Heal
    }
    
    [Serializable]
    public class Skill
    {
        public SkillType type;
        [HideInInspector] public Entity target;
        public int value;
        
        public Skill(SkillType type, int value)
        {
            this.type = type;
            target = null;
            this.value = value;
        }
        public Skill(SkillType type, Entity target, int value)
        {
            this.type = type;
            this.target = target;
            this.value = value;
        }
    }
}