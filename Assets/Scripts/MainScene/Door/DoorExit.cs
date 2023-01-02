using UnityEngine;
using System.Collections;
using Chameleon;

public class DoorExit : OptionInspectable,IDoor{
	[Header("Ballooon")]
	[SerializeField] float offsetLookTargetZ;

	[Header("EndSequence")]
	[SerializeField] float offsetCameraFinal;
	[SerializeField] Collider cNormal;
	[SerializeField] Collider cFinal;
	[SerializeField] float durationLiftPlayer;
	[SerializeField] float distanceCamFinal;
	[SerializeField] float durationCamPanFinal;
	private bool bFinalSequence = false;

	protected override void Awake(){
		base.Awake();
		cNormal.enabled = true;
		cFinal.enabled = false;
	}
	public void reset(){
		throw new System.NotImplementedException();
	}
	void OnCollisionExit(Collision collision){
		//prevent player sliding after exit collision
		if(collision.collider.CompareTag(tagPlayer)){
			collision.rigidbody.velocity = Vector3.zero;}
	}
	protected override void activate(){
		base.activate();
		float zOffset = Vector3.Dot(
			PlayerController.Instance.transform.position-transform.position,
			transform.forward
		);
		SceneMainManager.Instance.CanvasBalloon.transform.position += 
			(zOffset>=0 ? offsetLookTargetZ : -offsetLookTargetZ) * transform.forward;
	}
	protected override void onOptionAction(){
		StartCoroutine(rfApproachedSequence());
	}
	private IEnumerator rfApproachedSequence(){
		PlayerController playerController = PlayerController.Instance;
		playerController.InputMode = eInputMode.Freeze;
		bFinalSequence = true;
		dlgFooter.TweenDlg.bReverse = true;
		dlgFooter.TweenDlg.Reset();
		Vector3 vForward = Vector3.Project(
			playerController.transform.position-transform.position,
			transform.forward
		).normalized;
		subitrPanCamera.reset(
			subitrPanCamera.End,
			subitrPanCamera.Start //switch place
		);
		subitrPanCamera.End.vCamTarget = tLookTarget.position;
		Vector3 eulerCamTarget = Quaternion.LookRotation(-vForward,Vector3.up).eulerAngles;
		subitrPanCamera.End.qCamTarget = Quaternion.Euler(
			eulerCamTarget.newX(MathfExtension.clamp(eulerCamTarget.x,rangeCamEulerX)));
		subitrPanCamera.End.camDistance = offsetCameraFinal;

		Vector3 vPosFinal = transform.position +
			Quaternion.FromToRotation(Vector3.forward,-vForward)*
				playerController.VOpenOffset	
		;
		yield return new ParallelEnumerator(this,
			dlgFooter.TweenDlg,
			subitrPanCamera
		);
		dlgFooter.close(true);

		/* To account for the case where player is already colliding with cNormal */
		cNormal.enabled = false;
		cNormal.enabled = true;
		yield return playerController.rfWalkToward(
			vPosFinal,PlayerController.eCutsceneMotion.Walk,true);

		yield return playerController.turnToward(-vForward);
		
		subitrPanCamera.reset(
			subitrPanCamera.End,
			subitrPanCamera.Start
		);
		subitrPanCamera.End.vCamTarget = subitrPanCamera.Start.vCamTarget;
		subitrPanCamera.End.qCamTarget = subitrPanCamera.Start.qCamTarget;
		subitrPanCamera.End.camDistance = distanceCamFinal;
		subitrPanCamera.Reset(durationCamPanFinal);
		playerController.finalTouch();
		yield return subitrPanCamera;
		
		SceneMainManager.Instance.onWin();
	}
	void OnCollisionEnter(Collision collision){
		if(bFinalSequence && collision.gameObject.CompareTag(tagPlayer)){
			//collision.rigidbody.velocity = Vector3.zero;
			/* This is workaround that prevents player from jerking after animation ends.
			I do not yet know the cause, but guess it has something to do with Unity's Physics. */
			if(cNormal.enabled){
				cNormal.enabled = false;
				PlayerController.Instance.GetComponent<Rigidbody>().isKinematic = true;
				StartCoroutine(rfLiftPlayer(collision.rigidbody,transform.localScale.y*2));
				//height of default cylinder is twice of its localScale.y	
			}
		}
	}
	private IEnumerator rfLiftPlayer(Rigidbody rbPlayer,float deltaLift){
		Vector3 vPosPlayer = PlayerController.Instance.transform.position;
		float time = 0.0f;
		float yStart = rbPlayer.position.y;
		while(time < durationLiftPlayer){
			yield return null;
			rbPlayer.position = rbPlayer.position.newY(yStart+time*deltaLift/durationLiftPlayer);
			time += Time.deltaTime;
		}
		rbPlayer.position = rbPlayer.position.newY(yStart+deltaLift);
	}
}
