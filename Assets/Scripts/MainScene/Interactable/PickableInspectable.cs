using UnityEngine;
using System.Collections;
using Chameleon;
using UnityEngine.UI;

public class PickableInspectable : Inspectable{
	[SerializeField] float durationDlgTween;
	private Vector2 v2DlgAnchoredPosStart;
	private Vector2 v2DlgPosEnd;
	[SerializeField] string sDlgMessage = "What should you do?";
	[SerializeField] string sDlgBtn1 = "Collect";
	[SerializeField] string sDlgBtn2 = "Leave it";
	[SerializeField] float distanceCollectable;
	private TweenRoutineUnit subitrTweenDlg;
	DlgTwoButton dlgFooter;
	MeshRenderer meshRenderer;

	[SerializeField] Image imgIconPick;
	[SerializeField] Sprite spIcon;
	[SerializeField] float durationTweenIcon;
	private TweenRoutineUnit subitrTweenIconPick;
	private LoneCoroutine routineTweenIconPick = new LoneCoroutine();
	private Vector3 vIconStartPos;
	private Vector3 vIconEndPos;


	protected override void Awake(){
		base.Awake();
		dlgFooter = SceneMainManager.Instance.DlgFooter;
		meshRenderer = GetComponentInChildren<MeshRenderer>();
	}
	void OnEnable(){
		meshRenderer.enabled = true;
	}
	void Start(){
		RectTransform rtDlgFooter = (RectTransform)dlgFooter.transform;
		RectTransform rtHUDCanvas = (RectTransform)rtDlgFooter.root.GetComponent<Canvas>().transform;
		v2DlgAnchoredPosStart = new Vector2(
			rtHUDCanvas.rect.width/2 + rtDlgFooter.rect.width/2,
			-rtHUDCanvas.rect.height/2 + rtDlgFooter.rect.height + 5.0f
		);
		v2DlgPosEnd = v2DlgAnchoredPosStart;
		v2DlgPosEnd.x -= rtDlgFooter.rect.width;
		subitrTweenDlg =
			((RectTransform)dlgFooter.transform).tweenAnchoredPosition(
				v2DlgAnchoredPosStart,v2DlgPosEnd,durationDlgTween);
		//Assume resolution is fixed since game starts
		subitrTweenIconPick = new TweenRoutineUnit(
			(float t) => {
				imgIconPick.transform.position = Vector3.Lerp(vIconStartPos,vIconEndPos,t);
			},
			durationTweenIcon
		);
	}
	public override void onInteracted(){
		switch(inspectionState){
			case eInspectionState.None:
				routineInteract.start(this,rfKeyStartInspectSequence());
				break;
			case eInspectionState.FooterTexting:
				FooterManager.Instance.stepFooter();
				break;
			case eInspectionState.Suspended:
				break; //do nothing
		}
	}
	private IEnumerator rfKeyStartInspectSequence(){
		yield return rfStartInspectSequence();
		dlgFooter.popup(
			v2DlgAnchoredPosStart,
			sDlgMessage,
			sDlgBtn1,
			sDlgBtn2,
			onOptionPickup,
			onOptionCancel,
			onOptionCancel
		);
		subitrTweenDlg.bReverse = false;
		subitrTweenDlg.Reset();
		yield return subitrTweenDlg;
	}
	private IEnumerator rfKeyEndInspectSequence(){
		subitrTweenDlg.bReverse = true;
		subitrTweenDlg.Reset();
		yield return new ParallelEnumerator(this,
			subitrTweenDlg,
			rfEndInspectSequence()
		);
		dlgFooter.close(true);
	}
	private void onOptionCancel(){
		routineInteract.start(this,rfKeyEndInspectSequence());
	}
	private void onOptionPickup(){
		routineInteract.start(this,rfKeyPickupSequence());
	}
	private IEnumerator rfKeyPickupSequence(){
		PlayerController playerController = PlayerController.Instance;
		subitrTweenDlg.bReverse = true;
		subitrTweenDlg.Reset();
		Vector3 vWalkTarget =
			transform.position.newY(playerController.transform.position.y)-
			playerController.transform.forward*distanceCollectable
		;
		yield return new ParallelEnumerator(this,
			PlayerController.Instance.rfWalkToward(
				vWalkTarget,
				Vector3.Distance(
					transform.position.xz(),
					PlayerController.Instance.transform.position.xz()
				) < distanceCollectable
			),
			subitrTweenDlg
		);
		dlgFooter.close(true);
		yield return PlayerController.Instance.rfPickup();
		yield return rfEndInspectSequence();
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
	public void onPicked(){
		routineTweenIconPick.start(this,rfPickIconSequence());
	}
	private IEnumerator rfPickIconSequence(){
		HeadLookController.Instance.setHeadLookTarget(null);
		meshRenderer.enabled = false;
		vIconStartPos = Camera.main.WorldToScreenPoint(transform.position);
		//z doesn't matter for overlay Canvas (we assume icon is on such)
		imgIconPick.gameObject.SetActive(true);
		subitrTweenIconPick.Reset();
		yield return subitrTweenIconPick;
	}
}
