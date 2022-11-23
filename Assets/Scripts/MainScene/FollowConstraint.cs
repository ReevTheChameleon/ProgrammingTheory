using UnityEngine;

public class FollowConstraint : MonoBehaviour{
	public Transform tTarget;
	public Vector3 vOffset;

	void LateUpdate(){
		transform.position = tTarget.position + vOffset;
	}
}
