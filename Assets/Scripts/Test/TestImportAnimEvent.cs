using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public static class TestImportAnimEvent{
	[MenuItem("CONTEXT/AnimationClip/Show Event")]
	static void AnimationClipShowEvent(MenuCommand menuCommand){
		string sPath = AssetDatabase.GetAssetPath((AnimationClip)menuCommand.context);
		sPath += ".meta";
		string sContent = File.ReadAllText(sPath);
		int i = sContent.IndexOf("messageOptions: 0");
		//replace ALL occurances and reassign (strings are immutable)
		sContent = sContent.Replace("messageOptions: 0","messageOptions: 1");
		Debug.Log(sContent);
		File.WriteAllText(sPath,sContent);
	}
}
