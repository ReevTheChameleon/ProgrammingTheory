
using UnityEngine;
using Chameleon;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class RigFixedSetting{
	public Transform transform;
	public TransformData tdTarget;
	public RigFixedSetting(Transform t){
		transform = t;
		tdTarget = t.save();
	}
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RigFixedSetting))]
class RigFixedSettingDrawer : PropertyDrawer{
	/* At the moment, there is flickering when TransformData is expanded in the list
	and Inspector Panel is dragged. Not sure why this is happening, but may look into it later. */
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUIUtility.wideMode = true;
		position.height = EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position,property.FindPropertyRelative(nameof(RigFixedSetting.transform)));
		SerializedProperty spTdTarget = property.FindPropertyRelative(nameof(RigFixedSetting.tdTarget));
		EditorGUI.PropertyField(position,spTdTarget,GUIContent.none,spTdTarget.isExpanded);
	}
	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		SerializedProperty spTdTarget = property.FindPropertyRelative(nameof(RigFixedSetting.tdTarget));
		return EditorGUIUtility.singleLineHeight *
			(spTdTarget.isExpanded ? 7 : 1);
	}
}
#endif

public class RigFixed : MonoBehaviour{
	[Range(0,1)] public float weight;
	public List<RigFixedSetting> lRigSetting;

	void LateUpdate(){
		applyRig();
	}
	public void applyRig(){
		if(weight==0.0f){
			return;}
		for(int i=0; i<lRigSetting.Count; ++i){
			if(weight==1.0f){
				lRigSetting[i].transform.load(lRigSetting[i].tdTarget);}
			else{
				lRigSetting[i].transform.lerpUnclamped(
					lRigSetting[i].transform.save(),
					lRigSetting[i].tdTarget,
					weight
				);
			}
		}
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(RigFixed))]
class RigFixedEditor : Editor{
	private RigFixed targetAs;
	private bool bPreview = false;
	void OnEnable(){
		targetAs = (RigFixed)target;
	}
	public override void OnInspectorGUI(){
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(RigFixed.weight)));
		/* This makes sure drag-drop event is captured before the List's header captured
		and consumed it. */
		Rect rect = GUILayoutUtility.GetLastRect();
		rect.y += EditorGUIUtility.singleLineHeight;
		Object[] aDropped = EditorHelper.dropZone<Transform>(rect);
		for(int i=0; i<aDropped?.Length; ++i){
			Transform t = aDropped[i] as Transform;
			if(!t){
				continue;}
			targetAs.lRigSetting.Add(new RigFixedSetting(t));
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(RigFixed.lRigSetting)));
		serializedObject.ApplyModifiedProperties();
		bPreview = GUILayout.Toggle(bPreview,"Preview",new GUIStyle(GUI.skin.button));
		if(bPreview){
			targetAs.applyRig();}
	}
}
#endif
