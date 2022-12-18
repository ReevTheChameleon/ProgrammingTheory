using UnityEngine;
using Chameleon;
using System.Collections;

public class CandlePickable : PickableInspectable{
	[SerializeField] float lightAmount;
	LoneCoroutine routineTweenIconPick = new LoneCoroutine();

	void OnEnable(){
		for(int i=0; i<transform.childCount; ++i){
			transform.GetChild(i).gameObject.SetActive(true);}
	}
	public override void onPicked(){
		base.onPicked();
		routineTweenIconPick.start(this,rfPickSequence());
	}
	private IEnumerator rfPickSequence(){
		for(int i=0; i<transform.childCount; ++i){
			transform.GetChild(i).gameObject.SetActive(false);}
		yield return CandleManager.Instance.tweenIconCandlePick(transform.position);
		CandleManager.Instance.addLight(lightAmount);
	}
}
