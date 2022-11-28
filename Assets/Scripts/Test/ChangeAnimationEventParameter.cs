using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//public class TestAnimationEvent : MonoBehaviour{
//	public AnimationEvent ae;
//	public AnimationClip clip;
//}

//#if UNITY_EDITOR
//[CustomEditor(typeof(TestAnimationEvent))]
//class TestAnimationEventEditor : Editor{
//	TestAnimationEvent targetAs;
//	void OnEnable(){
//		targetAs = (TestAnimationEvent)target;
//	}
//	public override void OnInspectorGUI() {
//		DrawDefaultInspector();
//		if(GUILayout.Button("Click")){
//			Debug.Log(AnimationUtility.GetAnimationEvents(targetAs.clip)[0].intParameter);
//			AnimationUtility.GetAnimationEvents(targetAs.clip)[0].intParameter = 3;
//		}
//	}
//}
//#endif
using System.Linq;

public class ChangeAnimationEventParameter : MonoBehaviour{
    [SerializeField] AnimationClip animationClip = null;
    void Start(){
		Debug.Log(animationClip.events[0].intParameter);
        AnimationEvent[] animEvent = animationClip.events;
		animEvent[0].intParameter = 2;
        animationClip.events = animEvent;
		Debug.Log(animationClip.events[0].intParameter);
    }
}
