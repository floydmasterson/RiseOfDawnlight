using UnityEngine;

[CreateAssetMenu(fileName = "New Knocked Down Effect", menuName = "Status Effects/Knocked Down")]
public class KnockedDownStatus : StatusEffectSO
{
	public ComabtManager comabtManager;

	public override void ApplyEffect(EntityManager entity)
	{
		entity.healthBar.ToggleStatus(effectNumber, true);
		entity.ApplyStatus(GetCopy(entity));
	}

	public override StatusEffectSO GetCopy(EntityManager entity)
	{
		KnockedDownStatus knockedDownStatus = Instantiate(this);
		knockedDownStatus.target = entity;
		knockedDownStatus.comabtManager = FindObjectOfType<ComabtManager>();
		knockedDownStatus.entityName = knockedDownStatus.target.enityName;
		return knockedDownStatus;
	}

	public override string LogEntry()
	{
		return $"{entityName} is knocked down and loses an action.";
	}

	public override void StatusEffect()
	{
		if (duration > 0)
		{
			comabtManager.actionLeft--;
			duration--;
			target.healthBar.ToggleStatus(effectNumber, false);
		}
	}
}
