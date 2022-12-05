using UnityEngine;
using System.Collections;
using Chameleon;

public abstract class OptionInspectable : Inspectable{
	[SerializeField] string sDlgMessage;
	[SerializeField] string sDlgBtn1;
	[SerializeField] string sDlgBtn2;
	protected DlgInteract dlgFooter;

	protected override void Awake(){
		base.Awake();
		dlgFooter = SceneMainManager.Instance.DlgFooter;
	}
	public override void onInteracted(){
		switch(inspectionState){
			case eInspectionState.None:
				routineInteract.start(this,rfStartOptionInspcetSequence());
				break;
			case eInspectionState.FooterTexting:
				FooterManager.Instance.stepFooter();
				break;
			case eInspectionState.Suspended:
			case eInspectionState.EndSequence:
				break; //do nothing
		}
	}
	protected virtual IEnumerator rfStartOptionInspcetSequence(){
		yield return rfStartInspectSequence();
		dlgFooter.popup(
			Vector2.zero,
			sDlgMessage,
			sDlgBtn1,
			sDlgBtn2,
			onOptionAction,
			onOptionCancel,
			onOptionCancel
		);
		dlgFooter.TweenDlg.bReverse = false;
		dlgFooter.TweenDlg.Reset();
		yield return dlgFooter.TweenDlg;
	}
	protected virtual IEnumerator rfCancelOptionInspectSequence(){
		dlgFooter.TweenDlg.bReverse = true;
		dlgFooter.TweenDlg.Reset();
		yield return new ParallelEnumerator(this,
			dlgFooter.TweenDlg,
			rfEndInspectSequence()
		);
		dlgFooter.close(true);
	}
	protected virtual void onOptionCancel(){
		routineInteract.start(this,rfCancelOptionInspectSequence());
	}
	protected abstract void onOptionAction();
}
