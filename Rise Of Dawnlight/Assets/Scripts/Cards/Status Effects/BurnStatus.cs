using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "New Burn Effect", menuName = "Status Effects/Burn")]
public class BurnStatus : StatusEffectSO
{
	private int damage;

	public override void ApplyEffect(EntityManager entity)
	{
		entity.healthBar.ToggleStatus(effectNumber, true);
		entity.ApplyStatus(GetCopy(entity));
	}

	public override StatusEffectSO GetCopy(EntityManager entity)
	{
		BurnStatus burnEffect = Instantiate(this);
		burnEffect.target = entity;
		burnEffect.entityName = burnEffect.target.enityName;
		return burnEffect;
	}

	public override string LogEntry()
	{
		if (damage > 0)
			return $"Fire burns {entityName} for {damage} damage.";
		else
			return string.Empty;
	}

	public override void StatusEffect()
	{
		if (duration > 0)
		{
			target.TakeDamage(BurnDamage());
			duration--;
		}
		else
		{
			target.healthBar.ToggleStatus(effectNumber, false);
		}
	}

	private int BurnDamage()
	{
		int chance = Random.Range(1, 3);
		if (chance >= 2)
		{
			damage = Random.Range(2, 4);
		}
		else
		{
			damage = 0;
		}
		return damage;
	}
}
