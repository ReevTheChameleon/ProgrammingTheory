#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Chameleon;

[RemoveOnBuild]
public class TransformRevealer : MonoBehaviour{}

[CustomEditor(typeof(TransformRevealer))]
class TransformRevealerEditor : Editor{
	private TransformRevealer targetAs;
	private bool bLocal;
	private bool bRectTransformRelative;

	void OnEnable(){
		targetAs = target as TransformRevealer;
		bLocal = Tools.pivotRotation==PivotRotation.Local;
	}
	public override void OnInspectorGUI(){
		if(!EditorGUIUtility.wideMode)
			EditorGUIUtility.wideMode = true;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Transform",EditorStyles.boldLabel);
		if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
			GenericMenu contextMenu = new GenericMenu();
			contextMenu.AddItem(
				new GUIContent("Reset (&R)"),
				false,
				()=>{
					Undo.RecordObject(targetAs.transform,"Reset Transform");
					if(bLocal){
						targetAs.transform.localPosition = Vector3.zero;
						targetAs.transform.localRotation = Quaternion.identity;
						targetAs.transform.localScale = Vector3.one;
					}
					else{
						targetAs.transform.position = Vector3.zero;
						targetAs.transform.rotation = Quaternion.identity;
						targetAs.transform.localScale = Vector3.one;
					}
				}
			);
			contextMenu.ShowAsContext();
		}
		if(bLocal && GUILayout.Button("In Local Coordinate"))
			bLocal = false;
		else if(!bLocal && GUILayout.Button("In Global Coordinate"))
			bLocal = true;
		EditorGUILayout.EndHorizontal();

		++EditorGUI.indentLevel;
		Vector3 vTemp;
		if(bLocal){
			/* I wanted to group this into a function, but properties cannot be passed as
			ref arguments, so there is no choice but to write them all explicitly.
			This complication is so that it can be undoable. One can use PropertyField, but
			that doesn't allow for custom modification like showing rotation as quaternion later. */
			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Local Position",
				targetAs.transform.localPosition
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.localPosition = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.localPosition)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.localPosition = Vector3.zero;
					}
				);
				contextMenu.ShowAsContext();
			}

			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Local Rotation",
				targetAs.transform.localEulerAngles
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.localEulerAngles = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.localRotation)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.localRotation = Quaternion.identity;
					}
				);
				contextMenu.ShowAsContext();
			}

			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Local Scale",
				targetAs.transform.localScale
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.localScale = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.localScale)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.localScale = Vector3.one;
					}
				);
				contextMenu.ShowAsContext();
			}
		}
		else{ //!bLocal
			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Position",
				targetAs.transform.position
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.position = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.position)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.position = Vector3.zero;
					}
				);
				contextMenu.ShowAsContext();
			}

			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Rotation",
				targetAs.transform.eulerAngles
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.eulerAngles = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.rotation)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.rotation = Quaternion.identity;
					}
				);
				contextMenu.ShowAsContext();
			}

			EditorGUI.BeginChangeCheck();
			vTemp = EditorGUILayout.Vector3Field(
				"Local Scale",
				targetAs.transform.localScale
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change Transform");
				targetAs.transform.localScale = vTemp;
			}
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(Transform.localScale)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change Transform");
						targetAs.transform.localScale = Vector3.one;
					}
				);
				contextMenu.ShowAsContext();
			}
		}
		--EditorGUI.indentLevel;

		if(targetAs.transform is RectTransform){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RectTransform",EditorStyles.boldLabel);
			if(EditorHelper.contextClicked(GUILayoutUtility.GetLastRect())){
				GenericMenu contextMenu = new GenericMenu();
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Reset RectTransform");
						((RectTransform)targetAs.transform).anchorMin = Vector2.zero;
						((RectTransform)targetAs.transform).anchorMax = Vector2.one;
						((RectTransform)targetAs.transform).anchoredPosition = Vector2.zero;
						((RectTransform)targetAs.transform).sizeDelta = Vector2.zero;
						((RectTransform)targetAs.transform).pivot = new Vector2(0.5f,0.5f);
					}
				);
				contextMenu.ShowAsContext();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight*5.0f);
			Rect rectField = GUILayoutUtility.GetLastRect();
			float singleLineHeight = EditorGUIUtility.singleLineHeight;
			float labelWidth = EditorGUIUtility.labelWidth;
			rectField.height = singleLineHeight;
			rectField.width = (rectField.width-labelWidth)*3/2 + labelWidth;
			
			++EditorGUI.indentLevel;
			Vector2 v2Temp;
			EditorGUI.BeginChangeCheck();
			v2Temp = EditorGUI.Vector2Field(
				rectField,
				"Anchor Min",
				((RectTransform)targetAs.transform).anchorMin
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change RectTransform");
				((RectTransform)targetAs.transform).anchorMin = v2Temp;
			}
			if(EditorHelper.contextClicked(rectField)){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(RectTransform.anchorMin)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change RectTransform");
						((RectTransform)targetAs.transform).anchorMin = Vector2.zero;
					}
				);
				contextMenu.ShowAsContext();
			}

			rectField.y += singleLineHeight;
			EditorGUI.BeginChangeCheck();
			v2Temp = EditorGUI.Vector2Field(
				rectField,
				"Anchor Max",
				((RectTransform)targetAs.transform).anchorMax
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change RectTransform");
				((RectTransform)targetAs.transform).anchorMax = v2Temp;
			}
			if(EditorHelper.contextClicked(rectField)){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(RectTransform.anchorMax)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change RectTransform");
						((RectTransform)targetAs.transform).anchorMax = Vector2.zero;
					}
				);
				contextMenu.ShowAsContext();
			}
			
			rectField.y += singleLineHeight;
			EditorGUI.BeginChangeCheck();
			if(bRectTransformRelative){
			}
			else{
				v2Temp = EditorGUI.Vector2Field(
					rectField,
					"Anchored Position",
					((RectTransform)targetAs.transform).anchoredPosition
				);
				if(EditorGUI.EndChangeCheck()){
					Undo.RecordObject(targetAs.transform,"Change RectTransform");
					((RectTransform)targetAs.transform).anchoredPosition = v2Temp;
				}
				if(EditorHelper.contextClicked(rectField)){
					GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
						targetAs.transform,
						nameof(RectTransform.anchoredPosition)
					);
					contextMenu.AddSeparator("");
					contextMenu.AddItem(
						new GUIContent("Reset (&R)"),
						false,
						()=>{
							Undo.RecordObject(targetAs.transform,"Change RectTransform");
							((RectTransform)targetAs.transform).anchoredPosition = Vector2.zero;
						}
					);
					contextMenu.ShowAsContext();
				}
			}
			
			rectField.y += singleLineHeight;
			EditorGUI.BeginChangeCheck();
			v2Temp = EditorGUI.Vector2Field(
				rectField,
				"Size Delta",
				((RectTransform)targetAs.transform).sizeDelta
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change RectTransform");
				((RectTransform)targetAs.transform).sizeDelta = v2Temp;
			}
			if(EditorHelper.contextClicked(rectField)){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(RectTransform.sizeDelta)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change RectTransform");
						((RectTransform)targetAs.transform).sizeDelta = Vector2.zero;
					}
				);
				contextMenu.ShowAsContext();
			}
			
			rectField.y += singleLineHeight;
			EditorGUI.BeginChangeCheck();
			v2Temp = EditorGUI.Vector2Field(
				rectField,
				"Pivot",
				((RectTransform)targetAs.transform).pivot
			);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(targetAs.transform,"Change RectTransform");
				((RectTransform)targetAs.transform).pivot = v2Temp;
			}
			if(EditorHelper.contextClicked(rectField)){
				GenericMenu contextMenu = EditorHelper.createCopyPasteContextMenu(
					targetAs.transform,
					nameof(RectTransform.pivot)
				);
				contextMenu.AddSeparator("");
				contextMenu.AddItem(
					new GUIContent("Reset (&R)"),
					false,
					()=>{
						Undo.RecordObject(targetAs.transform,"Change RectTransform");
						((RectTransform)targetAs.transform).pivot = new Vector2(0.5f,0.5f);
					}
				);
				contextMenu.ShowAsContext();
			}
			--EditorGUI.indentLevel;
		}
	}
}
#endif
