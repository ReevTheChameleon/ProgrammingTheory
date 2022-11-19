using UnityEngine;
using TMPro;
using Chameleon;
using System.Collections;
using System.Collections.Generic;

public class FooterManager : LoneMonoBehaviour<FooterManager>{
	[SerializeField] TextMeshProUGUI txtFooter;
	[SerializeField][GrayOnPlay] Vector2 v2AnchoredPosShow;
	[SerializeField][GrayOnPlay] Vector2 v2AnchoredPosHide;
	[SerializeField][GrayOnPlay] float footerTransitionTime;
	[SerializeField][GrayOnPlay] float typewriteSpeed;
	[SerializeField] GameObject gContinue;
	[SerializeField] float cooldownSkip;
	
	RectTransform rtFooter;
	private LoneCoroutine routineFooter = new LoneCoroutine();
	private TweenRoutineUnit subitrTweenInFooter;
	private TypewriteRoutineUnit subitrTypewrite;
	private Trigger triggerSkip = new Trigger();
	
	public bool IsShowing{get; private set;} = false;
	public bool IsDone{ get{return !routineFooter.IsRunning;} }

	void Start(){
		/* Need to initialize here to be sure that all images sizes are already set. */
		rtFooter = (RectTransform)transform;
		v2AnchoredPosShow = rtFooter.anchoredPosition;
		v2AnchoredPosHide = v2AnchoredPosShow;
		v2AnchoredPosHide.y -= rtFooter.rect.height;
		rtFooter.anchoredPosition = v2AnchoredPosHide;
		subitrTweenInFooter = rtFooter.tweenAnchoredPosition(
			v2AnchoredPosHide,
			v2AnchoredPosShow,
			footerTransitionTime,
			dMapping: (float t)=>{return Mathf.SmoothStep(0.0f,1.0f,t);}
		);
		subitrTypewrite = txtFooter.typewrite(typewriteSpeed);
	}
	private IEnumerator rfShowFooter(List<string> lText,int textCount=-1){
		IsShowing = true;
		txtFooter.text = "";
		subitrTweenInFooter.bReverse = false;
		gContinue.SetActive(false);
		yield return subitrTweenInFooter;

		Cooldown cooldown = new Cooldown();
		for(int i=0; i<lText.Count; ++i){
			subitrTypewrite.Text = lText[i];
			cooldown.set(cooldownSkip);
			while(subitrTypewrite.MoveNext()){
				yield return subitrTypewrite.Current;
				if(triggerSkip && !cooldown){
					triggerSkip.clear();
					subitrTypewrite.skip();
					break;
				}
			}
			cooldown.set(cooldownSkip);
			while(cooldown)
				yield return null;
			gContinue.SetActive(true);
			if(i==lText.Count-1) //for last text, no need to wait for skip
				yield break;
			/* This is for the best, because stopping and resuming ALLOCATES
			more memory on StartCoroutine, while this check, although done
			every frame, eats very small performance. */
			while(!triggerSkip)
				yield return null;
			gContinue.SetActive(false);
			triggerSkip.clear();
		}
		//hideFooter();
	}
	public void showFooter(List<string> lText){
		routineFooter.start(this,rfShowFooter(lText));
	}
	public void hideFooter(){
		IsShowing = false;
		subitrTweenInFooter.bReverse = true;
		routineFooter.start(this,subitrTweenInFooter);
	}
	public void stepFooter(){
		triggerSkip.set();
	}
}
