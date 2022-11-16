//using UnityEngine;
//using System.Collections;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using Chameleon;

//public static class SceneViewRotator{
//	private static double startTime;
//	private static Quaternion qStart;
//	private static Quaternion qEnd;
//	private static float lerpTime = 0.3f;
//	private static bool bInProgress = false;
//	public static void lerpTo(Quaternion in_qEnd){
//		if(!bInProgress){
//			bInProgress = true;
//			startTime = EditorApplication.timeSinceStartup;
//			qStart = SceneView.lastActiveSceneView.rotation;
//			qEnd = in_qEnd;
//			EditorApplication.update += update;
//		}
//	}
//	public static void update(){
//		float t = (float)(EditorApplication.timeSinceStartup-startTime)/lerpTime;
//		if(t > 1.0f){
//			EditorApplication.update -= update;
//			bInProgress = false;
//			return;
//		}
//		SceneView.lastActiveSceneView.rotation = Quaternion.Lerp(qStart,qEnd,t);
//	}
//	[MenuItem("Chameleon/SceneView/Top View")]
//	public static void topView(){
//		lerpTo(QuaternionExtension.down);
//	}
//	[MenuItem("Chameleon/SceneView/Bottom View")]
//	public static void BottomView(){
//		lerpTo(QuaternionExtension.up);
//	}
//	[MenuItem("Chameleon/SceneView/Front View")]
//	public static void frontView(){
//		/* For most 3D apps, front view means looking forward, which is
//		inconsistent with top and side view, but that is how it is (Credit: Regnas, blender.community) */
//		lerpTo(Quaternion.identity);
//	}
//	[MenuItem("Chameleon/SceneView/Back View")]
//	public static void backView(){
//		/* For most 3D apps, front view means looking forward, which is
//		inconsistent with top and side view, but that is how it is (Credit: Regnas, blender.community) */
//		lerpTo(QuaternionExtension.back);
//	}
//	[MenuItem("Chameleon/SceneView/Side View")]
//	public static void rightView(){
//		lerpTo(QuaternionExtension.left);
//	}
//	[MenuItem("Chameleon/SceneView/Side View")]
//	public static void leftView(){
//		lerpTo(QuaternionExtension.right);
//	}
//	[MenuItem("Chameleon/SceneView/Toggle Orthographic Camera")]
//	public static void toggleOrthographicCamera(){
//		SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
//	}
//}

//public class TestSceneView : MonoBehaviour{}

//#if UNITY_EDITOR
//[CustomEditor(typeof(TestSceneView))]
//class TestSceneViewEditor : Editor{
//	private TestSceneView targetAs;

//	void OnEnable(){
//		targetAs = target as TestSceneView;
//	}
//	public override void OnInspectorGUI(){
//		DrawDefaultInspector();
//		if(GUILayout.Button("Test")){
//			((MonoBehaviour)target).transform.rotation =
//				QuaternionExtension.down;
//		}
//	}
//}
//#endif
