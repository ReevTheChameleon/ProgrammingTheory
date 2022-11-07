using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour{
	[Bakable] static float fadeTime = 0.25f;
	CanvasGroup canvasGroup;
	private LoneCoroutine routineFade;

	void Awake(){
		canvasGroup = GetComponent<CanvasGroup>();
		routineFade = new LoneCoroutine(
			this,
			canvasGroup.tweenAlpha(
				0.0f,1.0f,fadeTime,
				dOnDone:(float t)=>{
					if(routineFade.getItr<TweenRoutineUnit>().bReverse)
						canvasGroup.gameObject.SetActive(false);
				}
			)
		);
	}
	public void fade(bool bIn){
		canvasGroup.gameObject.SetActive(true);
		routineFade.getItr<TweenRoutineUnit>().bReverse = !bIn;
		routineFade.resume();
	}
}

[CustomEditor(typeof(CanvasGroupFader))]
class CanvasGroupFaderEditor: MonoBehaviourBakerEditor{}
