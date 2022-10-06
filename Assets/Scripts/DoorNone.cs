using UnityEngine;
using Chameleon;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoorNone : MonoBehaviour{
	private LoneCoroutine routineUnveil;
	private LoneCoroutine routineVeil;
	private Material matInstanceDoor;
	[Bakable] float durationUnveil = 1.5f;
	public 

	void Awake(){
		matInstanceDoor = GetComponent<Renderer>().material;
		/* This create a material clone, but it is necessary because each door will need
		their own material when manipulating alpha. If use object pooling, this shouldn't
		create too many clones.
		Alternatively, one can use MaterialPropertyBlock, but we are using URP which allows
		SRP batching, and MaterialPropertyBlock is not compatible with it.
		Because we are sharing Lit Shader with other Materials, this approach seems best. */
		routineUnveil = new LoneCoroutine();
		routineVeil = new LoneCoroutine(onVeilComplete);
	}
	private void onVeilComplete(){
		SceneMainManager.Instance.discardNeighbor();
	}
	void OnTriggerEnter(Collider other){
		SceneMainManager.Instance.prepareNeighbor(transform);
		routineVeil.stop(this,false);
		routineUnveil.start(this,rfUnveil());
	}
	void OnTriggerExit(Collider other){
		SceneMainManager.Instance.updateCurrentRoom();
		routineUnveil.stop(this);
		routineVeil.start(this,rfVeil());
	}
	void OnDisable(){
		matInstanceDoor.color = matInstanceDoor.color.newA(1.0f);
	}
	void OnDestroy(){
		Destroy(matInstanceDoor);
	}
	private IEnumerator rfUnveil(){
		float tweenAmount = matInstanceDoor.color.a;
		yield return matInstanceDoor.tweenAlpha(
			1.0f,
			0.0f,
			durationUnveil*tweenAmount,
			TweenRoutine.eTweenLoopMode.Once,
			1.0f-tweenAmount
		);
	}
	private IEnumerator rfVeil(){
		float tweenAmount = 1.0f-matInstanceDoor.color.a;
		yield return matInstanceDoor.tweenAlpha(
			0.0f,
			1.0f,
			durationUnveil*tweenAmount,
			TweenRoutine.eTweenLoopMode.Once,
			1.0f-tweenAmount
		);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DoorNone))]
class DoorNormalEditor : MonoBehaviourBakerEditor{
	private DoorNone targetAs;
	private BoxCollider boxCollider;

	protected override void OnEnable(){
		base.OnEnable();
		targetAs = (DoorNone)target;
		boxCollider = targetAs.GetComponent<BoxCollider>();
	}
	public override void OnInspectorGUI(){
		float localScaleZ = targetAs.transform.localScale.z;
		EditorGUI.BeginChangeCheck();
		float userColliderThickness = EditorGUILayout.FloatField(
			"Collider Thickness",
			boxCollider.size.z*localScaleZ
		);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(boxCollider,"Change DoorNormal collider size");
			boxCollider.size = boxCollider.size.newZ(userColliderThickness/localScaleZ);
			//boxCollider.center = boxCollider.center.newZ(-boxCollider.size.z/2);
		}
		base.OnInspectorGUI();
	}
}
#endif
