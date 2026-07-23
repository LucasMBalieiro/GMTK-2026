using UnityEditor.UI;
using UnityEngine;

namespace Actors.Player
{
    public class Player: Entity
    {

        [SerializeField] private Skill defendSkill;
        [SerializeField] private Skill attackSkill;
        [SerializeField] private Skill reloadSkill;
        [SerializeField] private Skill healSkill;
        
        public override void Defend()
        {
            Orchestrator.Instance.AddSkill(this, defendSkill);
        }
        
    }
}