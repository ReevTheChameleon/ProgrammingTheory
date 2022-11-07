using UnityEngine;

public class Billboard : MonoBehaviour{
	[SerializeField] float distanceDefault;
	private Transform tCamera;

	void Awake(){
		tCamera = Camera.main.transform;
	}
	void LateUpdate(){
		transform.LookAt(tCamera);
		transform.localScale = 
			Vector3.one * Vector3.Distance(transform.position,tCamera.position)/distanceDefault;
	}
}
