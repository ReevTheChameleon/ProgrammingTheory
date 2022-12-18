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
	public Quaternion RiggedRotation{get; internal set;}
}

#if UNITY_EDITOR
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
#endif

//[Serializable]
public class RigLookAt : MonoBehaviour{
	//public bool bAutoApply = true;
	public Transform tLookTarget;
	[Range(0,1)] public float weight =1.0f;
	public Vector3 eulerOffset;
	[Tooltip("Must be ordered down the hierarchy for now. Will revise later")]
	public RigLookAtSetting[] aRigSetting;
	
	#if UNITY_EDITOR
	[SerializeField] bool bDrawRay;
	#endif

	void LateUpdate(){
		calculateRig(true);
		
		#if UNITY_EDITOR
		/* Do this outside main loop to draw ACTUAL result after ENTIRE loop is done
		(because result of next loop may alter rotation of previous loop,
		which I will fix later.) */
		if(bDrawRay)
			for(int i=0; i<aRigSetting.Length; ++i)
				Debug.DrawRay(aRigSetting[i].transform.position,aRigSetting[i].transform.forward);
		#endif
	}
	/* Calculation result is stored in aRigSettings[i].RiggedRotation */
	public void calculateRig(bool bApply=false){
		for(int i=0; i<aRigSetting.Length; ++i)
			aRigSetting[i].RiggedRotation = aRigSetting[i].transform.rotation;

		if(tLookTarget!=null && weight!=0.0f){
			for(int i=0; i<aRigSetting.Length; ++i){
				Transform tBone = aRigSetting[i].transform;
				Quaternion qLook = Quaternion.Lerp(
					aRigSetting[i].RiggedRotation,
					Quaternion.LookRotation(tLookTarget.position-tBone.position),
					weight * aRigSetting[i].weight
				);
				Vector3 eulerAnglesDelta =
					(aRigSetting[i].RiggedRotation.inverse() * qLook).eulerAngles
					+ eulerOffset
				;
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
				aRigSetting[i].RiggedRotation = aRigSetting[i].RiggedRotation * qDeltaClamped;
			}
		}
		if(bApply){
			for(int i=0; i<aRigSetting.Length; ++i)
				aRigSetting[i].transform.rotation = aRigSetting[i].RiggedRotation;
		}
	}
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public void drawLookRay(){
		for(int i=0; i<aRigSetting.Length; ++i)
			Debug.DrawRay(aRigSetting[i].transform.position,aRigSetting[i].transform.forward);
	}
}
