using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Chameleon;

public class TestQuaternion : MonoBehaviour{
	public Vector3 v;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestQuaternion))]
class TestQuaternionEditor : Editor{
	private TestQuaternion targetAs;

	void OnEnable(){
		targetAs = target as TestQuaternion;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if(GUILayout.Button("Reflect")){
			Quaternion q = QuaternionExtension.qReflection(targetAs.v);
			Debug.Log(q.toPreciseString());
			Debug.Log((q*targetAs.v).toPreciseString());
		}
	}
}
#endif
