using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityManager : MonoBehaviour
{
	[TabGroup("Setup")]
	public string enityName;
	[TabGroup("Setup")]
	public HealthBar healthBar;
	[TabGroup("Setup")]
	public CardMasterControl cardContorl;
	[TabGroup("Health")]

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

	public List<StatusEffectSO> currentEffects = new List<StatusEffectSO>();

	public abstract void SetBaseStats();
	public abstract void TakeDamage(int damage);
	public abstract void Heal(int healing);
	public abstract void ApplyStatus(StatusEffectSO statusEffect);
	public abstract void RemoveStatus(StatusEffectSO statusEffect);
}
