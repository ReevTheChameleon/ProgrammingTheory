using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TestAddEvent{
	[MenuItem("CONTEXT/AnimationClip/AddEvent")]
	public static void addEvent(MenuCommand menuCommand){
		AnimationClip clip = (AnimationClip)menuCommand.context;
		AnimationEvent animEvent = new AnimationEvent();
		animEvent.time = clip.length;
		animEvent.functionName = "onEnd";
		AnimationUtility.SetAnimationEvents(clip,new AnimationEvent[]{animEvent});
	}
}

public class TestEndEvent : MonoBehaviour{
	void onEnd(){
		Debug.Log("End");
	}
}
