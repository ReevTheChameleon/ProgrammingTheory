using UnityEngine;
using System.Collections;
using Chameleon;
using UnityEngine.UI;

public class PickableInspectable : OptionInspectable{
	protected MeshRenderer meshRenderer;

	[Header("Action Pick")]
	[SerializeField] float durationTweenIcon;
	//private TweenRoutineUnit subitrTweenIconPick;
	private LoneCoroutine routineTweenIconPick = new LoneCoroutine();
	protected Vector2 v2IconStartPos;
	protected Vector2 v2IconEndPos;

	protected override void Awake(){
		base.Awake();
		meshRenderer = GetComponentInChildren<MeshRenderer>();
	}
	void OnEnable(){
		meshRenderer.enabled = true;
	}
	//protected virtual void Start(){
	//	subitrTweenIconPick = imgIconPick.transform.tweenPosition(
	//		Vector3.zero,Vector3.zero,
	//		durationTweenIcon
	//		//(float t) => {
	//		//	imgIconPick.transform.position = Vector2.Lerp(v2IconStartPos,v2IconEndPos,t);},
	//	);
	//}
	protected override void onOptionAction(){
		routineInteract.start(this,rfActionSequence());
	}
	protected virtual IEnumerator rfActionSequence(){
		PlayerController playerController = PlayerController.Instance;
		dlgFooter.TweenDlg.bReverse = true;
		dlgFooter.TweenDlg.Reset();
		Vector3 vWalkTarget =
			transform.position.newY(playerController.transform.position.y)-
			playerController.transform.forward*playerController.DistancePickup
		;
		yield return new ParallelEnumerator(this,
			PlayerController.Instance.rfWalkToward(
				vWalkTarget,
				Vector3.Distance(
					transform.position.xz(),
					PlayerController.Instance.transform.position.xz()
				) < playerController.DistancePickup ?
					PlayerController.eCutsceneMotion.WalkBackward :
					PlayerController.eCutsceneMotion.Walk
			),
			dlgFooter.TweenDlg
		);
		dlgFooter.close(true);
		yield return PlayerController.Instance.rfPickup();
		yield return rfEndInspectSequence();
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
	public void onPicked(){
		routineTweenIconPick.start(this,rfPickIconSequence());
	}
	protected virtual IEnumerator rfPickIconSequence(){
		HeadLookController.Instance.setHeadLookTarget(null);
		meshRenderer.enabled = false;
		yield return KeyManager.Instance.tweenIconKeyPick(transform.position,true);

	//	//v2IconStartPos = Camera.main.WorldToScreenPoint(transform.position);
	//	///* Although z doesn't seem to matter for overlay Canvas (we assume icon is on such),
	//	//if it is outside some range, it will not show up, so better to play safe. */
	//	//imgIconPick.gameObject.SetActive(true);
	//	//subitrTweenIconPick.Reset();
	//	//yield return subitrTweenIconPick;
	//	//imgIconPick.gameObject.SetActive(false);
	}
}
