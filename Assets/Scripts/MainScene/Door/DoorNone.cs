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
	[SerializeField] BoxCollider boxCollider;
	[Bakable] static float durationUnveil = 0.4f;
	private LoneCoroutine routineDoor;
	private Material matInstanceDoor;
	private TweenRoutineUnit subitrUnveil;

	void Awake(){
		matInstanceDoor = GetComponent<Renderer>().material;
		/* This create a material clone, but it is necessary because each door will need
		their own material when manipulating alpha. If use object pooling, this shouldn't
		create too many clones.
		Alternatively, one can use MaterialPropertyBlock, but we are using URP which allows
		SRP batching, and MaterialPropertyBlock is not compatible with it.
		Because we are sharing Lit Shader with other Materials, this approach seems best. */
		subitrUnveil = matInstanceDoor.tweenAlpha(
			1.0f,0.0f,durationUnveil,
			dOnDone: (float t) => {
				Debug.Log(t);
				if(t<=0.0f) //reverse
					SceneMainManager.Instance.discardNeighbor();
				else if(t>=1.0f) //forward
					boxCollider.enabled = false; //allow player to pass when alpha is 0
			}
		);
		routineDoor = new LoneCoroutine(this,subitrUnveil);
	}
	void OnTriggerEnter(Collider other){
		SceneMainManager.Instance.prepareNeighbor(transform);
		subitrUnveil.bReverse = false;
		routineDoor.resume();
	}
	void OnTriggerExit(Collider other){
		SceneMainManager.Instance.updateCurrentRoom();
		boxCollider.enabled = true;
		subitrUnveil.bReverse = true;
		routineDoor.resume();
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
	private BoxCollider boxTrigger;

	protected override void OnEnable(){
		base.OnEnable();
		targetAs = (DoorNone)target;
		BoxCollider[] aBoxCollider = targetAs.GetComponents<BoxCollider>();
		for(int i=0; i<aBoxCollider.Length; ++i){
			if(aBoxCollider[i].isTrigger)
				boxTrigger = aBoxCollider[i];
		}
	}
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		float localScaleZ = targetAs.transform.localScale.z;
		EditorGUI.BeginChangeCheck();
		float userColliderThickness = EditorGUILayout.FloatField(
			"Collider Thickness",
			boxTrigger.size.z*localScaleZ
		);
		if(EditorGUI.EndChangeCheck()){
			Undo.RecordObject(boxTrigger,"Change DoorNormal collider size");
			boxTrigger.size = boxTrigger.size.newZ(userColliderThickness/localScaleZ);
			//boxCollider.center = boxCollider.center.newZ(-boxCollider.size.z/2);
		}
	}
}
#endif
