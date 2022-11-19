using UnityEngine;
using System.Collections.Generic;

public class MessageInteractable : Interactable{
	[SerializeField][TextArea] protected List<string> lText = new List<string>();

	public override void onInteracted(){
		FooterManager footerManager = FooterManager.Instance;
		if(!footerManager.IsShowing){
			PlayerController.Instance.setActiveInputMovement(false);
			footerManager.showFooter(lText);
		}
		else if(!footerManager.IsDone)
			footerManager.stepFooter();
		else{
			footerManager.hideFooter();
			PlayerController.Instance.setActiveInputMovement(true);
		}
	}
	protected override void OnTriggerExit(Collider other){
		//FooterManager.Instance.hideFooter();
		base.OnTriggerExit(other);
	}
}
