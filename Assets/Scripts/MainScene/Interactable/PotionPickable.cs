using UnityEngine;
using Chameleon;
using System.Collections;

public class PotionPickable : PickableInspectable{
	[SerializeField] float healAmount; //1.0f=full
	Renderer[] aRenderer;
	LoneCoroutine routineTweenIconPick = new LoneCoroutine();

	protected override void Awake(){
		base.Awake();
		aRenderer = GetComponentsInChildren<Renderer>();
	}
	void OnEnable(){
		for(int i=0; i<aRenderer.Length; ++i)
			aRenderer[i].enabled = true;
	}
	public override void onPicked(){
		base.onPicked();
		routineTweenIconPick.start(this,rfPickSequence());
	}
	private IEnumerator rfPickSequence(){
		for(int i=0; i<aRenderer.Length; ++i){
			aRenderer[i].enabled = false;}
		yield return HpBarController.Instance.tweenIconHealPick(transform.position);
		PlayerController.Instance.healPlayer(healAmount);
	}
}
