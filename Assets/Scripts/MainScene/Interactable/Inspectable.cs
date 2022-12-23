using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Chameleon;

public class Inspectable : Interactable{
	[SerializeField][TextArea] protected List<string> lText = new List<string>();

	//Pan Camera Routine
	[Header("Camera Pan")]
	[SerializeField] protected float durationPan;
	[SerializeField][WideVector2] protected Vector2 rangeCamEulerX;
	[SerializeField] protected float targetDistanceCamera;

	protected LoneCoroutine routineInteract = new LoneCoroutine();
	protected ThirdPersonCameraControl cameraControl;
	protected Transform tCamTarget;
	
	protected class CamInfo{
		public Vector3 vCamTarget;
		public Quaternion qCamTarget;
		public float camDistance;
	}
	protected TweenRoutineUnit<CamInfo> subitrPanCamera;

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
		subitrPanCamera = new TweenRoutineUnit<CamInfo>(
			(CamInfo camInfoStart,CamInfo camInfoEnd,float t) => {
				float tDamped = Mathf.SmoothStep(0.0f,1.0f,t);
				tCamTarget.position =
					Vector3.Lerp(camInfoStart.vCamTarget,camInfoEnd.vCamTarget,tDamped);
				tCamTarget.localRotation =
					Quaternion.Lerp(camInfoStart.qCamTarget,camInfoEnd.qCamTarget,tDamped);
				cameraControl.targetCameraDistance =
					Mathf.Lerp(camInfoStart.camDistance,camInfoEnd.camDistance,tDamped);
			},
			new CamInfo(),
			new CamInfo(),
			durationPan
		);
	}
	protected IEnumerator rfStartInspectSequence(){
		inspectionState = eInspectionState.StartSequence;
		PlayerController playerController = PlayerController.Instance;
		playerController.InputMode = eInputMode.Interacting;
		playerController.turnToward(transform);

		FollowConstraint camTargetContraint = tCamTarget.GetComponent<FollowConstraint>();
		camTargetContraint.enabled = false;
		bHideBalloon = true;
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);

		subitrPanCamera.Start.vCamTarget = tCamTarget.position;
		subitrPanCamera.End.vCamTarget = tLookTarget.position;
		subitrPanCamera.Start.qCamTarget = tCamTarget.localRotation;
		Vector3 eulerCamTarget = tCamTarget.localEulerAngles;
		Quaternion qCamTargetEnd = Quaternion.Euler(eulerCamTarget.newX(
			MathfExtension.clamp(Mathf.DeltaAngle(0.0f,eulerCamTarget.x),rangeCamEulerX))
		);
		subitrPanCamera.End.qCamTarget = qCamTargetEnd;
		subitrPanCamera.Start.camDistance = cameraControl.targetCameraDistance;
		subitrPanCamera.End.camDistance = targetDistanceCamera;
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
		FollowConstraint camTargetConstraint = tCamTarget.GetComponent<FollowConstraint>();
		subitrPanCamera.Start.vCamTarget = camTargetConstraint.TargetPosition;
		subitrPanCamera.bReverse = true;
		subitrPanCamera.Reset();
		yield return subitrPanCamera;
		
		bHideBalloon = false;
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(true);
		camTargetConstraint.enabled = true;
		PlayerController.Instance.InputMode = eInputMode.MainGameplay;
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
