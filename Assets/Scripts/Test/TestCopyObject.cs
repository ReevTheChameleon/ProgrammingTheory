using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestCopyObject : MonoBehaviour{
	static SerializedObject so;
	[MenuItem("CONTEXT/Component/Copy")]
	public static void copyCheck(){
		EditorApplication.ExecuteMenuItem("CONTEXT/Transform/Copy Component");
	}
}