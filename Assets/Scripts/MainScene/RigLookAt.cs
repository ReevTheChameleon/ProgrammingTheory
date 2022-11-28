using UnityEngine;
using System;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RigLookAtSetting{ //struct wouldn't allow field initializer
	public Transform transform;
	[Range(0,1)] public float weight =1.0f;
	public bool bRigX,bRigY,bRigZ;
	[WideMode] public Vector2 eulerLimitX;
	[WideMode] public Vector2 eulerLimitY;
	[WideMode] public Vector2 eulerLimitZ;
	[NonSerialized] public Quaternion riggedRotation;
}
[CustomPropertyDrawer(typeof(RigLookAtSetting))]
class RigLookAtSettingDrawer : PropertyDrawer{
	private GUIContent contentLimit = new GUIContent("Limit");
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		bool bSavedGUIEnable = GUI.enabled;
		Rect rectOriginal = position;
		position.height = EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigLookAtSetting.transform)));
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigLookAtSetting.weight)));
		
		EditorGUIUtility.labelWidth = 50.0f;
		position.y += EditorGUIUtility.singleLineHeight;
		position.width = rectOriginal.width/3;
		SerializedProperty spRig = property.FindPropertyRelative(nameof(RigLookAtSetting.bRigX));
		EditorGUI.PropertyField(position,spRig);
		GUI.enabled = spRig.boolValue;
		position.x += position.width;
		position.width = rectOriginal.width-position.width;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigLookAtSetting.eulerLimitX)),contentLimit);
		GUI.enabled = bSavedGUIEnable;
		
		position.y += EditorGUIUtility.singleLineHeight;
		position.x = rectOriginal.x;
		position.width = rectOriginal.width/3;
		spRig = property.FindPropertyRelative(nameof(RigLookAtSetting.bRigY));
		EditorGUI.PropertyField(position,spRig);
		GUI.enabled = spRig.boolValue;
		position.x += position.width;
		position.width = rectOriginal.width-position.width;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigLookAtSetting.eulerLimitY)),contentLimit);
		GUI.enabled = bSavedGUIEnable;
		
		position.y += EditorGUIUtility.singleLineHeight;
		position.x = rectOriginal.x;
		position.width = rectOriginal.width/3;
		spRig = property.FindPropertyRelative(nameof(RigLookAtSetting.bRigZ));
		EditorGUI.PropertyField(position,spRig);
		GUI.enabled = spRig.boolValue;
		position.x += position.width;
		position.width = rectOriginal.width-position.width;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigLookAtSetting.eulerLimitZ)),contentLimit);
		GUI.enabled = bSavedGUIEnable;
	}
	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return EditorGUIUtility.singleLineHeight * 5;
	}
}

[Serializable]
public class RigLookAt{
	//public bool bAutoApply = true;
	public Transform tLookTarget;
	[Range(0,1)] public float weight =1.0f;
	[Tooltip("Must be ordered down the hierarchy for now. Will revise later")]
	public RigLookAtSetting[] aRigSetting;
	
	//#if UNITY_EDITOR
	//[SerializeField] bool bDrawRay;
	//#endif

	//void LateUpdate(){
	//	if(bAutoApply){
	//		calculateRig();
	//		for(int i=0; i<aRigSetting.Length; ++i)
	//			aRigSetting[i].transform.rotation = aRigSetting[i].rotation;
	//	}
		
	//	#if UNITY_EDITOR
	//	/* Do this outside main loop to draw ACTUAL result after ENTIRE loop is done
	//	(because result of next loop may alter rotation of previous loop,
	//	which I will fix later.) */
	//	if(bDrawRay)
	//		for(int i=0; i<aRigSetting.Length; ++i)
	//			Debug.DrawRay(aRigSetting[i].transform.position,aRigSetting[i].transform.forward);
	//	#endif
	//}
	public void calculateRig(bool bApply=false){
		for(int i=0; i<aRigSetting.Length; ++i)
			aRigSetting[i].riggedRotation = aRigSetting[i].transform.rotation;

		if(tLookTarget!=null && weight!=0.0f){
			for(int i=0; i<aRigSetting.Length; ++i){
				Transform tBone = aRigSetting[i].transform;
				Quaternion qLook = Quaternion.Lerp(
					aRigSetting[i].riggedRotation,
					Quaternion.LookRotation(tLookTarget.position-tBone.position),
					weight * aRigSetting[i].weight
				);
				Vector3 eulerAnglesDelta =
					(aRigSetting[i].riggedRotation.inverse() * qLook).eulerAngles;
				Quaternion qDeltaClamped = Quaternion.Euler(
					aRigSetting[i].bRigX ?
						MathfExtension.clampAngleDeg(
							eulerAnglesDelta.x,aRigSetting[i].eulerLimitX.x,aRigSetting[i].eulerLimitX.y) :
						0.0f
					,
					aRigSetting[i].bRigY ?
						MathfExtension.clampAngleDeg(
							eulerAnglesDelta.y,aRigSetting[i].eulerLimitY.x,aRigSetting[i].eulerLimitY.y) :
						0.0f
					,
					aRigSetting[i].bRigZ ?
						MathfExtension.clampAngleDeg(
							eulerAnglesDelta.z,aRigSetting[i].eulerLimitZ.x,aRigSetting[i].eulerLimitZ.y) :
						0.0f
				);
				aRigSetting[i].riggedRotation = aRigSetting[i].riggedRotation * qDeltaClamped;
			}
		}
		if(bApply){
			for(int i=0; i<aRigSetting.Length; ++i)
				aRigSetting[i].transform.rotation = aRigSetting[i].riggedRotation;
		}
	}
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public void drawLookRay(){
		for(int i=0; i<aRigSetting.Length; ++i)
			Debug.DrawRay(aRigSetting[i].transform.position,aRigSetting[i].transform.forward);
	}
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(RigLookAt))]
//class RigPlayerEditor : Editor{
//	RigLookAt targetAs;
//	void OnEnable(){
//		targetAs = (RigLookAt)target;
//	}
//	public override void OnInspectorGUI(){
		
//	}
//}
//#endif
