using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatLog : MonoBehaviour
{
	public class TextData
	{
		public string extraText;
		public string user;
		public string defender;
		public CardSO cardData;

		public void SetData(string _user = "", string _defender = "", CardSO _cardData = null, string _extraText = "")
		{
			user = _user;
			defender = _defender;
			cardData = _cardData;
			extraText = _extraText;
		}
	}
	public class TextLine
	{
		public string text;

		public enum TextType
		{
			Basic,
			Potion,
			Attack,
		}
		public TextType textType;

		public void TextSelect(TextData data)
		{
			switch (textType)
			{
				case TextType.Basic:
					CreateBasicText(data);
					break;
				case TextType.Potion:
					CreatePotionText(data);
					break;
				case TextType.Attack:
					CreateAttackerText(data);
					break;
			}
		}
		public string CreateBasicText(TextData data)
		{
			string createdText = data.extraText;
			text = createdText;
			return text;
		}

		public string CreatePotionText(TextData data)
		{
			string createdText = $"{data.user} used a {data.cardData.name} and restored {data.cardData.restoreValue} {data.cardData.restoreType}";
			text = createdText;
			return text;
		}
		public string CreateAttackerText(TextData data)
		{
			string createdText = $"{data.user} played {data.cardData.name} and did {data.cardData.damage} damage to {data.defender}";
			text = createdText;
			return text;
		}
	}


	[SerializeField] private GameObject logPrefab;
	[SerializeField] private GameObject contentView;
	[SerializeField] private GameObject logView;
	[SerializeField] private GameObject toggleButton;

	private bool visable = true;


	public void UpdateLog(TextLine textLog)
	{
		GameObject log = Instantiate(logPrefab);
		log.GetComponentInChildren<TextMeshProUGUI>().text = textLog.text;
		log.transform.SetParent(contentView.transform);
	}

	public TextLine CreateLog(TextLine.TextType type, TextData data)
	{
		TextLine textLine = new TextLine();
		textLine.textType = type;
		textLine.TextSelect(data);
		return textLine;
	}

	public TextData CreateTextData(string user, string defender, CardSO cardData, string extraText = "")
	{
		TextData data = new TextData();
		data.SetData(user, defender, cardData, extraText);
		return data;
	}

	public void ToggleLog()
	{
		if(visable == true)
		{
			logView.SetActive(false);
			toggleButton.SetActive(true);
			visable = false;
		}
		else if( visable == false)
		{
			logView.SetActive(true);
			toggleButton.SetActive(false);
			visable = true;
		}
	}
}
