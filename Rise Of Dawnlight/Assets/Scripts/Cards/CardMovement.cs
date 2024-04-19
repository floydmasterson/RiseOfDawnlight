using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler,  IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	private RectTransform rectTransform;
	private Canvas canvas;
	private Vector2 originalLoaclPointerPosition;
	private Vector3 originalPanelLocalPosition;
	private Vector3 originalScale;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private int currentState = 0;
	private int originalChildIndex;
	private bool mouseOver;

	[SerializeField]
	private float selectScale = 1.1f;
	[SerializeField]
	private float lerpFactor;
	[SerializeField]
	private GameObject glow;
	[SerializeField]
	private GameObject selectedglow;

	public CardMasterControl.HandManager handManager;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		originalScale = rectTransform.localScale;
		originalPosition= rectTransform.localPosition;
		originalRotation = rectTransform.localRotation;
	}

	private void Update()
	{
		switch(currentState)
		{
			case 1:
				HandleHoverState();
				break;
			case 2:
				HandleDragState();
				if (!Input.GetMouseButton(0) && !mouseOver)
					TransitionToState0();
				break;
			
		}

		if(currentState == 1 || currentState == 2)
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
				rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, lerpFactor);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (currentState == 1)
		{
			currentState = 2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLoaclPointerPosition);
			originalPanelLocalPosition = rectTransform.localPosition;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(currentState == 0)
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
		if(currentState == 1 || currentState == 2)
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
	


	public void OnPointerClick(PointerEventData eventData)
	{
	
			if(handManager.selectedCard == null && handManager.selectedCard != gameObject)
			{
				selectedglow.SetActive(true);
				handManager.selectedCard = gameObject;
			}
			else
			{
				CardMovement oldCard = handManager.selectedCard.GetComponent<CardMovement>();
				oldCard.selectedglow.SetActive(false);
				oldCard.TransitionToState0();
				handManager.selectedCard = gameObject;
				selectedglow.SetActive(true);
		}
		
	}
}
