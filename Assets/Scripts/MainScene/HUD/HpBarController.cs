using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Chameleon;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HpBarController : LoneMonoBehaviour<HpBarController>{
	[SerializeField] RectTransform rtBack;
	[SerializeField] Image imgMid;
	[SerializeField] Image imgFront;
	[SerializeField] Image imgCircle;
	[SerializeField] Color colorBarDamage;
	[SerializeField] Color colorBarHeal;
	[SerializeField] Color colorCircleDamage;
	[SerializeField] Color colorCircleHeal;
	[SerializeField][GrayOnPlay] float durationSuspend;
	[SerializeField] float durationTransition;
	private LoneCoroutine routineTransitionHp = new LoneCoroutine();
	private enum eComboStatus{None,Reducing,Increasing}
	private eComboStatus comboStatus = eComboStatus.None;
	private WaitForSeconds waitSuspend;
	private Color colorBackground;

	[SerializeField] Image imgHealIconPick;
	[SerializeField] float durationTweenIconHealPick;
	private LoneCoroutine routineTweenIconHealPick;
	private TweenRoutineUnit<Vector3> subitrTweenIconHealPick;

	void Start(){
		subitrTweenIconHealPick = imgHealIconPick.transform.tweenPosition(
			Vector3.zero,
			imgCircle.transform.position,
			durationTweenIconHealPick,
			dOnDone: (float t) => {imgHealIconPick.gameObject.SetActive(false);}
		);
		routineTweenIconHealPick = new LoneCoroutine(this,subitrTweenIconHealPick);
	}
	public float Fraction{get; private set;} = 1.0f;
	public void addHp(float amount){
		transitionTo(Mathf.Clamp01(Fraction+amount));
		/* This happens because I found out later that 1.0f-0.2f five times is > 0.0f,
		and even >Mathf.Epsilon. At first I thought this was taken care of by Unity, but... */
		if(Fraction <= 0.001f){
			SceneMainManager.Instance.onDie();}
	}
	public void addHpImmediate(float amount){
		setImmediate(Mathf.Clamp01(Fraction+amount));
	}
	protected override void Awake(){
		waitSuspend = new WaitForSeconds(durationSuspend);
		colorBackground = imgCircle.color;
	}
	private void setImmediate(float target){
		routineTransitionHp.stop();
		Fraction = target;
		imgMid.rectTransform.setWidth(target*rtBack.rect.width);
		imgFront.rectTransform.setWidth(target*rtBack.rect.width);
	}
	private void transitionTo(float target){
		if(target == Fraction){
			comboStatus = eComboStatus.None;
			return;
		}
		if(routineTransitionHp.IsRunning)
			routineTransitionHp.stop();
		RectTransform rt;
		if(target < Fraction){ //damage
			imgFront.rectTransform.setWidth(target*rtBack.rect.width);
			imgMid.color = colorBarDamage;
			imgCircle.color = colorCircleDamage;
			if(comboStatus != eComboStatus.Reducing)
				imgMid.rectTransform.setWidth(Fraction*rtBack.rect.width);
			comboStatus = eComboStatus.Reducing;
			rt = imgMid.rectTransform;
		}
		else{ //heal
			imgMid.rectTransform.setWidth(target*rtBack.rect.width);
			imgMid.color = colorBarHeal;
			imgCircle.color = colorCircleHeal.newA(imgCircle.color.a);
			if(comboStatus != eComboStatus.Increasing)
				imgFront.rectTransform.setWidth(Fraction*rtBack.rect.width);
			comboStatus = eComboStatus.Increasing;
			rt = imgFront.rectTransform;
		}
		Fraction = target;
		routineTransitionHp.start(this,rfTransitionHp(rt,target*rtBack.rect.width));
	}
	private IEnumerator rfTransitionHp(RectTransform rt,float widthEnd){
		float widthStart = rt.rect.width;
		yield return waitSuspend;

		comboStatus = eComboStatus.None;
		imgCircle.color = colorBackground;
		float t = 0.0f;
		while(t < 1.0f){
			rt.setWidth(Mathf.Lerp(widthStart,widthEnd,t));
			t += Time.deltaTime/durationTransition;
			yield return null;
		}
		rt.setWidth(widthEnd);
	}
	public WaitLoneCoroutine tweenIconHealPick(Vector3 vWorldPos){
		subitrTweenIconHealPick.reset(
			Camera.main.WorldToScreenPoint(vWorldPos).newZ(0.0f),
			subitrTweenIconHealPick.End
		);
		imgHealIconPick.gameObject.SetActive(true);
		return routineTweenIconHealPick.resume();
	}
	//void Update(){
	//	//if(Keyboard.current.spaceKey.wasPressedThisFrame)
	//	//	transitionTo(0.5f);
	//	//if(Keyboard.current.rKey.wasPressedThisFrame)
	//	//	transitionTo(1.0f);
	//	if(Keyboard.current.downArrowKey.wasPressedThisFrame)
	//		addHp(-0.1f);
	//	if(Keyboard.current.upArrowKey.wasPressedThisFrame)
	//		addHp(0.1f);
	//}
}
