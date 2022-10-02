using UnityEngine;
using UnityEngine.InputSystem;
using Chameleon;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour{
	[SerializeField] InputActionID actionIDMove;
	[SerializeField] AnimatorParamID_Bool animParamRun;

	PlayerInput playerInput;
	Animator animator;

	void Awake(){
		playerInput = GetComponent<PlayerInput>();
		animator = GetComponent<Animator>();
	}
	void OnEnable(){
		playerInput.actions[actionIDMove.Id].performed += onInputMove;
		playerInput.actions[actionIDMove.Id].canceled += onInputIdle;
	}
	void OnDisable(){
		playerInput.actions[actionIDMove.Id].performed -= onInputMove;
		playerInput.actions[actionIDMove.Id].canceled -= onInputIdle;
	}
	void onInputMove(InputAction.CallbackContext context){
		animator.setParameter(animParamRun,true);
		Vector2 v2Direction = context.ReadValue<Vector2>();
	}
	void onInputIdle(InputAction.CallbackContext context){
		animator.setParameter(animParamRun,false);
	}
}
