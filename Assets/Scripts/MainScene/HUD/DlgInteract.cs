using UnityEngine;
using Chameleon;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class DlgInteract : DlgTwoButton{
	[SerializeField] float durationDlgTween;
	//protected TweenRoutineUnit subitrTweenDlg;
	public TweenRoutineUnit TweenDlg{get; private set;}

	void Start(){
		RectTransform rt = (RectTransform)transform;
		RectTransform rtHUDCanvas = (RectTransform)rt.root.GetComponent<Canvas>().transform;
		Vector2 v2DlgPosStart = new Vector2(
			rtHUDCanvas.rect.width/2 + rt.rect.width/2,
			-rtHUDCanvas.rect.height/2 + rt.rect.height + 5.0f
		);
		Vector2 v2DlgPosEnd = v2DlgPosStart;
		v2DlgPosEnd.x -= rt.rect.width;
		TweenDlg = rt.tweenAnchoredPosition(v2DlgPosStart,v2DlgPosEnd,durationDlgTween);
		//Assume resolution is fixed since game starts
	}
}
