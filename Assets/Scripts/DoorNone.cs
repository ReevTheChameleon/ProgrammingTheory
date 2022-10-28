using UnityEngine;
using Chameleon;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IDoor{
	void reset();
}
public class DoorNone : MonoBehaviour,IDoor{
	private LoneCoroutine routineDoor = new LoneCoroutine();
	private Material matInstanceDoor;
	[Bakable] static float durationUnveil = 0.5f;
	private TweenRoutineUnit subitrUnveil;

	void Awake(){
		matInstanceDoor = GetComponent<Renderer>().material;
		/* This create a material clone, but it is necessary because each door will need
		their own material when manipulating alpha. If use object pooling, this shouldn't
		create too many clones.
		Alternatively, one can use MaterialPropertyBlock, but we are using URP which allows
		SRP batching, and MaterialPropertyBlock is not compatible with it.
		Because we are sharing Lit Shader with other Materials, this approach seems best. */
		subitrUnveil = matInstanceDoor.tweenAlpha(1.0f,0.0f,durationUnveil);
	}
	private IEnumerator rfUnveil(){
		SceneMainManager.Instance.prepareNeighbor(transform);
		subitrUnveil.bReverse = false;
		yield return subitrUnveil;
	}
	private IEnumerator rfVeil(){
		subitrUnveil.bReverse = true;
		yield return subitrUnveil;
		SceneMainManager.Instance.discardNeighbor();
	}
	void OnTriggerEnter(Collider other){
		routineDoor.stop();
		routineDoor.start(this,rfUnveil());
	}
	void OnTriggerExit(Collider other){
		SceneMainManager.Instance.updateCurrentRoom();
		routineDoor.stop();
		routineDoor.start(this,rfVeil());
	}
	void OnDisable(){
		reset();
	}
	void OnDestroy(){
		Destroy(matInstanceDoor);
	}
	public void reset(){
		routineDoor.stop();
		subitrUnveil.bReverse = false;
		subitrUnveil.Reset();
		matInstanceDoor.color = matInstanceDoor.color.newA(1.0f);
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
