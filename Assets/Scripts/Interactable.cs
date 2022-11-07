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

	[SerializeField][ShowPosition("Balloon")] Vector3 vBallonPos;
	private CanvasGroupFader canvasGroupFader;

	public static Interactable Focused{get; private set;}
	public static bool interact(){
		if(Focused){
			Focused.onInteracted();
			return true;
		}
		return false;
	}
	void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = this;
			if(!canvasGroupFader || !canvasGroupFader.gameObject.activeInHierarchy)
				canvasGroupFader = SceneMainManager.Instance.PoolerBalloon
					.getObject(vBallonPos).GetComponentInChildren<CanvasGroupFader>(true); //include inactive (Credit: damdor, UF)
			canvasGroupFader.fade(true);
			//canvasGroupFader.gameObject.SetActive(true);
			//routineFade.getItr<TweenRoutineUnit>().bReverse = false;
			//routineFade.resume();
			//canvasGroup.gameObject.SetActive(true);
		} 
	}
	void OnTriggerExit(Collider other){
		if(other.CompareTag(tagPlayer)){
			Focused = null;
			canvasGroupFader.fade(false);
			//routineFade.getItr<TweenRoutineUnit>().bReverse = true;
			//routineFade.resume();
		}
	}
	protected virtual void onInteracted(){ }
}

[CustomEditor(typeof(Interactable))]
class InteractableEditor: MonoBehaviourBakerEditorWithScene{}
