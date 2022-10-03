using UnityEngine;
using UnityEngine.InputSystem;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(AnimationPlayer))]
public class PlayerController : LoneMonoBehaviour<PlayerController>{
	//InputActionIDs
	[SerializeField] InputActionID actionIDMove;

	//AnimationClips
	[SerializeField] AnimationClip clipIdle;
	[SerializeField] AnimationClip clipRun;
	[Bakable] float transitionTime = 0.2f;

	[Bakable] float forwardAngle = 90f; //y Euler Angle of forward direction

	PlayerInput playerInput;
	AnimationPlayer animPlayer;

	protected override void Awake(){
		base.Awake();
		playerInput = GetComponent<PlayerInput>();
		animPlayer = GetComponent<AnimationPlayer>();
	}
	void OnEnable(){
		playerInput.actions[actionIDMove].performed += onInputMove;
		playerInput.actions[actionIDMove].canceled += onInputIdle;
	}
	void OnDisable(){
		playerInput.actions[actionIDMove].performed -= onInputMove;
		playerInput.actions[actionIDMove].canceled -= onInputIdle;
	}
	void onInputMove(InputAction.CallbackContext context){
		Vector2 v2Direction = context.ReadValue<Vector2>();
		transform.setEulerY(-v2Direction.polarAngle()*Mathf.Rad2Deg + forwardAngle);
		animPlayer.transitionTo(clipRun,transitionTime);
	}
	void onInputIdle(InputAction.CallbackContext context){
		animPlayer.transitionTo(clipIdle,transitionTime);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerController))]
class PlayerControllerEditor : MonoBehaviourBakerEditor{ }
#endif
