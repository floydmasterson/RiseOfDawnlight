using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatLog : MonoBehaviour
{
	// Classes for storing text data and determining text type
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

	// Serialized fields for inspector references
	[SerializeField] private GameObject logPrefab;
	[SerializeField] private GameObject contentView;
	[SerializeField] private ScrollRect scrollRect;
	[SerializeField] private GameObject toggleButton;
	[SerializeField] private GameObject logView;

	// Flag for log visibility
	private bool visible = true;

	// Method to update the combat log
	public void UpdateLog(TextLine textLog)
	{
		// Instantiate log prefab
		GameObject log = Instantiate(logPrefab);
		log.GetComponentInChildren<TextMeshProUGUI>().text = textLog.text;
		log.transform.SetParent(contentView.transform);

		ScrollLogToBottom();
	}

	// Method to create a log entry
	public TextLine CreateLog(TextLine.TextType type, TextData data)
	{
		TextLine textLine = new TextLine();
		textLine.textType = type;
		textLine.TextSelect(data);
		return textLine;
	}

	// Method to create text data for a log entry
	public TextData CreateTextData(string user, string defender, CardSO cardData, string extraText = "")
	{
		TextData data = new TextData();
		data.SetData(user, defender, cardData, extraText);
		return data;
	}

	// Method to toggle log visibility
	public void ToggleLog()
	{
		visible = !visible;
		logView.SetActive(visible);
		toggleButton.SetActive(!visible);
	}

	// Method to scroll the log to the bottom
	private void ScrollLogToBottom()
	{
		Canvas.ForceUpdateCanvases(); // Ensure canvas updates before scrolling
		scrollRect.normalizedPosition = new Vector2(0, 0); // Scroll to the bottom
	}

	public void ClearLog()
	{
		for (int i = 0; i < contentView.transform.childCount; i++)
		{
			Destroy(contentView.transform.GetChild(i).gameObject);
		}
	}
}
