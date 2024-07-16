using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : EntityManager
{
	#region Stats
	[TabGroup("Health")]
	private int _maxMana;
	private int _maxStamina;
	[SerializeField, TabGroup("Mana"), ProgressBar(0, "MaxMana", 0, 0, 1)]
	private int _currentMana;
	[SerializeField, TabGroup("Stamina"), ProgressBar(0, "MaxStamina", 0, 1, 0)]
	private int _currentStamina;

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
			if (_currentMana < 0)
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
			if (_currentStamina < 0)
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
	public enum Class
	{
		Mage,
		Warrior,
		Rouge,
		Paladin,
	}
	[TabGroup("Setup"), EnumToggleButtons]
	public Class selectedClass;

	private void Awake()
	{
		SetBaseStats();
	}

	public override void SetBaseStats()
	{
		switch (selectedClass)
		{
			case Class.Mage:
				MaxHealth = 8;
				_maxMana = 10;
				_maxStamina = 4;
				break;
			case Class.Warrior:
				MaxHealth = 12;
				_maxMana = 4;
				_maxStamina = 10;
				break;
			case Class.Rouge:
				MaxHealth = 8;
				_maxMana = 7;
				_maxStamina = 7;
				break;
			case Class.Paladin:
				MaxHealth = 10;
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

	public void PostFightReset()
	{
		CurrentHealth = MaxHealth;
		CurrentMana = MaxMana;
		CurrentStamina = MaxStamina;
		cardContorl.PostFightCleanUp();
		if(currentEffects.Count > 0)
		{
			foreach (StatusEffectSO effect in currentEffects)
			{
				healthBar.ToggleStatus(effect.effectNumber, false);
			}
			currentEffects.Clear();
		}
	}
}
