using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Billboard : MonoBehaviour{
	private Transform tCamera;

	void Awake(){
		tCamera = Camera.main.transform;
	}
	void LateUpdate(){
		transform.lookDirection(-tCamera.forward,tCamera.up);
		//transform.rotation = Quaternion.LookRotation(-tCamera.forward,tCamera.up);
		//transform.LookAt(tCamera,tCamera.up); //this method distort if near edge
		//if(playerUpdateMethod == eUpdateMethod.LateUpdate)
		//	transform.localScale = 
		//		Vector3.one * Vector3.Distance(transform.position,tPlayer.position)/distanceDefault;
	}
}
