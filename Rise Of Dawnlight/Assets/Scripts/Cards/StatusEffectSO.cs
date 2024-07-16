using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


public abstract class StatusEffectSO : ScriptableObject
{
	public enum StatusEffects
	{
		Bleed,
		Cursed,
		Burn,
		Dazed,
		Knocked_Down,
		Poisoned,
	}
	[SerializeField, EnumToggleButtons]
	public StatusEffects effectNumber;
	public int duration;
	[Title("Runtime Variables")]
	public EntityManager target;
	public string entityName;
	public abstract void StatusEffect();
    public abstract void ApplyEffect(EntityManager entity);
    public abstract StatusEffectSO GetCopy(EntityManager entity);
    public abstract string LogEntry();
}
