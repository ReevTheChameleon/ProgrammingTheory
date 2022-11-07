using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour{
	//[SerializeField] string textFooter;
	//[SerializeField] string textCommand;
	//[SerializeField] Collider cBound;
	[Bakable] static string tagPlayer = "Player";
	[Bakable] static float fadeTime = 0.25f;
	[SerializeField][ShowPosition("Balloon")] Vector3 vBallonPos;
	private LoneCoroutine routineFade;
	private CanvasGroup cvgBalloon;

	public static Interactable Focused{get; private set;}
	public static bool interact(){
		if(Focused){
			Focused.onInteracted();
			return true;
		}
		return false;
	}
	void Awake(){
		cvgBalloon = SceneMainManager.Instance.CanvasGroupBalloon;
		routineFade = new LoneCoroutine(
			this,
			cvgBalloon.tweenAlpha(
				0.0f,1.0f,fadeTime,
				dOnDone:(float t)=>{
					if(routineFade.getItr<TweenRoutineUnit>().bReverse)
						cvgBalloon.gameObject.SetActive(false);
				}
			)
		);
	}
	void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = this;
			cvgBalloon.transform.position = vBallonPos;
			cvgBalloon.gameObject.SetActive(true);
			routineFade.getItr<TweenRoutineUnit>().bReverse = false;
			routineFade.resume();
		} 
	}
	void OnTriggerExit(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = null;
			routineFade.getItr<TweenRoutineUnit>().bReverse = true;
			routineFade.resume();
		}
	}
	protected virtual void onInteracted(){ }
}

[CustomEditor(typeof(Interactable))]
class InteractableEditor: MonoBehaviourBakerEditorWithScene{}
