using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card Data")]
public class CardSO : ScriptableObject
{
	[PreviewField(60), HideLabel]
	[TableColumnWidth(300, resizable: false)]
	[HorizontalGroup("Item", 60)]
	public Sprite cardArt;
	[VerticalGroup("Item/Right"), LabelWidth(65)]
	public string cardName;
	[VerticalGroup("Item/Right"), LabelWidth(65)]
	public string description;
	[VerticalGroup("Item/Right"), LabelWidth(65), ShowIf("@attackType != AttackType.None")]
	public int damage = 0;
	[VerticalGroup("Item/Right"), LabelWidth(65), ShowIf("@attackType == AttackType.None")]
	public int restoreValue = 0;
	[VerticalGroup("Item/Right"), LabelWidth(65), ShowIf("@attackType == AttackType.Magic")]
	public int manaCost = 0;
	[VerticalGroup("Item/Right"), LabelWidth(65), ShowIf("@attackType == AttackType.Melee")]
	public int staminaCost = 0;
	[VerticalGroup("Item/Right"), LabelWidth(65), ShowIf("@cardType != CardType.BaseMagic || cardType != CardType.BaseMelee")]
	public int cost = 1;
	public StatusEffectSO statusEffect;
	[EnumToggleButtons]
	public CardType cardType;
	[EnumToggleButtons, ShowIf("@cardType != CardType.Potion || cardType != CardType.Artifact")]
	public AttackType attackType;
	[EnumToggleButtons, ShowIf("@cardType == CardType.Potion")]
	public RestoreType restoreType;

	public enum CardType
	{
		BaseMagic,
		BaseMelee,
		BaseMagicFocus,
		BaseMeleeFocus,	
		Magic,
		MagicFocus,
		Melee,
		MeleeFocus,
		Item,
		Potion,
		Artifact,
	}
	
	public enum AttackType
	{
		None,
		Melee,
		Magic,
	}

	public enum RestoreType
	{
		Health,
		Mana,
		Stamina,
	}

}
