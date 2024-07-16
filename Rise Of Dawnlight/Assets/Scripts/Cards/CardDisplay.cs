using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Sirenix.OdinInspector;

public class CardDisplay : MonoBehaviour
{
	public CardSO cardData;
	[TabGroup("Text"), SerializeField, Required]
	private TMP_Text nameText;
	[TabGroup("Text"), SerializeField, Required]
	private TMP_Text descriptionText;
	[TabGroup("Text"), SerializeField, Required]
	private TMP_Text damageText;
	[TabGroup("Text"), SerializeField, Required]
	private TMP_Text energycostText;
	[TabGroup("Text"), SerializeField, Required]
	private TMP_Text costText;
	[TabGroup("Graphics"), SerializeField, Required]
	private Image cardIamge;
	[TabGroup("Graphics"), SerializeField, Required]
	private GameObject[] cardBackground;
	
	
	public void UpdateCardDisplay()
	{
		cardBackground[0].SetActive(false);
		nameText.text = cardData.name;
		if(cardData.cardArt != null)
			cardIamge.sprite = cardData.cardArt;
		descriptionText.text = cardData.description;
		damageText.text = cardData.damage.ToString();
		switch(cardData.cardType)
		{
			case CardSO.CardType.Blank:
				cardBackground[0].SetActive(true);
				break;
			case CardSO.CardType.BaseMagic:
				cardBackground[1].SetActive(true);
				break;
			case CardSO.CardType.BaseMelee:
				cardBackground[2].SetActive(true);
				break;
			case CardSO.CardType.BaseMagicFocus:
				cardBackground[3].SetActive(true);
				break;
			case CardSO.CardType.BaseMeleeFocus:
				cardBackground[4].SetActive(true);
				break;
			case CardSO.CardType.Magic:
				cardBackground[3].SetActive(true);
				break;
			case CardSO.CardType.MagicFocus:
				cardBackground[4].SetActive(true);
				break;
			case CardSO.CardType.Melee:
				cardBackground[5].SetActive(true);
				break;
			case CardSO.CardType.MeleeFocus:
				cardBackground[6].SetActive(true);
				break;
			case CardSO.CardType.Item:
				cardBackground[7].SetActive(true);
				break;
			case CardSO.CardType.Potion:
				cardBackground[8].SetActive(true);
				break;
			case CardSO.CardType.Artifact:
				cardBackground[9].SetActive(true);
				break;
				
		}

		if(cardData.attackType == CardSO.AttackType.Melee)
			energycostText.text = cardData.staminaCost.ToString();
		else if(cardData.attackType == CardSO.AttackType.Magic)
			energycostText.text = cardData.manaCost.ToString();
		else
		{
			damageText.text = "";
			energycostText.text = "";
		}

		if (cardData.cardType == CardSO.CardType.BaseMagic || cardData.cardType == CardSO.CardType.BaseMagicFocus || cardData.cardType == CardSO.CardType.BaseMagic || cardData.cardType == CardSO.CardType.BaseMeleeFocus)
			costText.text = "";
		else
			costText.text = cardData.cost.ToString();

	}
}
