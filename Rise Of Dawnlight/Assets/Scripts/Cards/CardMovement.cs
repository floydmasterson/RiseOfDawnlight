using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
	private RectTransform rectTransform;
	private Canvas canvas;
#pragma warning disable IDE0052 // Remove unread private members
	private Vector2 originalLoaclPointerPosition;
	private Vector3 originalPanelLocalPosition;
#pragma warning restore IDE0052 // Remove unread private members
	private Vector3 originalScale;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	 private int currentState = 0;
	private bool mouseOver;

	[SerializeField]
	private float selectScale = 1.1f;
	[SerializeField]
	private float dragLerpFactor;
	[SerializeField]
	private float playLerpFactor;
	[SerializeField]
	private GameObject glow;
	[SerializeField]
	private GameObject selectedglow;
	[SerializeField]
	private Vector2 cardPlay;
	[SerializeField]
	private Vector3 playPosition;
	[SerializeField]
	private GameObject playArrow;
	private ArcRenderer arcRenderer;


	public CardMasterControl.HandManager handManager;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		originalScale = rectTransform.localScale;
		originalPosition = rectTransform.localPosition;
		originalRotation = rectTransform.localRotation;
		arcRenderer = playArrow.GetComponent<ArcRenderer>();
	}

	private void Update()
	{
		switch (currentState)
		{
			case 1:
				HandleHoverState();
				break;
			case 2:
				HandleDragState();
				if (!Input.GetMouseButton(0) && !mouseOver)
					TransitionToState0();
				break;
			case 3:
				HandlePlayState();
				break;


		}

		if (currentState == 1 || currentState == 2)
		{
			if (Input.GetMouseButton(1))
				rectTransform.localScale = originalScale * 2.5f;
			if (!Input.GetMouseButton(1))
				rectTransform.localScale = originalScale;
		}
	}


	public void TransitionToState0()
	{
		currentState = 0;
		glow.SetActive(false);
		playArrow.SetActive(false);
		rectTransform.localScale = originalScale;
		rectTransform.localPosition = originalPosition;
		rectTransform.localRotation = originalRotation;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (currentState == 2)
		{
			Vector2 localPointerPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPointerPosition))
				rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, dragLerpFactor);
			if (rectTransform.localPosition.y > cardPlay.y)
			{
				currentState = 3;
				playArrow.SetActive(true);
				rectTransform.position = playPosition;
			}
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		if (currentState == 3 && arcRenderer.onEnmey == true && handManager.selectedCard != null)
		{
				handManager.Play();
				currentState = 2;
				playArrow.SetActive(false);	
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (currentState == 1)
		{
			currentState = 2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLoaclPointerPosition);
			originalPanelLocalPosition = rectTransform.localPosition;
			if (handManager.selectedCard == null && handManager.selectedCard != gameObject)
			{
				handManager.selectedCard = gameObject;
			}
			else
			{
				CardMovement oldCard = handManager.selectedCard.GetComponent<CardMovement>();
				oldCard.TransitionToState0();
				handManager.selectedCard = gameObject;
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (currentState == 0)
		{
			mouseOver = true;
			originalPosition = rectTransform.localPosition;
			originalRotation = rectTransform.localRotation;
			originalScale = rectTransform.localScale;
			currentState = 1;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (currentState == 1 || currentState == 2)
		{
			mouseOver = false;
			TransitionToState0();
		}
	}

	private void HandleDragState()
	{
		rectTransform.localRotation = Quaternion.identity;
	}

	private void HandleHoverState()
	{
		glow.SetActive(true);
		rectTransform.localScale = originalScale * selectScale;
	}
	private void HandlePlayState()
	{
		rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, playPosition, playLerpFactor);
		rectTransform.localRotation = Quaternion.identity;


		if (Input.mousePosition.y < cardPlay.y)
		{
			currentState = 2;
			playArrow.SetActive(false);
		}
	}

}
