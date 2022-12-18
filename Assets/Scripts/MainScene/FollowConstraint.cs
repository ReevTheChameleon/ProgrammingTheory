using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FollowConstraint : MonoBehaviour{
	public Transform tTarget;
	public Vector3 vOffset;

	void LateUpdate(){
		transform.position = TargetPosition;
	}
	public Vector3 TargetPosition{ get{return tTarget.position+vOffset;} }
	
	#if UNITY_EDITOR
	void OnValidate(){
		LateUpdate();
	}
	#endif
}

[CustomEditor(typeof(FollowConstraint))]
class FollowConstraintEditor : Editor{
	private FollowConstraint targetAs;
	void OnEnable(){
		targetAs = (FollowConstraint)target;
	}
	void OnSceneGUI(){
		targetAs.vOffset = targetAs.transform.position - targetAs.tTarget.position;
	}
}
