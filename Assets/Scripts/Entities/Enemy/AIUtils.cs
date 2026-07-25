namespace Entities
{
    public static class AIUtils
    {
        public static Skill GetRandom(Skill a, Skill b)
        {
            return UnityEngine.Random.value > 0.5f ? a : b;
        }
        
        public static Skill GetRandom(Skill a, Skill b, Skill c)
        {
            float roll = UnityEngine.Random.value;
            return roll switch
            {
                <= 0.33f => a,
                <= 0.66f => b,
                _ => c
            };
        }
    }
}