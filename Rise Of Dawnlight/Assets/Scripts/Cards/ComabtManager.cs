using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CombatLog;

public class ComabtManager : MonoBehaviour
{
	public GameMaster gameMaster;

	[SerializeField, TabGroup("Turn")]
	private GameObject passHighLight;
	[SerializeField, TabGroup("Turn")]
	private int roundCount;
	[SerializeField, TabGroup("Turn")]
	private TMP_Text roundText;
	[SerializeField, TabGroup("Turn")]
	private int actionLimit = 3;
	[SerializeField, TabGroup("Turn"), ProgressBar(0, "actionLimit", 0, 1, 0, Segmented = true)]
	public int actionLeft;
	[SerializeField, TabGroup("Turn")]
	private TMP_Text actionsText;

	[TabGroup("Fighters")]
	public PlayerManager player;
	[TabGroup("Fighters")]
	public EnemyManager enemy;
	[SerializeField, TabGroup("Fighters")]
	private Transform enemySpawnLocation;


	[SerializeField, TabGroup("Fight Tracking")]
	private CombatLog combatLog;
	[SerializeField, TabGroup("Fight Tracking")]
	private MeterScript manaTrack;
	[SerializeField, TabGroup("Fight Tracking")]
	private MeterScript staminaTrack;

	[SerializeField, TabGroup("Turn")]
	private Button playButton;
	[SerializeField, TabGroup("Turn")]
	private Button drawButton;


	public void BeginFight(GameObject reciveEnemy)
	{
		GameObject createdEnemy = Instantiate(reciveEnemy, enemySpawnLocation);
		enemy = createdEnemy.GetComponent<EnemyManager>();
		actionLeft = actionLimit;
		roundCount++;
		playButton.onClick.AddListener(delegate { player.cardContorl.Play(); });
		drawButton.onClick.AddListener(delegate { player.cardContorl.DrawToHand(); });
		enemy.SetBaseStats();
		enemy.cardContorl.combatManager = this;
		enemy.cardContorl.StartUp();
		player.cardContorl.StartUp();
		SetStats();
		UpdateText();
	}

	private void Update()
	{
		if (actionLeft == 0)
		{
			passHighLight.SetActive(true);
		}
		else if (actionLeft > 0)
		{
			passHighLight.SetActive(false);
		}

	}
	private void EnemyTurn()
	{
		actionLeft = actionLimit;
		if (enemy.currentEffects != null)
		{
			foreach (StatusEffectSO statusEffect in enemy.currentEffects)
			{
				if (statusEffect.duration > 0)
				{
					statusEffect.StatusEffect();
					if (statusEffect.LogEntry() != string.Empty)
						UpdateLog(extraText: statusEffect.LogEntry());
				}
				else
				{
					enemy.RemoveStatus(statusEffect);
				}
			}
		}

		enemy.cardContorl.enemyHandManager.DrawToHand(2);
		enemy.cardContorl.UpdateHand();
		while (actionLeft > 0)
		{

			CardSO cardToPlay = enemy.cardContorl.enemyHandManager.PickCard();
			if (cardToPlay != null)
			{
				enemyCardPlayed(cardToPlay);
				UpdateText();
			}
			else
			{
				EnemyPass();
				break;
			}
		}

		EnemyPass();
	}
	public void Pass()
	{
		actionLeft = 0;
		UpdateLog(extraText: "Willow Passed");
		EnemyTurn();
	}
	private void EnemyPass()
	{
		actionLeft = 0;
		UpdateLog(extraText: $"{enemy.enityName} Passed");
		NewRound();
	}
	private void NewRound()
	{
		roundCount++;
		actionLeft = actionLimit;
		if (player.currentEffects != null)
		{
			foreach (StatusEffectSO statusEffect in player.currentEffects)
			{
				if (statusEffect.duration > 0)
				{
					statusEffect.StatusEffect();
					if (statusEffect.LogEntry() != string.Empty)
						UpdateLog(extraText: statusEffect.LogEntry());
				}
				else
				{
					player.RemoveStatus(statusEffect);
				}
			}
		}
		player.cardContorl.handManager.DrawToHand(2, true);
		player.cardContorl.UpdateHand();
		UpdateText();
	}
	public bool playerCardPlayed(CardSO card)
	{
		if (actionLeft <= 0)
			return false;

		if (card.cardType == CardSO.CardType.Potion)
		{
			switch (card.restoreType)
			{
				case CardSO.RestoreType.Health:
					player.Heal(card.restoreValue);
					actionLeft--;
					UpdateText();
					UpdateLog(player.enityName, TextLine.TextType.Potion, card);
					player.healthBar.UpdateHealth(player.CurrentHealth, player.MaxHealth);
					return true;
				case CardSO.RestoreType.Stamina:
					player.UpdateResource(card.restoreValue, 1);
					staminaTrack.SetValue(player.MaxStamina, player.MaxStamina);
					actionLeft--;
					UpdateText();
					UpdateLog(player.enityName, TextLine.TextType.Potion, card);

					return true;
				case CardSO.RestoreType.Mana:
					player.UpdateResource(card.restoreValue, 0);
					manaTrack.SetValue(player.CurrentMana, player.MaxMana);
					actionLeft--;
					UpdateText();
					UpdateLog(player.enityName, TextLine.TextType.Potion, card);
					return true;
			}
		}
		if (card.cardType == CardSO.CardType.Item)
		{
			return true;
		}
		if (card.attackType == CardSO.AttackType.Magic && player.CurrentMana >= card.manaCost)
		{
			player.UpdateResource(-card.manaCost, 0);
			manaTrack.SetValue(player.CurrentMana, player.MaxMana);
			enemy.TakeDamage(card.damage);
			if (card.statusEffect != null)
			{
				card.statusEffect.ApplyEffect(enemy);
			}
			actionLeft--;
			UpdateText();
			UpdateLog(player.enityName, TextLine.TextType.Attack, card, defender: enemy.enityName);
			enemy.healthBar.UpdateHealth(enemy.CurrentHealth, enemy.MaxHealth);
			return true;
		}
		else if (card.attackType == CardSO.AttackType.Melee && player.CurrentStamina >= card.staminaCost)
		{
			player.UpdateResource(-card.staminaCost, 1);
			staminaTrack.SetValue(player.CurrentStamina, player.MaxStamina);
			enemy.TakeDamage(card.damage);
			if (card.statusEffect != null)
			{
				card.statusEffect.ApplyEffect(enemy);
			}
			actionLeft--;
			UpdateText();
			UpdateLog(player.enityName, TextLine.TextType.Attack, card, defender: enemy.enityName);
			enemy.healthBar.UpdateHealth(enemy.CurrentHealth, enemy.MaxHealth);
			return true;
		}
		Debug.Log($"{card.name} is unplayable");
		return false;

	}
	private bool enemyCardPlayed(CardSO card)
	{
		if (actionLeft <= 0)
			return false;

		if (card.cardType == CardSO.CardType.Potion)
		{
			switch (card.restoreType)
			{
				case CardSO.RestoreType.Health:
					enemy.Heal(card.restoreValue);
					actionLeft--;
					UpdateText();
					UpdateLog(enemy.enityName, TextLine.TextType.Potion, card);
					enemy.healthBar.UpdateHealth(enemy.CurrentHealth, enemy.MaxHealth);
					return true;
			}
		}
		if (card.cardType == CardSO.CardType.Item)
		{
			return true;
		}
		if (card.attackType == CardSO.AttackType.Magic)
		{
			player.TakeDamage(card.damage);
			if (card.statusEffect != null)
			{
				card.statusEffect.ApplyEffect(player);
			}
			actionLeft--;
			UpdateText();
			UpdateLog(enemy.enityName, TextLine.TextType.Attack, card, defender: player.enityName);
			player.healthBar.UpdateHealth(player.CurrentHealth, player.MaxHealth);
			return true;
		}
		else if (card.attackType == CardSO.AttackType.Melee)
		{
			player.TakeDamage(card.damage);
			if (card.statusEffect != null)
			{
				card.statusEffect.ApplyEffect(player);
			}
			actionLeft--;
			UpdateText();
			UpdateLog(enemy.enityName, TextLine.TextType.Attack, card, defender: player.enityName);
			player.healthBar.UpdateHealth(player.CurrentHealth, player.MaxHealth);
			return true;
		}
		Debug.Log($"{card.name} is unplayable");
		return false;

	}
	public void UpdateText()
	{
		actionsText.text = $"Actions Left: {actionLeft}";
		roundText.text = $"Round {roundCount}";
	}
	private void UpdateLog(string user = "", TextLine.TextType type = 0, CardSO cardData = null, string extraText = "", string defender = "")
	{
		TextData textData = combatLog.CreateTextData(user, defender, cardData, extraText);
		TextLine textLine = combatLog.CreateLog(type, textData);
		combatLog.UpdateLog(textLine);
	}
	private void SetStats()
	{
		enemy.healthBar.SetBar(enemy.MaxHealth);
		player.healthBar.SetBar(player.MaxHealth);
		manaTrack.SetMaxValue(player.MaxMana);
		staminaTrack.SetMaxValue(player.MaxStamina);
	}

}

