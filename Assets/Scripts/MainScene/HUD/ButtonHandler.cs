using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Chameleon;

[RequireComponent(typeof(Image))]
public class ButtonHandler : MonoBehaviour,
	IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler,
	IPointerDownHandler,IPointerUpHandler
{
	[SerializeField] Color colorNormal;
	[SerializeField] Color colorHover;
	[SerializeField] Color colorClick;
	private Action dOnClick;
	private bool bDown = false;
	Image image;

	void Awake(){
		image = GetComponent<Image>();
	}
	public void setOnClickAction(Action action){
		dOnClick = action;
	}
	public void OnPointerClick(PointerEventData eventData){
		dOnClick?.Invoke();
	}
	public void OnPointerEnter(PointerEventData eventData){
		image.color = bDown ? colorClick : colorHover;
	}
	public void OnPointerExit(PointerEventData eventData){
		image.color = colorNormal;
	}
	public void OnPointerDown(PointerEventData eventData){
		image.color = colorClick;
		bDown = true;
	}
	public void OnPointerUp(PointerEventData eventData){
		image.color = colorNormal;
		bDown = false;
	}
}
