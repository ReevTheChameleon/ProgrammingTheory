/* This is quite ad hoc, and probably not worth including in UsefulScripts package
because usually you don't leave your Material as default. Actually you should not
have to port from Standard Render Pipeline to URP in the first place.
However, if that happens, this script can help somewhat */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public static class SwapDefaultMaterial{
	[MenuItem("Chameleon/Upgrade Material")]
	static void upgrade(){
		Material matDefaultStandard =
			AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
		GameObject gTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Material matDefaultLitURP = gTemp.GetComponent<Renderer>().sharedMaterial;
		Object.DestroyImmediate(gTemp);

		#if !UNITY_2020_3_OR_NEWER
		foreach(GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
			foreach(Renderer r in g.GetComponentsInChildren<Renderer>(true))
				if(r.sharedMaterial == matDefaultStandard)
					r.sharedMaterial = matDefaultLitURP;
		#endif
		foreach(Renderer r in Object.FindObjectsOfType<Renderer>(true))
			if(r.sharedMaterial == matDefaultStandard)
				r.sharedMaterial = matDefaultLitURP;
	}
}

#endif
