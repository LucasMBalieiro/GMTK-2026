using Entities;

public interface IEvent { }

public struct AddSkillEvent : IEvent 
{
    public Entity caster;
    public Skill skill;
}

public struct RequestNextActionEvent : IEvent { }
public struct ResetConditions : IEvent { }

public struct Tick : IEvent { }
public struct PlayMetronome: IEvent { }
public struct PauseMetronome: IEvent { }