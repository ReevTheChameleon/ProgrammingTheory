using UnityEngine;
using UnityEngine.InputSystem;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AnimationPlayer))]
public class PlayerController : LoneMonoBehaviour<PlayerController>{
	PlayerInput playerInput;
	AnimationPlayer animPlayer;
	
	[Header("InputMove")]
	[SerializeField] InputActionID actionIDMove;
	[SerializeField] AnimationClip clipIdle;
	[SerializeField] AnimationClip clipRun;
	[SerializeField] float transitionTime = 0.2f;
	private bool bMoving;
	private Vector2 v2Direction;

	[Header("inputLook")]
	[SerializeField] InputActionID actionIDLook;
	[SerializeField] Transform tVCamTarget;
	[SerializeField] Vector2 v2LookRangeVertical;
	public float inputLookSensitivity;

	[Header("inputZoom")]
	[SerializeField] InputActionID actionIDZoom;
	[SerializeField] ThirdPersonCameraControl cameraControl;
	[SerializeField] Vector2 v2ZoomRange;
	[SerializeField] float inputZoomSensitivity;

	protected override void Awake(){
		base.Awake();
		playerInput = GetComponent<PlayerInput>();
		animPlayer = GetComponent<AnimationPlayer>();
	}
	void OnEnable(){
		playerInput.actions[actionIDMove].performed += onInputMove;
		playerInput.actions[actionIDMove].canceled += onInputIdle;
		playerInput.actions[actionIDLook].performed += onInputLook;
		playerInput.actions[actionIDZoom].performed += onInputZoom;
	}
	void OnDisable(){
		playerInput.actions[actionIDMove].performed -= onInputMove;
		playerInput.actions[actionIDMove].canceled -= onInputIdle;
		playerInput.actions[actionIDLook].performed -= onInputLook;
		playerInput.actions[actionIDZoom].performed -= onInputZoom;
	}
	private void onInputMove(InputAction.CallbackContext context){
		v2Direction = context.ReadValue<Vector2>();
		faceDirection(v2Direction.polarAngle()*Mathf.Rad2Deg -90.0f);
		animPlayer.transitionTo(clipRun,transitionTime);
		bMoving = true;
	}
	private void onInputIdle(InputAction.CallbackContext context){
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
		if(bMoving)
			faceDirection(v2Direction.polarAngle()*Mathf.Rad2Deg -90.0f);
	}
	private void faceDirection(float angle){ //delta angle from tVCamTarget forward
		/* Trick from Unity tutorial */
		transform.setEulerY(tVCamTarget.eulerAngles.y-angle);
		tVCamTarget.setLocalEulerY(angle);
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
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerController))]
//class PlayerControllerEditor : MonoBehaviourBakerEditor{ }
//#endif
