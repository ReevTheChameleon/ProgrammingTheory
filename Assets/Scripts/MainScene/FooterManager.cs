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
	
	RectTransform rtFooter;
	private LoneCoroutine routineFooter = new LoneCoroutine();
	private TweenRoutineUnit subitrTweenInFooter;
	private TypewriteRoutineUnit subitrTypewrite;
	private Trigger triggerSkip = new Trigger();
	
	public bool IsShowing{get; private set;} = false;
	public bool IsDone{ get{return !routineFooter.IsRunning;} }
	protected override void Awake(){
		base.Awake();
		rtFooter = (RectTransform)transform;
		subitrTweenInFooter = rtFooter.tweenAnchoredPosition(
			v2AnchoredPosHide,
			v2AnchoredPosShow,
			footerTransitionTime,
			dMapping: (float t)=>{return Mathf.SmoothStep(0.0f,1.0f,t);}
		);
		subitrTypewrite = txtFooter.typewrite(typewriteSpeed);
	}
	void Start(){
		rtFooter.anchoredPosition = v2AnchoredPosHide;
	}
	private IEnumerator rfShowFooter(List<string> lText,int textCount=-1){
		IsShowing = true;
		txtFooter.text = "";
		subitrTweenInFooter.bReverse = false;
		yield return subitrTweenInFooter;

		for(int i=0; i<lText.Count; ++i){
			subitrTypewrite.Text = lText[i];
			while(subitrTypewrite.MoveNext()){
				yield return subitrTypewrite.Current;
				if(triggerSkip){
					triggerSkip.clear();
					subitrTypewrite.skip();
					break;
				}
			}
			if(i==lText.Count-1) //for last text, no need to wait for skip
				yield break;
			/* This is for the best, because stopping and resuming ALLOCATES
			more memory on StartCoroutine, while this check, although done
			every frame, eats very small performance. */
			while(!triggerSkip)
				yield return null;
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
