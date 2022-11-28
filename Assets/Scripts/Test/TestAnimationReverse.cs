using UnityEngine;
using Chameleon;

public class TestAnimationReverse : MonoBehaviour{
	[SerializeField] AnimationClip clipEvent;
	private PlayableController p;
	public double speed;

	private void funcEvent(){
		Debug.Log("Yeah");
	}
	void Start(){
		p = GetComponent<AnimationPlayer>().play(clipEvent);
		p.setSpeed(1.0);
	}
	#if UNITY_EDITOR
	void OnValidate(){
		p?.setSpeed(speed);
	}
	#endif
}
