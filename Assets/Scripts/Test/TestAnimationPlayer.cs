using UnityEngine;
using Chameleon;
using UnityEngine.InputSystem;


public class TestAnimationPlayer : MonoBehaviour{
	AnimationPlayer animPlayer;
	PlayerInput playerInput;
	[SerializeField] InputActionID inputIDMove;
	[SerializeField] AnimationClip clipIdle;
	[SerializeField] AnimationClip clipWalk;

	void Awake(){
		animPlayer = GetComponent<AnimationPlayer>();
		playerInput = GetComponent<PlayerInput>();
		GraphVisualizerClient.Show(animPlayer.getGraph());
	}
	void OnEnable(){
		playerInput.actions[inputIDMove.Id].performed += onInputMove;
	}
	void OnDisable(){
		playerInput.actions[inputIDMove.Id].performed -= onInputMove;
	}
	void Start(){
		animPlayer.play(clipIdle);
	}

	private void onInputMove(InputAction.CallbackContext context){
		Vector2 inputValue = context.ReadValue<Vector2>();
		if(inputValue.y > 0){
			animPlayer.transitionTo(clipWalk,0.2f);
		}
		else{
			animPlayer.transitionTo(clipIdle,0.2f);
		}
	}
}
