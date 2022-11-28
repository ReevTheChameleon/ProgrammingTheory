using UnityEngine;

public class FollowConstraint : MonoBehaviour{
	public Transform tTarget;
	public Vector3 vOffset;

	void LateUpdate(){
		transform.position = TargetPosition;
	}
	public Vector3 TargetPosition{ get{return tTarget.position+vOffset;} }
}
