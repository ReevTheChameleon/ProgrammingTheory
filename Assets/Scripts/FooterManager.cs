using UnityEngine;
using TMPro;
using Chameleon;
using System.Collections;

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
	protected override void Awake(){
		rtFooter = (RectTransform)transform;
		subitrTweenInFooter = rtFooter.tweenAnchoredPosition(
			v2AnchoredPosHide,
			v2AnchoredPosShow,
			footerTransitionTime,
			dMapping: (float t)=>{return Mathf.SmoothStep(0.0f,1.0f,t);}
		);
		subitrTypewrite = txtFooter.typewrite("",typewriteSpeed);
	}
	void Start(){
		rtFooter.anchoredPosition = v2AnchoredPosHide;
	}
	private IEnumerator rfShowFooter(string[] aText){
		IsShowing = true;
		txtFooter.text = "";
		subitrTweenInFooter.bReverse = false;
		yield return subitrTweenInFooter;

		for(int i=0; i<aText.Length; ++i){
			subitrTypewrite.Text = aText[i];
			while(subitrTypewrite.MoveNext()){
				yield return subitrTypewrite.Current;
				if(triggerSkip){
					triggerSkip.clear();
					subitrTypewrite.skip();
					break;
				}
			}
			/* This is for the best, because stopping and resuming ALLOCATES
			more memory on StartCoroutine, while this check, although done
			every frame, eats very small performance. */
			while(!triggerSkip)
				yield return null;
		}

		subitrTweenInFooter.bReverse = true;
		yield return subitrTweenInFooter;
		IsShowing = false;
	}
	public void showFooter(string[] aText){
		routineFooter.start(this,rfShowFooter(aText));
	}
	public void stepFooter(){
		triggerSkip.set();
	}
}
