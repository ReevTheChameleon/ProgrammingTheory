using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Chameleon;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HpBarController : MonoBehaviour{
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

	public float Fraction{get; private set;} = 1.0f;
	public void addHp(float amount){
		transitionTo(Mathf.Clamp01(Fraction+amount));
	}
	public void addHpImmediate(float amount){
		setImmediate(Mathf.Clamp01(Fraction+amount));
	}
	void Awake(){
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
	void Update(){
		//if(Keyboard.current.spaceKey.wasPressedThisFrame)
		//	transitionTo(0.5f);
		//if(Keyboard.current.rKey.wasPressedThisFrame)
		//	transitionTo(1.0f);
		if(Keyboard.current.downArrowKey.wasPressedThisFrame)
			addHp(-0.1f);
		if(Keyboard.current.upArrowKey.wasPressedThisFrame)
			addHp(0.1f);
	}
}
