using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TestPauseEditor : MonoBehaviour{
	void Update(){
		//Thread.Sleep(1000);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestPauseEditor))]
class TestPauseEditorEditor : Editor{
	private TestPauseEditor targetAs;

	void OnEnable(){
		targetAs = target as TestPauseEditor;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
	}
}
#endif
