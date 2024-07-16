using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CombatLog;

public class ComabtManager : MonoBehaviour
{
	public GameMaster gameMaster;
	public EnemyHex enemyHex;

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
	[SerializeField, TabGroup("Fighters")]
	private GameObject spawnedEnemy;


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

	[SerializeField, TabGroup("Post Fight")]
	private GameObject postScreenText;
	[SerializeField, TabGroup("Post Fight")]
	private TMP_Text winText;
	[SerializeField, TabGroup("Post Fight")]
	private TMP_Text countDown;

	private bool timerGo = false;
	private float remainingTime;

	private void OnEnable()
	{
		playButton.onClick.AddListener(delegate { player.cardContorl.Play(); });
		drawButton.onClick.AddListener(delegate { player.cardContorl.DrawToHand(); });
	}

	private void OnDisable()
	{
		playButton.onClick.RemoveAllListeners(); 
		drawButton.onClick.RemoveAllListeners();
	}

	public IEnumerator PostFightScreen(string winner)
	{
		actionLeft = 0;
		winText.text = $"{winner} has won!";
		postScreenText.SetActive(true);
		timerGo = true;
		yield return new WaitForSeconds(4);
		enemyHex.fightTrigger.enabled = false;
		player.PostFightReset();
		postScreenText.SetActive(false);
		Destroy(enemyHex.SpawnedEnemy);
		Destroy(spawnedEnemy);
		timerGo = false;
		combatLog.ClearLog();
		gameMaster.TranistionOutOfFight();
	}

	public void BeginFight(EnemyHex reciveEnemyHex)
	{
		enemyHex = reciveEnemyHex;
		GameObject recivedEnemy = enemyHex.selcetedEnemy.GetComponent<EnemyData>().tableEnemy;
		GameObject createdEnemy = Instantiate(recivedEnemy, enemySpawnLocation);
		spawnedEnemy = createdEnemy;
		enemy = createdEnemy.GetComponent<EnemyManager>();
		actionLeft = actionLimit;
		roundCount++;
		enemy.SetBaseStats();
		enemy.cardContorl.combatManager = this;
		enemy.cardContorl.StartUp();
		player.cardContorl.StartUp();
		SetStats();
		UpdateText();
	}

	private void Update()
	{
		if (timerGo == true)
		{
			if (remainingTime == 0)
				remainingTime = 4;
			if (remainingTime > 0)
			{
				remainingTime -= Time.deltaTime;
			}
			else if (remainingTime < 0)
			{
				remainingTime = 0;
			}
			int minutes = Mathf.FloorToInt(remainingTime / 60);
			int seconds = Mathf.FloorToInt(remainingTime % 60);
			countDown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}
		if (actionLeft == 0)
		{
			passHighLight.SetActive(true);
		}
		else if (actionLeft > 0)
		{
			passHighLight.SetActive(false);
		}
		DeathCheck();
	}
	private void EnemyTurn()
	{
		actionLeft = actionLimit;
		CheckStatusEffects(enemy);

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
		CheckStatusEffects(player);
		DrawForTurn(player);
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

	private void CheckStatusEffects(EntityManager entity)
	{

		if (entity.currentEffects != null)
		{
			List<StatusEffectSO> effectsToRemove = new List<StatusEffectSO>();
			foreach (StatusEffectSO statusEffect in entity.currentEffects)
			{
				if (statusEffect.duration > 0)
				{
					statusEffect.StatusEffect();
					if (statusEffect.LogEntry() != string.Empty)
						UpdateLog(extraText: statusEffect.LogEntry());
				}
				else
				{
					effectsToRemove.Add(statusEffect);
				}
			}
			if (effectsToRemove.Count > 0)
			{
				foreach (StatusEffectSO effectToBeRemoved in effectsToRemove)
				{
					entity.RemoveStatus(effectToBeRemoved);
				}
			}
		}
		entity.healthBar.UpdateHealth(entity.CurrentHealth, entity.MaxHealth);
	}

	private void DrawForTurn(EntityManager entity)
	{
		if (entity is PlayerManager)
		{
			entity.cardContorl.handManager.DrawToHand(2, true);
			entity.cardContorl.UpdateHand();

		}
		else if (entity is EntityManager)
		{
			entity.cardContorl.enemyHandManager.DrawToHand(2);
			entity.cardContorl.UpdateHand();
		}
	}

	private void DeathCheck()
	{
		if (player.CurrentHealth <= 0)
		{
			StartCoroutine(PostFightScreen(enemy.enityName));
		}
		else if (enemy.CurrentHealth <= 0)
		{
			StartCoroutine(PostFightScreen(player.enityName));
		}
	}
}

