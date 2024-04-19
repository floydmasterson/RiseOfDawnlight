using System.Collections;
using UnityEngine;


public abstract class StatusEffectSO : ScriptableObject
{
    public abstract void StatusEffect();
    public abstract void ApplyEffect(object enemy);
    public abstract StatusEffectSO GetCopy();
}
