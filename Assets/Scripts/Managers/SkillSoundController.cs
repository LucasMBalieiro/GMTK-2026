using System;
using System.Collections.Generic;
using System.Linq;
using AudioSystem;
using Entities;
using UnityEngine;

public enum SoundEffectType
{
    Attack,
    Defend,
    Heal,
    Reload,
    AttackBlocked,
    HealInterrupted
}

[Serializable]
public struct SoundEffectEntry
{
    public SoundEffectType Type;
    public SoundData Sound;
}

public class SkillSoundController : MonoBehaviour
{
    [Tooltip("Um SoundData por tipo de efeito sonoro. Arraste os ScriptableObjects aqui.")]
    [SerializeField] private List<SoundEffectEntry> soundEffects = new List<SoundEffectEntry>();

    private EventBinding<ActionExecutedEvent> actionBinding;
    private EventBinding<AttackBlockedEvent> attackBlockedBinding;
    private EventBinding<HealInterruptedEvent> healInterruptedBinding;

    private void OnEnable()
    {
        actionBinding = new EventBinding<ActionExecutedEvent>(HandleActionExecuted);
        EventBus<ActionExecutedEvent>.Register(actionBinding);

        attackBlockedBinding = new EventBinding<AttackBlockedEvent>(HandleAttackBlocked);
        EventBus<AttackBlockedEvent>.Register(attackBlockedBinding);

        healInterruptedBinding = new EventBinding<HealInterruptedEvent>(HandleHealInterrupted);
        EventBus<HealInterruptedEvent>.Register(healInterruptedBinding);
    }

    private void OnDisable()
    {
        EventBus<ActionExecutedEvent>.Deregister(actionBinding);
        actionBinding = null;

        EventBus<AttackBlockedEvent>.Deregister(attackBlockedBinding);
        attackBlockedBinding = null;

        EventBus<HealInterruptedEvent>.Deregister(healInterruptedBinding);
        healInterruptedBinding = null;
    }

    private void HandleActionExecuted(ActionExecutedEvent eventData)
    {
        if (eventData.Caster == null || !eventData.Caster.IsPlayer) return;

        var soundType = ToSoundEffectType(eventData.SkillType);
        if (soundType.HasValue)
            PlaySound(soundType.Value);
    }

    private void HandleAttackBlocked(AttackBlockedEvent eventData)
    {
        if (eventData.Defender == null || !eventData.Defender.IsPlayer) return;
        PlaySound(SoundEffectType.AttackBlocked);
    }

    private void HandleHealInterrupted(HealInterruptedEvent eventData)
    {
        if (eventData.Target == null || !eventData.Target.IsPlayer) return;
        PlaySound(SoundEffectType.HealInterrupted);
    }

    private static SoundEffectType? ToSoundEffectType(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Attack: return SoundEffectType.Attack;
            case SkillType.Defend: return SoundEffectType.Defend;
            case SkillType.Heal: return SoundEffectType.Heal;
            case SkillType.Reload: return SoundEffectType.Reload;
            default: return null;
        }
    }

    /// <summary>
    /// Toca o som configurado para o tipo de efeito. Pode ser chamado de fora
    /// (ex: quando Entity avisar que um ataque foi bloqueado ou uma cura interrompida).
    /// </summary>
    public void PlaySound(SoundEffectType type)
    {
        var entry = soundEffects.FirstOrDefault(s => s.Type == type);
        if (entry.Sound == null)
        {
            Debug.LogWarning($"Nenhum SoundData configurado para {type}.");
            return;
        }

        SoundManager.Instance.CreateSound().Play(entry.Sound);
    }
}