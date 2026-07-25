using System;
using System.Collections.Generic;
using System.Linq;
using AudioSystem;
using Entities;
using UnityEngine;

public struct StartupTickEvent : IEvent 
{
    public int ticksRemaining;
}

public class Orchestrator : MonoBehaviour
{
    [SerializeField] private int tempoTicks;
    [SerializeField] private int startupTickAmount = 2;
    

    [SerializeField] private SoundData tickSound;
    [SerializeField] private SoundData noPlayerInputSound;

    private int startupBufferTicks;
    private int currentTick = 0;

    private readonly Dictionary<Entity, Skill> skillPool = new Dictionary<Entity, Skill>();

    private EventBinding<AddSkillEvent> skillBinding;
    private EventBinding<Tick> tickBinding;

    private void OnEnable()
    {
        startupBufferTicks = tempoTicks * startupTickAmount;
        
        tickBinding = new EventBinding<Tick>(HandleTicking); 
        EventBus<Tick>.Register(tickBinding);
        
        skillBinding = new EventBinding<AddSkillEvent>(AddSkill);
        EventBus<AddSkillEvent>.Register(skillBinding);
    }

    private void OnDisable()
    {
        EventBus<Tick>.Deregister(tickBinding);
        
        EventBus<AddSkillEvent>.Deregister(skillBinding);
        skillBinding =  null;
    }

    private void AddSkill(AddSkillEvent eventData)
    {
        skillPool[eventData.caster] = eventData.skill;
    }
    
    private void HandleTicking()
    {
        
        if (startupBufferTicks > 0)
        {
            startupBufferTicks--;

            SoundManager.Instance.CreateSound().Play(startupBufferTicks % tempoTicks == 0 ? noPlayerInputSound : tickSound);

            EventBus<StartupTickEvent>.Raise(new StartupTickEvent { ticksRemaining = startupBufferTicks });
            return;
        }
        
        currentTick++;
        
        if (currentTick >= tempoTicks)
        {
            SoundManager.Instance.CreateSound().Play(noPlayerInputSound);
            HandleSkillOrder();
            currentTick = 0;
        }
        else
        {
            SoundManager.Instance.CreateSound().Play(tickSound);
        }
    }

    private void HandleSkillOrder()
    {
        var sortedSkills = skillPool
            .OrderBy(kvp => kvp.Value.type)
            .ThenByDescending(kvp => kvp.Key.IsPlayer) 
            .ToList();

        foreach (var (entity, skill) in sortedSkills)
        {
            if (entity.IsDead) continue;
            ExecuteSkill(entity, skill);
        }
        
        EventBus<ResetConditions>.Raise(new ResetConditions());
        skillPool.Clear();
        EventBus<RequestNextActionEvent>.Raise(new RequestNextActionEvent());
    }
    
    private static void ExecuteSkill(Entity caster, Skill skill)
    {
        switch (skill.type)
        {
            case SkillType.Defend:
                skill.target.Defend();
                break;
            case SkillType.Attack:
                caster.ConsumeAmmo(1); 
                skill.target.TakeDamage(skill.value);
                break;
            case SkillType.Reload:
                skill.target.Reload(skill.value);
                break;
            case SkillType.Heal:
                skill.target.Heal(skill.value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
}
