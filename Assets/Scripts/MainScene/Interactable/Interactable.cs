using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

/* Note: Currently the code will not work well if 2 triggers overlap.
It is possible to fix, but if there is no overlap triggers, it wastes performance,
so it is not implemented. */
[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour{
	[Bakable][Tag] protected const string tagPlayer = "Player";
	[SerializeField] protected Vector2 rangeEulerYInteractable;
	[SerializeField][ShowPosition(true,"Balloon")] protected Vector3 vBallonPos;
	protected Transform tPlayer;
	protected bool bHideBalloon;
	private bool bActivatingBalloon = false;

	public static Interactable Focused{get; private set;} = null;
	public static bool interact(){
		if(Focused){
			Focused.onInteracted();
			return true;
		}
		return false;
	}
	protected virtual void Awake(){
		enabled = false;
	}
	protected virtual void OnDisable(){
		deactivate();
	}
	protected virtual void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			this.enabled = true;
			//activate();
			Update();
		}
	}
	protected virtual void OnTriggerExit(Collider other){
		if(other.CompareTag(tagPlayer)){
			deactivate();
			this.enabled = false;
		}
	}
	protected virtual void activate(){
		Focused = this;
		Canvas cvBalloon = SceneMainManager.Instance.CanvasBalloon;
		cvBalloon.transform.position = transform.TransformPoint(vBallonPos);
		cvBalloon.gameObject.SetActive(true);
		HeadLookController.Instance.setHeadLookTarget(transform);
		bActivatingBalloon = true;
	}
	protected virtual void deactivate(){
		Focused = null;
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);
		HeadLookController.Instance.setHeadLookTarget(null);
		bActivatingBalloon = false;
	}
	protected virtual void Update(){
		if(bHideBalloon)
			return;
		bool bInRange = IsInYLookRange;
		if(!bActivatingBalloon && bInRange)
			activate();
		else if(bActivatingBalloon && !bInRange)
			deactivate();
	}
	protected bool IsInYLookRange{
		get{
			Transform tPlayer = PlayerController.Instance.transform;
			float deltaAngle = Mathf.DeltaAngle(
				tPlayer.eulerAngles.y,
				(transform.position-tPlayer.position).eulerAngles().y
			);
			return 
				deltaAngle>=rangeEulerYInteractable.x && 
				deltaAngle<=rangeEulerYInteractable.y
			;
		}
	}
	public abstract void onInteracted();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable),true)]
class InteractableEditor: MonoBehaviourBakerEditorWithScene{}
#endif
