using UnityEngine;
using Chameleon;
using System.Collections;

public class KeyPickable : PickableInspectable{
	protected override void Start(){
		base.Start();
		v2IconEndPos = KeyManager.Instance.VKeyIconScreenPos;
	}
	protected override IEnumerator rfPickIconSequence(){
		v2IconStartPos = transform.position;
		yield return base.rfPickIconSequence();
		KeyManager.Instance.addKey();
	}
}
