using UnityEngine;
using Chameleon;
using UnityEngine.InputSystem;

public class TestAnimationLayer : MonoBehaviour{
	AnimationPlayer animPlayer;
	public AnimationClip clip1;
	public AnimationClip clip2;
	public AnimationClip clip3;
	public AvatarMask mask2;

	void Awake(){
		animPlayer = GetComponent<AnimationPlayer>();
		animPlayer.addLayer(1,mask2,true);
	}
	void Start(){
		animPlayer.play(clip1);
		animPlayer.play(clip2,1);
	}
	void Update(){
		if(Keyboard.current.spaceKey.wasPressedThisFrame){
			animPlayer.play(clip3); }
	}
}
