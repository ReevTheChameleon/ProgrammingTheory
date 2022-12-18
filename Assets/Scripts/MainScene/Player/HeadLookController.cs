using UnityEngine;
using Chameleon;
using System.Collections;

public class HeadLookController : LoneMonoBehaviour<HeadLookController>{
	[SerializeField] RigLookAt rigHeadLookAt;
	[SerializeField] float maxSpeedHeadTurn; //deg/s
	private Quaternion[] aqPrev;
		
	#if UNITY_EDITOR
	[SerializeField] bool bDrawRay;
	#endif

	public void setHeadLookTarget(Transform tTarget){
		rigHeadLookAt.tLookTarget = tTarget; //let LateUpdate check and assign
	}

	protected override void Awake(){
		base.Awake();
		aqPrev = new Quaternion[rigHeadLookAt.aRigSetting.Length];
		for(int i=0; i<aqPrev.Length; ++i)
			aqPrev[i] = Quaternion.identity;
	}
	void LateUpdate(){
		rigHeadLookAt.calculateRig();
		for(int i=0; i<rigHeadLookAt.aRigSetting.Length; ++i){
			RigLookAtSetting rigSetting = rigHeadLookAt.aRigSetting[i];
			Quaternion qDeltaLocal =
				rigSetting.transform.rotation.inverse() * rigSetting.RiggedRotation;
			Quaternion qDeltaPrev = aqPrev[i].inverse() * qDeltaLocal;
			float maxAngle = Time.deltaTime * maxSpeedHeadTurn * rigSetting.weight;
			qDeltaPrev = qDeltaPrev.clampAngle(-maxAngle,maxAngle);
			aqPrev[i] = aqPrev[i] * qDeltaPrev;
			rigSetting.transform.rotation = rigSetting.transform.rotation * aqPrev[i];
		}
		#if UNITY_EDITOR
		if(bDrawRay)
			rigHeadLookAt.drawLookRay();
		#endif
	}
}
