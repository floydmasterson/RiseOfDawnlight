using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "New Burn Effect", menuName = "Status Effects/Burn")]
public class BurnStatus : StatusEffectSO
{
	public EntityManager target;
	private int damage;
	[SerializeField] private int duration;
	[SerializeField, EnumToggleButtons] private HealthBar.StatusEffects effect;

	public override void ApplyEffect(EntityManager entity)
	{
		entity.gameObject.GetComponentInChildren<HealthBar>().ToggleStatus(effect, true);
		entity.ApplyStatus(GetCopy(entity));
	}

	public override StatusEffectSO GetCopy(EntityManager entity)
	{
		BurnStatus burnEffect = Instantiate(this);
		burnEffect.target = entity;
		return burnEffect;
	}

	public override string LogEntry()
	{
		if (damage > 0)
			return $"Fire burns {target.gameObject.name} for {damage} damage.";
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
			target.gameObject.GetComponentInChildren<HealthBar>().ToggleStatus(effect, false);
			target.RemoveStatus(this);
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
