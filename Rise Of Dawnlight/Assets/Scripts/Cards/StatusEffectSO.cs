using System.Collections;
using UnityEngine;


public abstract class StatusEffectSO : ScriptableObject
{
    public abstract void StatusEffect();
    public abstract void ApplyEffect(EntityManager entity);
    public abstract StatusEffectSO GetCopy(EntityManager entity);
    public abstract string LogEntry();
}
