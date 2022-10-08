using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
			if(Mathf.Abs(deltaCameraDistance) > epsilon)
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

	#if UNITY_EDITOR
	[CustomEditor(typeof(ThirdPersonCameraControl))]
	class ThirdPersonCameraControlEditor : Editor{
		private ThirdPersonCameraControl targetAs;
		private bool bFoldout = false;
		void OnEnable(){
			targetAs = (ThirdPersonCameraControl)target;
		}
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			bFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(bFoldout,"Info");
			EditorGUILayout.EndFoldoutHeaderGroup();
			GUI.enabled = false;
			if(bFoldout){
				EditorGUILayout.Vector3Field("vLook",targetAs.vLook);
				EditorGUILayout.Vector3Field("qLook",targetAs.qLook.eulerAngles);
				EditorGUILayout.FloatField("cameraDistance",targetAs.cameraDistance);
				EditorGUILayout.Toggle("bOccluded",targetAs.bOccluded);
				EditorGUILayout.FloatField("occludedDistance",targetAs.occludedDistance);
			}
		}
	}
	#endif
}

} //end namespace Chameleon
