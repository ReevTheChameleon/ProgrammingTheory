using UnityEngine;
using System.Collections;
using Chameleon;

public class TestTrigger : MonoBehaviour{
	[SerializeField] CanvasGroup canvasGroupBalloon;
	[SerializeField][Tag] string tagPlayer;
	[SerializeField] float fadeTime;
	private LoneCoroutine routineFade;

	void Awake(){
		routineFade = new LoneCoroutine(
			this,
			canvasGroupBalloon.tweenAlpha(
				0.0f,1.0f,
				fadeTime,
				dOnDone:(float t)=>{
					if(routineFade.getItr<TweenRoutineUnit>().bReverse)
						canvasGroupBalloon.gameObject.SetActive(false);
				}
			)
		);
	}
	void OnTriggerEnter(Collider other){
		if(other.CompareTag(tagPlayer)){
			routineFade.getItr<TweenRoutineUnit>().bReverse = false;
			routineFade.resume();
			canvasGroupBalloon.gameObject.SetActive(true);
		}
	}
	void OnTriggerExit(Collider other){
		routineFade.getItr<TweenRoutineUnit>().bReverse = true;
		routineFade.resume();
	}
}
