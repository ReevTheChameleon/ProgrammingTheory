//using UnityEngine;
//using Chameleon;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//public class DoorBase : MonoBehaviour{
//	#if UNITY_EDITOR
//	[SerializeField] RoomScaler roomScaler;
	
//	[CustomEditor(typeof(DoorBase))]
//	class DoorBaseEditor : Editor{
//		public override void OnInspectorGUI(){
//			DrawDefaultInspector();
//			if(GUILayout.Button("Sync roomScaler")){
//				Transform transform = ((DoorBase)target).transform;
//				RoomScaler roomScaler = ((DoorBase)target).roomScaler;
//				transform.transform.localScale = new Vector3(
//					roomScaler.

//			}
//		}
//	}
//	#endif
//}
