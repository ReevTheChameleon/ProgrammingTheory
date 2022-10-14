using UnityEngine;
using Chameleon;

public enum eRootUpdateMethod{OnAnimatorMove,FixedUpdate};
public delegate void DRootMotionOverride(Vector3 vDelta,Quaternion qDelta);
public class RootMotionOverrider : MonoBehaviour{
	[SerializeField] eRootUpdateMethod updateMethod;
	public DRootMotionOverride dRootMotionOverride;

	Animator animator;
	Rigidbody rb;
	private Vector3 vDelta;
	private Quaternion qDelta =Quaternion.identity;

	public void setOverrideDelegate(DRootMotionOverride overrideDelegate){
		dRootMotionOverride = overrideDelegate;
	}
	void Awake(){
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	void OnAnimatorMove(){
		if(!animator)
			return;
		vDelta += animator.deltaPosition;
		qDelta = animator.deltaRotation*qDelta;
		if(updateMethod == eRootUpdateMethod.OnAnimatorMove)
			updateRootMotion();
	}
	void FixedUpdate(){
		if(!animator || updateMethod!=eRootUpdateMethod.FixedUpdate)
			return;
		updateRootMotion();
	}
	private void updateRootMotion(){
		if(dRootMotionOverride != null)
			dRootMotionOverride.Invoke(vDelta,qDelta);
		else{
			if(rb){
				rb.position += vDelta;
				rb.rotation = qDelta*rb.rotation;
			}
			else{
				transform.position += vDelta;
				transform.rotation = qDelta*transform.rotation;
			}
		}
		vDelta = Vector3.zero;
		qDelta = Quaternion.identity;
	}
}
