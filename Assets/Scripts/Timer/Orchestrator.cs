using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using UnityUtils;

public class Orchestrator : Singleton<Orchestrator>
{
    [SerializeField] private int tempoTicks;
    private Metronome metronome;
    private int currentTick = 0;

    private readonly Dictionary<Entity, Skill> skillPool = new Dictionary<Entity, Skill>();
    
    private void Start()
    {
        metronome = GetComponent<Metronome>();
    }

    private void OnEnable()
    {
        metronome.Tick += HandleTicking;
        metronome.Play();
    }

    private void OnDisable()
    {
        metronome.Tick -= HandleTicking;
        metronome.Pause();
    }

    private void HandleTicking()
    {
        currentTick++;
        
        if (currentTick >= tempoTicks)
        {
            HandleSkillOrder();
            currentTick = 0;
        }
        else
        {
            //TODO: ADD SOUND
        }
    }

    private void HandleSkillOrder()
    {
        var sortedSkills = skillPool.Values.OrderByDescending(s => s.type).ToList();
        
        foreach (var skill in sortedSkills)
        {
            ExecuteSkill(skill);
        }
        foreach (var entity in skillPool.Keys)
        {
            entity.ResetConditions();
        }
        
        skillPool.Clear();
    }

    private static void ExecuteSkill(Skill skill)
    {
        switch (skill.type)
        {
            case SkillType.Defend:
                skill.target.Defend();
                break;
            case SkillType.Attack:
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
    
    public void AddSkill(Entity caster, Skill skill)
    {
        skillPool[caster] = skill;
    }
}
