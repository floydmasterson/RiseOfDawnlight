using UnityEngine;

[CreateAssetMenu(fileName = "New Posion Effect", menuName = "Status Effects/Posion")]
public class PosionStatus : StatusEffectSO
{
	private int damage;
	public override void ApplyEffect(EntityManager entity)
	{
		entity.healthBar.ToggleStatus(effectNumber, true);
		entity.ApplyStatus(GetCopy(entity));
	}

	public override StatusEffectSO GetCopy(EntityManager entity)
	{
		PosionStatus posionEffect = Instantiate(this);
		posionEffect.target = entity;
		posionEffect.entityName = posionEffect.target.enityName;
		return posionEffect;
	}

	public override string LogEntry()
	{
		return $"Posion fills {entityName} veins for {damage} damage.";
	}

	public override void StatusEffect()
	{
		if (duration > 0)
		{
			target.TakeDamage(PosionDamage());
			duration--;
		}
		else
		{
			target.healthBar.ToggleStatus(effectNumber, false);
		}
	}

	private int PosionDamage()
	{
		damage = Random.Range(1, 2);
		return damage;
	}
}
