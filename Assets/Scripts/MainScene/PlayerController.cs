using UnityEngine;
using UnityEngine.InputSystem;
using Chameleon;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AnimationPlayer))]
public class PlayerController : LoneMonoBehaviour<PlayerController>{
	PlayerInput playerInput;
	AnimationPlayer animPlayer;
	Rigidbody rb;
	
	[Header("InputMove")]
	[SerializeField] InputActionID actionIDMove;
	[SerializeField] AnimationClip clipIdle;
	[SerializeField] AnimationClip clipRun;
	[SerializeField] float transitionTime = 0.2f;
	[SerializeField] float turnSpeed;
	private Vector2 v2Direction;
	private LoneCoroutine routineMove = new LoneCoroutine();
	private const float RUNSPEED = 2.939501f/0.7f;
	private bool bMoving;
	private LoneCoroutine routineTurn = new LoneCoroutine();
	private float targetFacingAngle;
	private float facingAngle;
	//private Vector3 vMoveDirection;

	[Header("InputLook")]
	[SerializeField] InputActionID actionIDLook;
	[SerializeField] Transform tVCamTarget;
	[SerializeField] Vector2 v2LookRangeVertical;
	public float inputLookSensitivity;

	[Header("InputZoom")]
	[SerializeField] InputActionID actionIDZoom;
	[SerializeField] ThirdPersonCameraControl cameraControl;
	[SerializeField] Vector2 rangeZoom;
	[SerializeField] float inputZoomSensitivity;

	[Header("InputInteract")]
	[SerializeField] InputActionID actionIDInteract;
	[SerializeField] AnimationClip clipLeftTurn90;
	[SerializeField] AnimationClip clipRightTurn90; //Unity cannot mirror generic rig, so need this
	[SerializeField] Vector2 rangeTurnWeight;
	[SerializeField] float turnTime;
	[SerializeField] AnimationClip clipPickup;

	protected override void Awake(){
		base.Awake();
		playerInput = GetComponent<PlayerInput>();
		animPlayer = GetComponent<AnimationPlayer>();
		rb = GetComponent<Rigidbody>();
	}
	void OnEnable(){
		playerInput.actions[actionIDMove].performed += onInputMove;
		playerInput.actions[actionIDMove].canceled += onInputIdle;
		playerInput.actions[actionIDLook].performed += onInputLook;
		playerInput.actions[actionIDZoom].performed += onInputZoom;
		playerInput.actions[actionIDInteract].performed += onInputInteract;
	}
	void OnDisable(){
		playerInput.actions[actionIDMove].performed -= onInputMove;
		playerInput.actions[actionIDMove].canceled -= onInputIdle;
		playerInput.actions[actionIDLook].performed -= onInputLook;
		playerInput.actions[actionIDZoom].performed -= onInputZoom;
		playerInput.actions[actionIDInteract].performed += onInputInteract;
		routineTurn.stop();
	}
	void Start(){
		animPlayer.play(clipIdle);
	}
	private void onInputMove(InputAction.CallbackContext context){
		v2Direction = context.ReadValue<Vector2>();
		targetFacingAngle = v2Direction.polarAngle()*Mathf.Rad2Deg -90.0f; //delta from tVCamTarget forward
		routineTurn.start(this,rfTurn(targetFacingAngle));
		routineMove.start(this,rfMove());
		//vMoveDirection = Quaternion.Euler(0.0f,-targetFacingAngle,0.0f)*tVCamTarget.forward;
		//dRedirectRootMotion = ()=>{redirectRootMotion(vDirection);};
		//animPlayer.evOnLateUpdate += dRedirectRootMotion;
		//faceDirection(v2Direction.polarAngle()*Mathf.Rad2Deg -90.0f);
		animPlayer.transitionTo(clipRun,transitionTime);
		bMoving = true;
	}
	private void onInputIdle(InputAction.CallbackContext context){
		routineTurn.stop();
		routineMove.stop();
		animPlayer.transitionTo(clipIdle,transitionTime);
		bMoving = false;
	}
	private void onInputLook(InputAction.CallbackContext context){
		Vector2 v2Delta = inputLookSensitivity*context.ReadValue<Vector2>();
		Vector3 eulerAngles = tVCamTarget.eulerAngles;
		eulerAngles.y = (eulerAngles.y+v2Delta.x) % 360.0f;
		eulerAngles.x = Mathf.Clamp(
			tVCamTarget.getPitchAngle()-v2Delta.y,
			v2LookRangeVertical.x,
			v2LookRangeVertical.y
		);
		tVCamTarget.eulerAngles = eulerAngles;
		if(bMoving){
			//faceDirection(v2Direction.polarAngle()*Mathf.Rad2Deg -90.0f);
			transform.setEulerY(tVCamTarget.eulerAngles.y-facingAngle);
		}
	}
	//private void faceDirection(float angle){ //delta angle from tVCamTarget forward
	//	/* Trick from Unity tutorial */
	//	transform.setEulerY(tVCamTarget.eulerAngles.y-angle);
	//	//tVCamTarget.setLocalEulerY(angle);
	//	//vMoveDirection = transform.rotation*Vector3.forward;
	//}
	private void onInputZoom(InputAction.CallbackContext context){
		float delta = inputZoomSensitivity*context.ReadValue<float>(); //usually Windows is +/-120
		cameraControl.targetCameraDistance =
			Mathf.Clamp(
				cameraControl.targetCameraDistance + delta,
				rangeZoom.x,
				rangeZoom.y
			)
		;
	}
	private IEnumerator rfTurn(float angleEnd){
		float angleStart = -transform.eulerAngles.y+tVCamTarget.eulerAngles.y; //left-handed
		float t = 0.0f;
		float turnTime = Mathf.Abs(Mathf.DeltaAngle(angleStart,angleEnd))/turnSpeed;
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		while(t < 1.0f){
			yield return null;
			facingAngle = Mathf.LerpAngle(angleStart,angleEnd,t);
			transform.setEulerY(tVCamTarget.eulerAngles.y-facingAngle);
			t += Time.deltaTime/turnTime;
		}
		transform.setEulerY(tVCamTarget.eulerAngles.y-angleEnd);
	}
	private IEnumerator rfMove(){
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		while(true){
			rb.position += RUNSPEED*Time.fixedDeltaTime*
				(Quaternion.Euler(0.0f,-targetFacingAngle,0.0f)*tVCamTarget.forward);
			yield return wait;
		}
	}
	private void onInputInteract(InputAction.CallbackContext context){
		Interactable.Focused?.onInteracted();
	}

	public void setActiveInputMovement(bool bEnable){
		if(bEnable){
			routineTurn.stop();
			animPlayer.transitionTo(clipIdle,transitionTime);
			playerInput.actions[actionIDMove].Enable();
			playerInput.actions[actionIDLook].Enable();
			playerInput.actions[actionIDZoom].Enable();
		}
		else{
			routineMove.stop();
			routineTurn.stop();
			animPlayer.transitionTo(clipIdle,transitionTime);
			playerInput.actions[actionIDMove].Disable();
			playerInput.actions[actionIDLook].Disable();
			playerInput.actions[actionIDZoom].Disable();
		}
	}
	public void turnToward(Transform tTarget){
		routineTurn.start(this,rfTurnToward(tTarget));
	}
	private IEnumerator rfTurnToward(Transform tTarget){
		Vector3 vDirection = tTarget.position-transform.position;
		float eulerYStart = transform.eulerAngles.y;
		float eulerYEnd = vDirection.xz().polarAngle()*-Mathf.Rad2Deg + 90.0f;
		//Because eulerAngles is positive clockwise
		
		animPlayer.addLayer(1,null,true);
		float deltaAngle = Mathf.DeltaAngle(eulerYStart,eulerYEnd);
		PlayableController playableController= animPlayer.transitionTo(
			deltaAngle>=0 ? clipLeftTurn90 : clipRightTurn90,
			transitionTime,
			1,
			resetMode: eTransitionResetMode.resetAlways
		);
		animPlayer.setLayerWeight(1,Mathf.Clamp(
			Mathf.Abs(Mathf.DeltaAngle(eulerYStart,eulerYEnd)/90.0f),
			rangeTurnWeight.x,
			rangeTurnWeight.y
		));
		
		float t = 0.0f;
		while(t < 1.0f){
			yield return null;
			Quaternion qLookTarget = tVCamTarget.rotation;
			facingAngle = Mathf.LerpAngle(eulerYStart,eulerYEnd,t);
			transform.setEulerY(facingAngle);
			//tVCamTarget.rotation = qLookTarget; //restore camera rotation
			t += Time.deltaTime/turnTime;
		}
		animPlayer.transitionTo((AnimationClip)null,transitionTime,1);
	}
	public bool ShowCursor{
		get{return Cursor.lockState != CursorLockMode.Locked;}
		set{Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;}
	}

	[SerializeField] Transform tTest;
	void Update(){
		if(Keyboard.current.xKey.wasPressedThisFrame)
			turnToward(tTest);
		if(Keyboard.current.yKey.wasPressedThisFrame)
			animPlayer.play(clipRightTurn90);
		if(Keyboard.current.cKey.wasPressedThisFrame)
			ShowCursor = true;
		if(Keyboard.current.hKey.wasPressedThisFrame)
			ShowCursor = false;
	}
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerController))]
//class PlayerControllerEditor : MonoBehaviourBakerEditor{ }
//#endif
