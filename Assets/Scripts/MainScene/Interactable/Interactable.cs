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
	[Bakable][Tag] const string tagPlayer = "Player";
	[SerializeField][ShowPosition(true,"Balloon")] protected Vector3 vBallonPos;

	public static Interactable Focused{get; private set;} = null;
	public static bool interact(){
		if(Focused){
			Focused.onInteracted();
			return true;
		}
		return false;
	}
	protected virtual void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = this;
			Canvas cvBalloon = SceneMainManager.Instance.CanvasBallon;
			cvBalloon.transform.position = transform.TransformPoint(vBallonPos);
			cvBalloon.gameObject.SetActive(true);
		} 
	}
	protected virtual void OnTriggerExit(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = null;
			SceneMainManager.Instance.CanvasBallon.gameObject.SetActive(false);
		}
	}
	public abstract void onInteracted();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable),true)]
class InteractableEditor: MonoBehaviourBakerEditorWithScene{}
#endif
