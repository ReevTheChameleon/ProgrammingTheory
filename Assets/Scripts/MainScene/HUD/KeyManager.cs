using UnityEngine;
using Chameleon;
using UnityEngine.UI;
using TMPro;
using System.Collections;
//using UnityEngine.InputSystem; //for test, remove later

public class KeyManager : LoneMonoBehaviour<KeyManager>{
	[SerializeField] Image imgKeyIcon;
	[SerializeField][GrayOnPlay] TextMeshProUGUI txtCount;
	[SerializeField][GrayOnPlay] float scaleFontBump;
	[SerializeField][GrayOnPlay] float durationFontBump;
	private LoneCoroutine routineChangeKeyCount;
	
	public int KeyCount{get; private set;} = 0;
	public Vector2 VKeyIconScreenPos{ get{return imgKeyIcon.transform.position;} }

	protected override void Awake(){
		base.Awake();
		routineChangeKeyCount = new LoneCoroutine(
			this,
			txtCount.sinBumpFontSize(scaleFontBump,durationFontBump)
		);
	}
	public void addKey(){
		gameObject.SetActive(true);
		txtCount.text = "x"+ ++KeyCount;
		routineChangeKeyCount.restart();
		routineChangeKeyCount.resume();
	}
	public bool removeKey(){
		if(KeyCount <= 0)
			return false;
		txtCount.text = "x"+ --KeyCount;
		routineChangeKeyCount.restart();
		routineChangeKeyCount.resume();
		return true;
	}
	//void Update(){
	//	if(Keyboard.current.upArrowKey.wasPressedThisFrame)
	//		addKey();
	//	if(Keyboard.current.downArrowKey.wasPressedThisFrame)
	//		removeKey();
	//}
}
