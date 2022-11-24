using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Chameleon;

public class Inspectable : Interactable{
	[SerializeField][TextArea] protected List<string> lText = new List<string>();

	//Pan Camera Routine
	[Header("Camera Pan")]
	[SerializeField] protected float durationPan;
	[SerializeField] protected float minCamEulerX;
	[SerializeField] protected float targetDistanceCamera;

	protected LoneCoroutine routineInteract = new LoneCoroutine();
	protected TweenRoutineUnit subitrPanCamera;
	protected ThirdPersonCameraControl cameraControl;
	protected Transform tCamTarget;
	protected Vector3 vCamTargetStart;
	protected Vector3 vCamTargetEnd;
	protected Quaternion qCamTargetStart;
	protected Quaternion qCamTargetEnd;
	protected float camDistanceStart;
	protected float camDistanceEnd;
	
	protected enum eInspectionState{
		None,
		StartSequence,
		FooterTexting,
		Suspended,
		EndSequence,
	}
	protected eInspectionState inspectionState = eInspectionState.None;

	protected override void Awake(){
		base.Awake();
		cameraControl = Camera.main.GetComponent<ThirdPersonCameraControl>();
		tCamTarget = cameraControl.tTarget;
		subitrPanCamera = new TweenRoutineUnit(
			(float t) => {
				float tDamped = Mathf.SmoothStep(0.0f,1.0f,t);
				tCamTarget.position = Vector3.Lerp(vCamTargetStart,vCamTargetEnd,tDamped);
				tCamTarget.localRotation = Quaternion.Lerp(qCamTargetStart,qCamTargetEnd,tDamped);
				cameraControl.targetCameraDistance = Mathf.Lerp(camDistanceStart,camDistanceEnd,tDamped);
			},
			durationPan
		);
	}
	protected IEnumerator rfStartInspectSequence(){
		inspectionState = eInspectionState.StartSequence;
		PlayerController playerController = PlayerController.Instance;
		playerController.setActiveInputMovement(false);
		playerController.turnToward(transform);

		FollowConstraint camTargetContraint = tCamTarget.GetComponent<FollowConstraint>();
		camTargetContraint.enabled = false;
		bHideBalloon = true;
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);

		vCamTargetStart = tCamTarget.position;
		vCamTargetEnd = transform.position;
		qCamTargetStart = tCamTarget.localRotation;
		Vector3 eulerCamTarget = tCamTarget.localEulerAngles;
		qCamTargetEnd = Quaternion.Euler(
			eulerCamTarget.newX(Mathf.Max(minCamEulerX,Mathf.DeltaAngle(0.0f,eulerCamTarget.x)))
		);
		camDistanceStart = cameraControl.targetCameraDistance;
		camDistanceEnd = targetDistanceCamera;
		subitrPanCamera.bReverse = false;
		subitrPanCamera.Reset();
		yield return subitrPanCamera;
		
		inspectionState = eInspectionState.FooterTexting;
		FooterManager footerManager = FooterManager.Instance;
		footerManager.showFooter(lText);
		while(!footerManager.IsDone)
			yield return null;

		//last passage does not wait for skip
		inspectionState = eInspectionState.Suspended;
	}
	protected IEnumerator rfEndInspectSequence(){
		inspectionState = eInspectionState.EndSequence;
		FooterManager.Instance.hideFooter();
		subitrPanCamera.bReverse = true;
		yield return subitrPanCamera;
		
		bHideBalloon = false;
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(true);
		tCamTarget.GetComponent<FollowConstraint>().enabled = true;
		PlayerController.Instance.setActiveInputMovement(true);
		inspectionState = eInspectionState.None;
	}
	public override void onInteracted(){
		switch(inspectionState){
			case eInspectionState.None:
				routineInteract.start(this,rfStartInspectSequence());
				break;
			case eInspectionState.FooterTexting:
				FooterManager.Instance.stepFooter();
				break;
			case eInspectionState.Suspended:
				routineInteract.start(this,rfEndInspectSequence());
				break;
		}
	}
}
