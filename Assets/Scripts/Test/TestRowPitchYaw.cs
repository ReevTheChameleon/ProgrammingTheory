using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestRowPitchYaw : MonoBehaviour{

}

#if UNITY_EDITOR
[CustomEditor(typeof(TestRowPitchYaw))]
class TestRowPitchYawEditor : Editor{
	private TestRowPitchYaw targetAs;

	void OnEnable(){
		targetAs = target as TestRowPitchYaw;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if(GUILayout.Button("Click"))
			Debug.Log(targetAs.transform.eulerAngles);
	}
}
#endif
