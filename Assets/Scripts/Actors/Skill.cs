using System;

namespace Actors
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
        public Entity target;
        public int value;
        
        public Skill(SkillType type, int value)
        {
            this.type = type;
            target = null;
            this.value = value;
        }

        public void SetTarget(Entity entity)
        {
            this.target = entity;
        }
    }
}