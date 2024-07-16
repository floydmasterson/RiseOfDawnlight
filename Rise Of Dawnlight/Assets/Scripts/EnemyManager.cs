using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : EntityManager
{
	[TabGroup("Setup")]
	public EnemyStatSO enemyStats;

	private void OnValidate()
	{
		currentEffects.Clear();
	}
	public override void SetBaseStats()
	{
		if (enemyStats != null)
		{
			MaxHealth = enemyStats.maxHealth;
			CurrentHealth = MaxHealth;
			cardContorl.enemy = enemyStats;
			currentEffects.Clear();
		}
		else
		{
			Debug.LogError($"{gameObject.name} is missing stat object");
		}
	}

	public override void TakeDamage(int damage)
	{
		CurrentHealth -= damage;
	}

	public override void Heal(int healing)
	{
		CurrentHealth += healing;
	}

	public override void ApplyStatus(StatusEffectSO statusEffect)
	{
		int foundCount = 0;
		foreach (StatusEffectSO effect in currentEffects)
		{
			if (effect.effectNumber == statusEffect.effectNumber)
				foundCount++;
		}
		if (foundCount == 0)
			currentEffects.Add(statusEffect);
	}
	public override void RemoveStatus(StatusEffectSO statusEffect)
	{
		currentEffects.Remove(statusEffect);
	}

}
