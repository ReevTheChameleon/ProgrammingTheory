using UnityEngine;
using System.Collections;
using Chameleon;
using UnityEngine.UI;

public class PickableInspectable : OptionInspectable{
	protected override void onOptionAction(){
		routineInteract.start(this,rfActionSequence());
	}
	protected virtual IEnumerator rfActionSequence(){
		PlayerController playerController = PlayerController.Instance;
		dlgFooter.TweenDlg.bReverse = true;
		dlgFooter.TweenDlg.Reset();
		Vector3 vWalkTarget =
			transform.position.newY(playerController.transform.position.y)-
			playerController.transform.forward*playerController.DistancePickup
		;
		yield return new ParallelEnumerator(this,
			PlayerController.Instance.rfWalkToward(
				vWalkTarget,
				Vector3.Distance(
					transform.position.xz(),
					PlayerController.Instance.transform.position.xz()
				) < playerController.DistancePickup ?
					PlayerController.eCutsceneMotion.WalkBackward :
					PlayerController.eCutsceneMotion.Walk
			),
			dlgFooter.TweenDlg
		);
		dlgFooter.close(true);
		yield return PlayerController.Instance.rfPickup();
		yield return rfEndInspectSequence();
		SceneMainManager.Instance.CanvasBalloon.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
	public virtual void onPicked(){
		HeadLookController.Instance.setHeadLookTarget(null);
	}
}
