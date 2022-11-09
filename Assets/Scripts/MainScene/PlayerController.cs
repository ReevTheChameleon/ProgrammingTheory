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
	//private Vector3 vMoveDirection;

	[Header("InputLook")]
	[SerializeField] InputActionID actionIDLook;
	[SerializeField] Transform tVCamTarget;
	[SerializeField] Vector2 v2LookRangeVertical;
	public float inputLookSensitivity;

	[Header("InputZoom")]
	[SerializeField] InputActionID actionIDZoom;
	[SerializeField] ThirdPersonCameraControl cameraControl;
	[SerializeField] Vector2 v2ZoomRange;
	[SerializeField] float inputZoomSensitivity;

	[Header("InputInteract")]
	[SerializeField] InputActionID actionIDInteract;

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
			faceDirection(facingAngle);
		}
	}
	private float facingAngle;
	private void faceDirection(float angle){ //delta angle from tVCamTarget forward
		/* Trick from Unity tutorial */
		transform.setEulerY(tVCamTarget.eulerAngles.y-angle);
		tVCamTarget.setLocalEulerY(angle);
		//vMoveDirection = transform.rotation*Vector3.forward;
	}
	private void onInputZoom(InputAction.CallbackContext context){
		float delta = inputZoomSensitivity*context.ReadValue<float>(); //usually Windows is +/-120
		cameraControl.targetCameraDistance =
			Mathf.Clamp(
				cameraControl.targetCameraDistance + delta,
				v2ZoomRange.x,
				v2ZoomRange.y
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
			faceDirection(facingAngle);
			t += Time.deltaTime/turnTime;
		}
		facingAngle = angleEnd;
		faceDirection(angleEnd);
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
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerController))]
//class PlayerControllerEditor : MonoBehaviourBakerEditor{ }
//#endif
