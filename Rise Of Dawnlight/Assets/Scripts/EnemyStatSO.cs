using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy Data")]
public class EnemyStatSO : ScriptableObject
{
	public int maxHealth;
	public List<CardSO> deck = new List<CardSO>();

	public List<CardSO> CopyDeck()
	{
		List<CardSO> deckCopy = new List<CardSO>();
		foreach (CardSO card in deck)
		{
			deckCopy.Add(card);
		}
		return deckCopy;
	}
	
}
