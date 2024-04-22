using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CardMasterControl : MonoBehaviour
{
	public class DeckManager
	{

		public List<CardSO> deck = new List<CardSO>();
		private int currentIndex = 0;
		public void ComabtStart()
		{
			ListRandomizer.Shuffle(deck);
		}
		public CardSO DrawCard()
		{
			if (deck.Count == 0)
			{
				return null;
			}

			CardSO nextCard = deck[currentIndex];
			deck.Remove(nextCard);
			currentIndex = (currentIndex + 1) % deck.Count;
			return nextCard;
		}
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

	public class HandManager
	{

		private CardMasterControl cardMasterControl;
		private GameObject cardPrefab;
		private DeckManager deckManager;
		private Transform handTransfrom;
		private float fanSpread = 6.5f;
		private float cardSpaceing = -100f;
		private float verticalSpaceing = 100f;
		private int startingHandSize = 5;
		private int maxHandSize = 7;
		public GameObject selectedCard;


		public List<GameObject> cardsInHand = new List<GameObject>();
		private ComabtManager _comabtManager;

		public void Setup(CardMasterControl _cardMasterControl, GameObject _cardPrefab, DeckManager _deckManager, Transform _handTransform, float _fanSpread, float _cardSpacing, float _verticalSpacing, int _startingHandSize, int _maxHandSize)
		{
			cardMasterControl = _cardMasterControl;
			cardPrefab = _cardPrefab;
			deckManager = _deckManager;
			handTransfrom = _handTransform;
			fanSpread = _fanSpread;
			cardSpaceing = _cardSpacing;
			verticalSpaceing = _verticalSpacing;
			startingHandSize = _startingHandSize;
			maxHandSize = _maxHandSize;
		}
		public void CombatBegin(ComabtManager comabtManager)
		{
			_comabtManager = comabtManager;
			DrawToHand(startingHandSize, true);

		}
		[Button, PropertyOrder(0)]
		public void DrawToHand(int drawAmount, bool forTurn)
		{
			for (int i = 0; i < drawAmount; i++)
			{
				if (cardsInHand.Count <= maxHandSize && _comabtManager.actionLeft > 0 && deckManager.deck.Count > 0)
				{
					CardSO cardData = deckManager.DrawCard();
					GameObject newCard = Instantiate(cardPrefab, handTransfrom.position, Quaternion.identity, handTransfrom);
					newCard.name = cardData.name;
					cardsInHand.Add(newCard);
					CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
					cardDisplay.cardData = cardData;
					CardMovement cardMovment = newCard.GetComponent<CardMovement>();
					cardMovment.handManager = cardMasterControl.handManager;
					cardDisplay.UpdateCardDisplay();

					UpdateHand();

				}
			}
			if (!forTurn)
				_comabtManager.actionLeft--;
		}

		public void ActionDraw()
		{
			DrawToHand(2, false);
			_comabtManager.UpdateText();
		}

		public void Play()
		{
			CardSO playedCard = selectedCard.GetComponent<CardDisplay>().cardData;
			if (_comabtManager.playerCardPlayed(playedCard))
			{
				cardsInHand.Remove(selectedCard);
				Destroy(selectedCard);
			}
		}


		private void UpdateHand()
		{
			int cardCount = cardsInHand.Count;

			if (cardCount == 1)
			{
				cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
				return;
			}
			for (int i = 0; i < cardCount; i++)
			{
				float rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
				cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

				float horizontalOffset = (cardSpaceing * (i - (cardCount - 1) / 2f));

				float normalizePositon = (2f * i / (cardCount - 1) - 1f);

				float verticalOffset = (verticalSpaceing * (1 - normalizePositon * normalizePositon));
				cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
			}
		}


	}

	public class EnemyHandManager
	{

		public DeckManager deckManager;
		[SerializeField, TabGroup("Hand Limits")]
		private int startingHandSize = 5;
		[SerializeField, TabGroup("Hand Limits")]
		private int maxHandSize = 7;
		[PropertyOrder(1)]
		public List<CardSO> cardsInHand = new List<CardSO>();
		private ComabtManager _comabtManager;

		public void Setup(DeckManager _deckManager, int _startingHandSize, int _maxHandSize)
		{
			deckManager = _deckManager;
			startingHandSize = _startingHandSize;
			maxHandSize = _maxHandSize;
		}

		public void CombatBegin(ComabtManager comabtManager)
		{
			_comabtManager = comabtManager;
			DrawToHand(startingHandSize);

		}
		public void DrawToHand(int drawAmount)
		{
			for (int i = 0; i < drawAmount; i++)
			{
				if (cardsInHand.Count <= maxHandSize && deckManager.deck.Count > 0)
				{
					CardSO cardData = deckManager.DrawCard();
					cardsInHand.Add(cardData);
				}
			}
		}


		public CardSO PickCard()
		{
			foreach (CardSO card in cardsInHand)
			{
				if (_comabtManager.enemy.MaxHealth > _comabtManager.enemy.MaxHealth / 2)
				{
					if (card.cardType == CardSO.CardType.Potion && card.restoreType == CardSO.RestoreType.Health)
					{
						return card;
					}
				}
				if (card.attackType != CardSO.AttackType.None)
				{
					return card;
				}
			}
			return null;

		}

	}


	public HandManager handManager;
	public DeckManager deckManager;
	public EnemyHandManager enemyHandManager;
	public ComabtManager combatManager;
	public List<CardSO> deck = new List<CardSO>();
	[SerializeField, TitleGroup("Enemy")]
	private bool isEnemy = false;
	[TitleGroup("Enemy"), HideIf("@isEnemy != true")]
	public EnemyStatSO enemy;

	[TitleGroup("Hand Manager"), HideIf("@isEnemy == true")]
	public List<GameObject> playerCardsInHand = new List<GameObject>();
	[TitleGroup("Hand Manager"), HideIf("@isEnemy != true")]
	public List<CardSO> enemyCardsInHand = new List<CardSO>();
	[TitleGroup("Hand Manager")]
	[HorizontalGroup("Hand Manager/Split")]
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Object Setup"), HideIf("@isEnemy == true")]
	private GameObject cardPrefab;
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Object Setup"), HideIf("@isEnemy == true")]
	private Transform handTransfrom;

	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Card Spacing"), HideIf("@isEnemy == true")]
	private float fanSpread = 6.5f;
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Card Spacing"), HideIf("@isEnemy == true")]
	private float cardSpaceing = -100f;
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Card Spacing"), HideIf("@isEnemy == true")]
	private float verticalSpaceing = 100f;
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Hand Limits")]
	private int startingHandSize = 5;
	[SerializeField, TabGroup("Hand Manager/Split/Paramaters", "Hand Limits")]
	private int maxHandSize = 7;




	private void Awake()
	{
		deckManager = new DeckManager();
		if (isEnemy)
			enemyHandManager = new EnemyHandManager();
		else
			handManager = new HandManager();
	}
	public void StartUp()
	{
		if (!isEnemy)
		{
			handManager.Setup(this, cardPrefab, deckManager, handTransfrom, fanSpread, cardSpaceing, verticalSpaceing, startingHandSize, maxHandSize);
			deckManager.deck = deck;
			deckManager.ComabtStart();
			handManager.CombatBegin(combatManager);
			UpdateHand();

		}
		else if (isEnemy)
		{
			enemyHandManager.Setup(deckManager, startingHandSize, maxHandSize);
			
			deck = enemy.CopyDeck();
			deckManager.deck = deck;
			deckManager.ComabtStart();
			enemyHandManager.CombatBegin(combatManager);
			UpdateHand();
		}
	}

	public void Play()
	{
		handManager.Play();
	}

	public void DrawToHand()
	{
		handManager.DrawToHand(2, false);
		combatManager.UpdateText();
		UpdateHand();
	}

	public void UpdateHand()
	{
		if(handManager != null && playerCardsInHand != handManager.cardsInHand && isEnemy == false)
		{
			playerCardsInHand = handManager.cardsInHand;
		}
		else if( enemyHandManager != null && enemyCardsInHand != enemyHandManager.cardsInHand && isEnemy == true)
		{
			enemyCardsInHand = enemyHandManager.cardsInHand;
		}
	}
}
