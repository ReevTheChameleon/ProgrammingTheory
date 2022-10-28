using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interactable : MonoBehaviour{
	[SerializeField] string textFooter;
	[SerializeField] string textCommand;
	Collider cBound;

	void Awake(){
		cBound = GetComponent<Collider>();
	}
	void OnTriggerEnter(Collider other){
		onInteracted();
	}
	protected virtual void onInteracted(){
		//SceneMainManager.Instance.showFooter(textFooter);
	}

}

[CustomEditor(typeof(Interactable))]
class InteractableEditor: MonoBehaviourBakerEditor{}
