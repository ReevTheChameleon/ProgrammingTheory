using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour{
	[Bakable] static string tagPlayer = "Player";
	[SerializeField][ShowPosition("Balloon")] Vector3 vBallonPos;
	private Canvas cvBalloon;

	public static Interactable Focused{get; private set;}
	public static bool interact(){
		if(Focused){
			Focused.onInteracted();
			return true;
		}
		return false;
	}
	void Awake(){
		cvBalloon = SceneMainManager.Instance.CanvasBalloon;
	}
	void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = this;
			cvBalloon.transform.position = vBallonPos;
			cvBalloon.gameObject.SetActive(true);
		} 
	}
	void OnTriggerExit(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = null;
			cvBalloon.gameObject.SetActive(false);
		}
	}
	protected virtual void onInteracted(){ }
}

[CustomEditor(typeof(Interactable))]
class InteractableEditor: MonoBehaviourBakerEditorWithScene{}
