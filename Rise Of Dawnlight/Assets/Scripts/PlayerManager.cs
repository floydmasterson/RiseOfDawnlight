using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : EntityManager
{
	public enum Class
	{
		Mage,
		Warrior,
		Rouge,
		Paladin,
	}
	[EnumToggleButtons]
	public Class selectedClass;
	public CardMasterControl cardContorl;
	public List<StatusEffectSO> currentEffects = new List<StatusEffectSO>();
	public HealthBar healthBar;

	#region Stats
	[TabGroup("Health")]
	public bool isAlive = true;
	private int _maxHealth;
	private int _maxMana;
	private int _maxStamina;
	[SerializeField, TabGroup("Health"), ProgressBar(0, "MaxHealth", 1, 0, 0)]
	private int _currentHealth;
	[SerializeField, TabGroup("Mana"), ProgressBar(0, "MaxMana", 0, 0, 1)]
	private int _currentMana;
	[SerializeField, TabGroup("Stamina"), ProgressBar(0, "MaxStamina", 0, 1, 0)]
	private int _currentStamina;

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

	[TabGroup("Mana")]
	public int CurrentMana
	{
		get { return _currentMana; }
		set
		{
			_currentMana = value;
			if (_currentMana > MaxMana)
			{
				_currentMana = MaxMana;
			}
			if (_currentMana < 0 && !isAlive)
			{
				_currentMana = 0;
			}
		}
	}
	[TabGroup("Mana")]
	public int MaxMana
	{
		get { return _maxMana; }
		set { _maxMana = value; }
	}

	[TabGroup("Stamina")]
	public int CurrentStamina
	{
		get { return _currentStamina; }
		set
		{
			_currentStamina = value;
			if (_currentStamina > MaxStamina)
			{
				_currentStamina = MaxStamina;
			}
			if (_currentStamina < 0 && !isAlive)
			{
				_currentStamina = 0;
			}
		}
	}
	[TabGroup("Stamina")]
	public int MaxStamina
	{
		get { return _maxStamina; }
		set { _maxStamina = value; }
	}
	#endregion

	private void Awake()
	{
		SetBaseStats();
	}

	public override void SetBaseStats()
	{
		switch (selectedClass)
		{
			case Class.Mage:
				_maxHealth = 8;
				_maxMana = 10;
				_maxStamina = 4;
				break;
			case Class.Warrior:
				_maxHealth = 12;
				_maxMana = 4;
				_maxStamina = 10;
				break;
			case Class.Rouge:
				_maxHealth = 8;
				_maxMana = 7;
				_maxStamina = 7;
				break;
			case Class.Paladin:
				_maxHealth = 10;
				_maxMana = 6;
				_maxStamina = 8;
				break;
		}
		CurrentHealth = MaxHealth;
		CurrentMana = MaxMana;
		CurrentStamina = MaxStamina;
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

	public void UpdateResource(int resourceGain, int resourceToRestore)
	{
		switch (resourceToRestore)
		{
			case 0:
				CurrentMana += resourceGain;
				break;
			case 1:
				CurrentStamina += resourceGain;
				break;
		}
	}

}
