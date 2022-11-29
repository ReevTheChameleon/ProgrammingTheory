using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestScreenPoint : MonoBehaviour{
	public Transform t;
	public Vector2 v2;
	public RectTransform rt;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestScreenPoint))]
class TestScreenPointEditor : Editor{
	private TestScreenPoint targetAs;

	void OnEnable(){
		targetAs = target as TestScreenPoint;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if(GUILayout.Button("Click")){
			((RectTransform)targetAs.transform).anchoredPosition = targetAs.v2;
		}
		if(GUILayout.Button("Click2")){
			Debug.Log(Camera.main.pixelWidth+" "+Camera.main.pixelHeight);
			Debug.Log(Camera.main.WorldToScreenPoint(targetAs.transform.position));
		}
		if(GUILayout.Button("Click3")){
			Vector3 vScreen = Camera.main.WorldToScreenPoint(targetAs.t.position);
			Debug.Log(vScreen);
			targetAs.transform.position = vScreen;
		}
		if(GUILayout.Button("Click4")){
			Debug.Log(targetAs.rt.position);
		}
	}
}
#endif
