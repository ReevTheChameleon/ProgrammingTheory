using UnityEngine;
using Chameleon;
using System.Collections;

public class HeadLookController : LoneMonoBehaviour<HeadLookController>{
	[SerializeField] RigLookAt rigHeadLookAt;
	[SerializeField] Vector2 eulerAngleRangeY;
	[SerializeField] private Transform tHeadLookTarget;
	[SerializeField] private bool bInYLookRange;
	[SerializeField] float maxSpeedHeadTurn; //deg/s
	private Quaternion[] aqPrev;

	public void setHeadLookTarget(Transform tTarget){
		tHeadLookTarget = tTarget;
		rigHeadLookAt.tLookTarget = null; //let LateUpdate check and assign
		bInYLookRange = false;
	}

	protected override void Awake(){
		base.Awake();
		aqPrev = new Quaternion[rigHeadLookAt.aRigSetting.Length];
		for(int i=0; i<aqPrev.Length; ++i)
			aqPrev[i] = Quaternion.identity;
	}
	void LateUpdate(){
		if(tHeadLookTarget){
			float deltaEulerY = (transform.rotation.inverse() *
				Quaternion.LookRotation(tHeadLookTarget.position-transform.position))
				.eulerAngles.y
			;
			if(Mathf.DeltaAngle(deltaEulerY,eulerAngleRangeY.x)<=0 &&
				Mathf.DeltaAngle(deltaEulerY,eulerAngleRangeY.y)>=0)
			{
				if(!bInYLookRange){
					rigHeadLookAt.tLookTarget = tHeadLookTarget;
					bInYLookRange = true;
				}
			}
			else{
				if(bInYLookRange){
					rigHeadLookAt.tLookTarget = null;
					bInYLookRange = false;
				}
			}
		}

		rigHeadLookAt.calculateRig();
		for(int i=0; i<rigHeadLookAt.aRigSetting.Length; ++i){
			RigLookAtSetting rigSetting = rigHeadLookAt.aRigSetting[i];
			Quaternion qDeltaLocal =
				rigSetting.transform.rotation.inverse() * rigSetting.riggedRotation;
			Quaternion qDeltaPrev = aqPrev[i].inverse() * qDeltaLocal;
			float maxAngle = Time.deltaTime * maxSpeedHeadTurn * rigSetting.weight;
			qDeltaPrev = qDeltaPrev.clampAngle(-maxAngle,maxAngle);
			aqPrev[i] = aqPrev[i] * qDeltaPrev;
			rigSetting.transform.rotation = rigSetting.transform.rotation * aqPrev[i];
		}

		rigHeadLookAt.drawLookRay();
	}
}
