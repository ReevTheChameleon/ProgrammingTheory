#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class TestPrefab{
	[MenuItem("Chameleon/TestPrefab")]
	static void testPrefab(){
		GameObject g = Selection.activeGameObject;
		Debug.Log(
			"GetPrefabAssetType: "+PrefabUtility.GetPrefabAssetType(g) + "\n" +
			"GetPrefabInstanceStatus: "+PrefabUtility.GetPrefabInstanceStatus(g)
		);
	}
}
#endif
