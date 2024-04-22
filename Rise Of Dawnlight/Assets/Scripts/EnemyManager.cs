using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : EntityManager
{
	[TabGroup("Health")]
	public bool isAlive = true;
	private int _maxHealth;
	[SerializeField, TabGroup("Health"), ProgressBar(0, "MaxHealth", 1, 0, 0)]
	private int _currentHealth;
	[TabGroup("Health")]
	public int CurrentHealth
	{
		get { return _currentHealth; }
		set
		{
			_currentHealth = value;
			if (_currentHealth > MaxHealth)
			{
				_currentHealth = MaxHealth;
			}
			if (_currentHealth < 0 && !isAlive)
			{
				_currentHealth = 0;
			}
		}
	}
	[TabGroup("Health")]
	public int MaxHealth
	{
		get { return _maxHealth; }
		set { _maxHealth = value; }
	}

	public EnemyStatSO enemyStats;
	public CardMasterControl cardContorl;
	public HealthBar healthBar;

	public List<StatusEffectSO> currentEffects = new List<StatusEffectSO>();


	private void Awake()
	{
		SetBaseStats();
	}
	public override void SetBaseStats()
	{
		if (enemyStats != null)
		{
			_maxHealth = enemyStats.maxHealth;
			CurrentHealth = MaxHealth;
			cardContorl.enemy = enemyStats;
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
		if (currentEffects.Contains(statusEffect) == false)
			currentEffects.Add(statusEffect);
	}
	public override void RemoveStatus(StatusEffectSO statusEffect)
	{
		currentEffects.Remove(statusEffect);
	}
}
