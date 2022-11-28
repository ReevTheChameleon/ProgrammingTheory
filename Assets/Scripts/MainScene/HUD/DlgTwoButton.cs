using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Chameleon;

[RequireComponent(typeof(RectTransform))]
public class DlgTwoButton : MonoBehaviour{
	[SerializeField] TextMeshProUGUI txtMessage;
	[SerializeField] TextMeshProUGUI txtBtn1;
	[SerializeField] TextMeshProUGUI txtBtn2;
	[SerializeField] ButtonHandler handlerBtn1;
	[SerializeField] ButtonHandler handlerBtn2;
	[SerializeField] ButtonHandler handlerBtnClose;

	public void setButtonAction(Action action1,Action action2,Action actionClose){
		handlerBtn1.setOnClickAction(action1);
		handlerBtn2.setOnClickAction(action2);
		handlerBtnClose.setOnClickAction(actionClose);
	}
	public void popup(){
		gameObject.SetActive(true);
		PlayerController.Instance.ShowCursor = true;
	}
	public void popup(Vector2 v2AnchoredPosition,string textMessage,
		string textBtn1,string textBtn2,
		Action action1,Action action2,Action actionClose)
	{
		txtMessage.text = textMessage;
		txtBtn1.text = textBtn1;
		txtBtn2.text = textBtn2;
		((RectTransform)transform).anchoredPosition = v2AnchoredPosition;
		setButtonAction(action1,action2,actionClose);
		popup();
	}
	public void close(bool bClearButtonAction=false){
		if(bClearButtonAction){ //prevent memory leak
			handlerBtn1.setOnClickAction(null);
			handlerBtn2.setOnClickAction(null);
			handlerBtnClose.setOnClickAction(null);
		}
		gameObject.SetActive(false);
		PlayerController.Instance.ShowCursor = false;
	}
}
