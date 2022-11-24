using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Chameleon;

public class DlgTwoButton : MonoBehaviour{
	[SerializeField] TextMeshProUGUI txtMessage;
	[SerializeField] TextMeshProUGUI txtBtn1;
	[SerializeField] TextMeshProUGUI txtBtn2;
	[SerializeField] ButtonHandler handlerBtn1;
	[SerializeField] ButtonHandler handlerBtn2;
	[SerializeField] ButtonHandler handlerBtnClose;

	public void popup(Vector2 v2Position,string textMessage,string textBtn1,string textBtn2,
		Action action1,Action action2,Action actionClose)
	{
		txtMessage.text = textMessage;
		txtBtn1.text = textBtn1;
		txtBtn2.text = textBtn2;
		handlerBtn1.setOnClickAction(action1);
		handlerBtn2.setOnClickAction(action2);
		handlerBtnClose.setOnClickAction(actionClose);
		transform.position = v2Position;
	}
	void Start(){
		popup(transform.position,"Hello World!","OK","Cancel",
			()=>{gameObject.SetActive(false); },
			()=>{gameObject.SetActive(false); },
			()=>{gameObject.SetActive(false); }
		);
	}
}
