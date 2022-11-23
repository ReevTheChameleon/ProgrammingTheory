using UnityEngine;
using System;
using System.Collections;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DigitMessage : MessageInteractable{
	DigitAligner digitAligner;
	[Bakable] private const string sRichTextColorBull = "yellow";
	[Bakable] private const string sRichTextColorCow = "green";

	//Pan Camera Routine
	[Header("Camera Pan")]
	[SerializeField] float durationPan;
	[SerializeField] float minCamEulerX;
	[SerializeField] float targetDistanceCamera;
	private LoneCoroutine routineInteract = new LoneCoroutine();
	private TweenRoutineUnit subitrPanCamera;
	private FrameTrigger triggerSkip = new FrameTrigger();
	
	private ThirdPersonCameraControl cameraControl;
	private Transform tCamTarget;
	private Vector3 vCamTargetStart;
	private Vector3 vCamTargetEnd;
	private Quaternion qCamTargetStart;
	private Quaternion qCamTargetEnd;
	private float camDistanceStart;
	private float camDistanceEnd;
	
	protected override void Awake(){
		base.Awake();
		digitAligner = GetComponentInParent<DigitAligner>();
		cameraControl = Camera.main.GetComponent<ThirdPersonCameraControl>();
		tCamTarget = cameraControl.tTarget;
		subitrPanCamera = new TweenRoutineUnit(
			(float t) => {
				float tDamped = Mathf.SmoothStep(0.0f,1.0f,t);
				//Debug.Log(Vector3.Lerp(vCamTargetStart,vCamTargetEnd,1.0f).toPreciseString());
				tCamTarget.position = Vector3.Lerp(vCamTargetStart,vCamTargetEnd,tDamped);
				cameraControl.applySettingsImmediate();
				//Debug.Log(tCamTarget.position.toPreciseString()+" "+tDamped);
				//tCamTarget.localRotation = Quaternion.Lerp(qCamTargetStart,qCamTargetEnd,tDamped);
				//cameraControl.targetCameraDistance = Mathf.Lerp(camDistanceStart,camDistanceEnd,tDamped);
			},
			durationPan
		);
		//routineInteract = new LoneCoroutine(
		//	this,
		//	new TweenRoutineUnit(
		//		(float t) => {
		//			tCamTarget.position = Vector3.Lerp(vCamTargetStart,vCamTargetEnd,t);
		//			tCamTarget.rotation = Quaternion.Lerp(qCamTargetStart,qCamTargetEnd,t);
		//			cameraControl.targetCameraDistance = Mathf.Lerp(camDistanceStart,camDistanceEnd,t);
		//		},
		//		durationPan
		//	)
		//);
	}
	public void initText(int[] aDigit,eDigitType[] aDigitType){
		lText.Clear();
		if(aDigit?.Length<3 || aDigitType?.Length<3) //if null all comparison except != will be false 
			return;
		int bullCount = 0;
		int cowCount = 0;
		for(int i=0; i<3; ++i){
			switch(aDigitType[i]){
				case eDigitType.Bull: ++bullCount; break;
				case eDigitType.Cow: ++cowCount; break;
			}
		}
		if(bullCount==0 && cowCount==0)
			lText.Add("These numbers do not seem to mean anything...");
		else{
			eDigitType prevDigitType = eDigitType.Normal;
			bool bAlso = false;
			for(int i=0; i<3; ++i){
				switch(aDigitType[i]){
					case eDigitType.Bull:
						bAlso = prevDigitType==eDigitType.Bull;
						lText.Add("Number <color="+sRichTextColorBull+">"+
							aDigit[i]+"</color> " + (bAlso ? "also" : "") +
							" somehow feels strongly linked to the exit...");
						prevDigitType = aDigitType[i];
						break;
					case eDigitType.Cow:
						bAlso = prevDigitType==eDigitType.Cow;
						lText.Add("Number <color="+sRichTextColorCow+">"
							+aDigit[i]+"</color> " + (bAlso ? "also" : "") +
							" seems to hint something about the exit, but it"+
							(bAlso?", too,":"") + " looks misplaced...");
						prevDigitType = aDigitType[i];
						break;
				}
			}
			if(bullCount == 3)
				lText.Add("Maybe this is <color=yellow>the room</color>!?");
		}
	}

	private IEnumerator rfInteract(){
		PlayerController playerController = PlayerController.Instance;
		playerController.setActiveInputMovement(false);
		playerController.turnToward(transform);

		//ThirdPersonCameraControl cameraControl = Camera.main.GetComponent<ThirdPersonCameraControl>();
		//Transform tCamTarget = cameraControl.tTarget;
		vCamTargetStart = tCamTarget.position;
		vCamTargetEnd = transform.position;
		qCamTargetStart = tCamTarget.localRotation;
		Vector3 eulerCamTarget = tCamTarget.localEulerAngles;
		qCamTargetEnd = Quaternion.Euler(
			eulerCamTarget.newX(Mathf.Max(minCamEulerX,eulerCamTarget.x))
		);
		camDistanceStart = cameraControl.targetCameraDistance;
		camDistanceEnd = targetDistanceCamera;
		subitrPanCamera.bReverse = false;
		subitrPanCamera.Reset();
		cameraControl.enabled = false;
		//TweenRoutineUnit subitrPanCamera = new TweenRoutineUnit(
		//	(float t) => {
		//		tCamTarget.position = Vector3.Lerp(vCamTargetStart,vCamTargetEnd,t);
		//		tCamTarget.rotation = Quaternion.Lerp(qCamTargetStart,qCamTargetEnd,t);
		//		cameraControl.targetCameraDistance = Mathf.Lerp(camDistanceStart,camDistanceEnd,t);
		//	},
		//	durationPan
		//);
		yield return subitrPanCamera;
		
		FooterManager footerManager = FooterManager.Instance;
		footerManager.showFooter(lText);
		while(!footerManager.IsDone)
			yield return null;
		while(!triggerSkip) //last passage
			yield return null;

		footerManager.hideFooter();
		subitrPanCamera.bReverse = true;
		yield return subitrPanCamera;
		playerController.setActiveInputMovement(true);
		cameraControl.enabled = true;
	}
	public override void onInteracted(){
		if(!routineInteract.IsRunning)
			routineInteract.start(this,rfInteract());
		else if(!FooterManager.Instance.IsDone)
			FooterManager.Instance.stepFooter();
		else
			triggerSkip.set();
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DigitMessage))]
class DigitMessageEditor : MonoBehaviourBakerEditor{ }
#endif
