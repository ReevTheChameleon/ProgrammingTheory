using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Chameleon{

public enum eUpdateMethod{FixedUpdate,LateUpdate,}

public class ThirdPersonCameraControl : MonoBehaviour{
	[Header("Camera")]
	Camera targetCamera;
	[Min(0.000001f)] public float epsilon =0.0001f;
	[Min(0.0f)] public float cameraRadius =0.2f;

	[Header("Target")]
	public Transform tTarget;
	[Tooltip("Choose FixedUpdate if your character position is modified by Physics (like modified by collision)")]
	public eUpdateMethod followUpdateMethod =eUpdateMethod.FixedUpdate;
	[Min(0.0f)] public float followDamping;
	[Tooltip("Choose FixedUpdate if your character rotation is modified by Physics")]
	public eUpdateMethod lookUpdateMethod =eUpdateMethod.LateUpdate;
	[Min(0.0f)] public float lookDamping;
	
	[Header("Zoom")]
	[Min(0.0f)] public float targetCameraDistance;
	[Min(0.0f)] public float zoomDamping;
	public LayerMask obstacleLayer;
	
	private Vector3 vLook; //world space
	private Quaternion qLook;
	private float cameraDistance;

	void Awake(){
		targetCamera = GetComponent<Camera>();
	}
	void Start(){
		vLook = tTarget.position;
		qLook = tTarget.rotation;
		cameraDistance = targetCameraDistance;
		transform.position = vLook-tTarget.forward*cameraDistance;
	}
	private void updateFollow(float deltaTime){
		Vector3 vDelta = tTarget.position-vLook;
		float vDeltaMagnitude = vDelta.magnitude;
		if(vDeltaMagnitude < epsilon)
			return;
		if(followDamping != 0.0f){
			float t = interpolate(deltaTime,lookDamping);
			vDelta = (1-t)*vDelta;
			vLook = tTarget.position-vDelta;
		}
		else
			vLook = tTarget.position;
	}
	private void updateLook(float deltaTime){
		//if(Quaternion.Dot(qLook,tTarget.rotation) > 1.0f-epsilon) //Credit: falstro, gamedev.stackexchange
		//	return;
		if(qLook == tTarget.rotation) //equivalent to code above with epsilon 0.000001f
			return;
		Vector3 vDelta = tTarget.position-vLook;
		vDelta = Quaternion.Inverse(qLook)*vDelta;
		/* The really correct way to lerp Quaternion is to use slerp, (Credit: Luke Hutchison, math.stackexchange)
		but it shouldn't matter much if delta is small. */
		qLook =
			lookDamping==0.0f ? 
			tTarget.rotation :
			Quaternion.Lerp(qLook,tTarget.rotation,interpolate(deltaTime,lookDamping)) //clamped to [0,1]
		;
		vDelta = qLook*vDelta;
		vLook = tTarget.position-vDelta;
	}
	private void updateZoom(float deltaTime){
		float deltaCameraDistance = targetCameraDistance-cameraDistance;
		if(Mathf.Abs(deltaCameraDistance) > epsilon){
			float t = interpolate(deltaTime,followDamping);
			//float t = Mathf.Clamp01(deltaTime/followDamping);
			cameraDistance =
				zoomDamping == 0.0f ?
				targetCameraDistance :
				cameraDistance + t*deltaCameraDistance
			;
		}
		float occlusionDistance = getOcclusionDistance();
		if(occlusionDistance < cameraDistance)
			cameraDistance = occlusionDistance;
	}
	private float getOcclusionDistance(){
		if(obstacleLayer == 0)
			return Mathf.Infinity;
		float raycastDistance = Mathf.Min(
			cameraDistance,
			targetCamera ? targetCamera.farClipPlane : 5000.0f
		);
		RaycastHit hitInfo;
		if(Physics.SphereCast(
			vLook,
			cameraRadius,
			transform.position-vLook,
			out hitInfo,
			raycastDistance,
			obstacleLayer
		)){
			return hitInfo.distance;
		}
		return Mathf.Infinity;
	}
	private float interpolate(float deltaTime,float damping){
		/* Exponential decay is completely independent of framerate.
		Linear is approximation of it using instantaneous slope, so can deviate a little. */
		//return MathfExtension.exponentialDecay(0,1,deltaTime,1/damping);
		return Mathf.Clamp01(deltaTime/damping);
	}
	void FixedUpdate(){
		if(lookUpdateMethod == eUpdateMethod.FixedUpdate)
			updateLook(Time.fixedDeltaTime);
		if(followUpdateMethod == eUpdateMethod.FixedUpdate)
			updateFollow(Time.fixedDeltaTime);
	}
	void LateUpdate(){
		if(lookUpdateMethod == eUpdateMethod.LateUpdate)
			updateLook(Time.deltaTime);
		if(followUpdateMethod == eUpdateMethod.LateUpdate)
			updateFollow(Time.deltaTime);
		updateZoom(Time.deltaTime); //zoom is always in LateUpdate() (for now) because it is input-based
		
		//Apply update result in LateUpdate()
		transform.position = vLook-qLook*Vector3.forward*cameraDistance;
		transform.rotation = qLook;
	}

	//#if UNITY_EDITOR
	//[CustomEditor(typeof(ThirdPersonCameraControl))]
	//class ThirdPersonCameraControlEditor : Editor{
	//	private ThirdPersonCameraControl targetAs;
	//	private bool bFoldout = false;
	//	void OnEnable(){
	//		targetAs = (ThirdPersonCameraControl)target;
	//	}
	//	public override void OnInspectorGUI() {
	//		DrawDefaultInspector();
	//		bFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(bFoldout,"Info");
	//		EditorGUILayout.EndFoldoutHeaderGroup();
	//		GUI.enabled = false;
	//		if(bFoldout){
	//			EditorGUILayout.Vector3Field("vLook",targetAs.vLook);
	//			EditorGUILayout.Vector3Field("qLook",targetAs.qLook.eulerAngles);
	//			EditorGUILayout.FloatField("cameraDistance",targetAs.cameraDistance);
	//			EditorGUILayout.Toggle("bOccluded",targetAs.bOccluded);
	//			EditorGUILayout.FloatField("occludedDistance",targetAs.occludedDistance);
	//		}
	//	}
	//}
	//#endif
}

} //end namespace Chameleon
