using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Chameleon;
using System.Reflection;

public class TestPlayableReflection : MonoBehaviour{
	void Start(){
		foreach(var info in typeof(
			AnimationLayerMixerPlayable).GetProperties(ReflectionHelper.BINDINGFLAGS_ALL))
			Debug.Log(info.Name);
		foreach(var info in typeof(
			AnimationLayerMixerPlayable).GetFields(ReflectionHelper.BINDINGFLAGS_ALL))
			Debug.Log(info.Name);
	}
}
