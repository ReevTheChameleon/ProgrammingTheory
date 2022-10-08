using UnityEngine;

namespace Chameleon{

public class ThirdPersonCameraControl : MonoBehaviour{
	Camera targetCamera;
	[SerializeField] float epsilon =0.0001f;
	
	[Header("Target")]
	[SerializeField] Transform tTarget;
	[SerializeField] float moveDamping;
	
	[Header("Zoom")]
	[Min(0.0f)] public float targetCameraDistance;
	[SerializeField] float zoomDamping;
	[SerializeField] LayerMask obstacleLayer;
	
	private Vector3 vLook; //world space
	private Quaternion qLook;
	private float cameraDistance;
	private bool bOccluded;
	private float occludedDistance;

	void Awake(){
		targetCamera = GetComponent<Camera>();
	}
	void Start(){
		vLook = tTarget.position;
		qLook = tTarget.rotation;
		cameraDistance = targetCameraDistance;
		transform.position = vLook-tTarget.forward*cameraDistance;
	}
	void LateUpdate(){
		if(moveDamping != 0.0f){
			Vector3 vDelta = tTarget.transform.position-vLook;
			vDelta = tTarget.rotation*Quaternion.Inverse(qLook)*vDelta;
			float vDeltaMagnitude = vDelta.magnitude;
			if(vDeltaMagnitude > epsilon){
				vDelta -= vDelta * vDeltaMagnitude*Time.deltaTime/moveDamping;
				vLook = tTarget.transform.position-vDelta;
			}
		}
		else
			vLook = tTarget.transform.position;
		qLook = tTarget.rotation;
		
		if(zoomDamping != 0.0f){
			float deltaCameraDistance = targetCameraDistance-cameraDistance;
			if(deltaCameraDistance > epsilon)
				cameraDistance += deltaCameraDistance*Time.deltaTime/zoomDamping;
		}
		else
			cameraDistance = targetCameraDistance;

		if(bOccluded && occludedDistance<cameraDistance)
			cameraDistance = occludedDistance;

		transform.position = vLook-tTarget.forward*cameraDistance;
		transform.rotation = qLook;
	}
	void FixedUpdate(){
		if(obstacleLayer == 0)
			return;
		RaycastHit hitInfo;
		if(Physics.Raycast(
			vLook,
			transform.position-vLook,
			out hitInfo,
			targetCamera ? targetCamera.farClipPlane : 5000.0f,
			obstacleLayer
		)){
			bOccluded = true;
			occludedDistance = hitInfo.distance;
		}
		else
			bOccluded = false;
	}
}

} //end namespace Chameleon
