using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Chameleon;

public class TestDragDrop : MonoBehaviour{

}

#if UNITY_EDITOR
[CustomEditor(typeof(TestDragDrop))]
class TestDragDropEditor : Editor{
	private TestDragDrop targetAs;

	void OnEnable(){
		targetAs = target as TestDragDrop;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		EditorGUILayout.LabelField("Whatever");
		EditorGUILayout.LabelField("Whatever2");

		Object[] ag = EditorHelper.dropZone(GUILayoutUtility.GetLastRect(),typeof(GameObject));
		if(ag!=null)
			foreach(GameObject g in ag)
				Debug.Log(g);
	}
}
#endif
