using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CombatLog;

public class ComabtManager : MonoBehaviour
{
	[SerializeField, TabGroup("Stats")]
	private int playerHealth = 5;
	[SerializeField, TabGroup("Stats")]
	private int playerMana = 5;
	[SerializeField, TabGroup("Stats")]
	private int playerStamina = 5;
	[SerializeField, TabGroup("Stats")]
	public int enemyHealth = 5;


	[SerializeField, TabGroup("Stats")]
	private int playerHealthMax = 5;
	[SerializeField, TabGroup("Stats")]
	private int playerManaMax = 5;
	[SerializeField, TabGroup("Stats")]
	private int playerStaminaMax = 5;
	[SerializeField, TabGroup("Stats")]
	public int enemyHealthMax = 5;


	[SerializeField, TabGroup("Text")]
	private TMP_Text playerHealthText;
	[SerializeField, TabGroup("Text")]
	private TMP_Text playerManaText;
	[SerializeField, TabGroup("Text")]
	private TMP_Text playerStaminaText;
	[SerializeField, TabGroup("Text")]
	private TMP_Text enemyHealthText;
	[SerializeField, TabGroup("Text")]
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

	[SerializeField] private EnemyStatSO enemy;

	public CardMasterControl playerCardContorl;
	public CardMasterControl enemyCardContorl;
	[SerializeField] private CombatLog combatLog;

	[SerializeField] HealthBar playerBar;
	[SerializeField] HealthBar enemyBar;
	private void Awake()
	{
		actionLeft = actionLimit;
		roundCount++;
		enemyHealth = enemy.maxHealth;
		enemyHealthMax = enemy.maxHealth;
		enemyBar.SetBar(enemyHealthMax);
		playerBar.SetBar(playerHealthMax);
		enemyCardContorl.enemy = enemy;
		playerCardContorl.StartUp();
		enemyCardContorl.StartUp();
		
		UpdateText();
	}

	private void Update()
	{
		if(actionLeft == 0) 
		{
			passHighLight.SetActive(true);
		}
		else if(actionLeft > 0)
		{
			passHighLight.SetActive(false);
		}
	
	}
	private void EnemyTurn()
	{

		actionLeft = actionLimit;
		enemyCardContorl.enemyHandManager.DrawToHand(2);
		enemyCardContorl.UpdateHand();
		while(actionLeft > 0)
		{

			CardSO cardToPlay = enemyCardContorl.enemyHandManager.PickCard();
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
		UpdateLog(extraText: $"{enemy.name} Passed");
		NewRound();
	}
	private void NewRound()
	{
		roundCount++;
		actionLeft = actionLimit;
		playerMana += 1;
		playerStamina += 1;
		playerCardContorl.handManager.DrawToHand(2,true);
		playerCardContorl.UpdateHand();
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
					playerHealth += card.restoreValue;
					actionLeft--;
					UpdateText();
					UpdateLog("Willow", TextLine.TextType.Potion, card);
					playerBar.UpdateHealth(playerHealth, playerHealthMax);
					return true;
				case CardSO.RestoreType.Stamina:
					playerStamina += card.restoreValue;
					actionLeft--;
					UpdateText();
					UpdateLog("Willow", TextLine.TextType.Potion, card);
					
					return true;
				case CardSO.RestoreType.Mana:
					playerMana += card.restoreValue;
					actionLeft--;
					UpdateText();
					UpdateLog("Willow", TextLine.TextType.Potion, card);
					return true;
			}
		}
		if(card.cardType == CardSO.CardType.Item)
		{
			return true;
		}
		if (card.attackType == CardSO.AttackType.Magic && playerMana >= card.manaCost)
		{
			playerMana -= card.manaCost;
			enemyHealth -= card.damage;
			actionLeft--;
			UpdateText();
			UpdateLog("Willow", TextLine.TextType.Attack, card, defender:enemy.name);
			enemyBar.UpdateHealth(enemyHealth, enemyHealthMax);
			return true;
		}
		else if (card.attackType == CardSO.AttackType.Melee && playerStamina >= card.staminaCost)
		{
			playerStamina -= card.staminaCost;
			enemyHealth -= card.damage;
			actionLeft--;
			UpdateText();
			UpdateLog("Willow", TextLine.TextType.Attack, card, defender: enemy.name);
			enemyBar.UpdateHealth(enemyHealth, enemyHealthMax);
			return true;
		}
		Debug.Log($"{card.name} is unplayable");
		return false;

	}
	public bool enemyCardPlayed(CardSO card)
	{
		if (actionLeft <= 0)
			return false;

		if (card.cardType == CardSO.CardType.Potion)
		{
			switch (card.restoreType)
			{
				case CardSO.RestoreType.Health:
					enemyHealth += card.restoreValue;
					actionLeft--;
					UpdateText();
					UpdateLog(enemy.name, TextLine.TextType.Potion, card);
					enemyBar.UpdateHealth(enemyHealth, enemyHealthMax);
					return true;
			}
		}
		if (card.cardType == CardSO.CardType.Item)
		{
			return true;
		}
		if (card.attackType == CardSO.AttackType.Magic)
		{
			playerHealth -= card.damage;
			actionLeft--;
			UpdateText();
			UpdateLog(enemy.name, TextLine.TextType.Attack, card, defender: "Willow");
			playerBar.UpdateHealth(playerHealth, playerHealthMax);
			return true;
		}
		else if (card.attackType == CardSO.AttackType.Melee)
		{
			playerHealth -= card.damage;
			actionLeft--;
			UpdateText();
			UpdateLog(enemy.name, TextLine.TextType.Attack, card, defender: "Willow");
			playerBar.UpdateHealth(playerHealth, playerHealthMax);
			return true;
		}
		Debug.Log($"{card.name} is unplayable");
		return false;

	}
	public void UpdateText()
	{
		playerHealthText.text = $"Player Health:{playerHealth} / {playerHealthMax}";
		playerManaText.text = $"Player Mana:{playerMana} / {playerManaMax}";
		playerStaminaText.text = $"Player Stamina:{playerStamina} / {playerStaminaMax}";
		enemyHealthText.text = $"Enemy Health:{enemyHealth} / {enemyHealthMax}";
		actionsText.text = $"Actions Left: {actionLeft}";
		roundText.text = $"Round {roundCount}";
	}
	public void UpdateLog(string user = "", TextLine.TextType type = 0, CardSO cardData = null, string extraText = "", string defender = "")
	{
		TextData textData = combatLog.CreateTextData(user, defender, cardData, extraText);
		TextLine textLine = combatLog.CreateLog(type, textData);
		combatLog.UpdateLog(textLine);
	}
}

