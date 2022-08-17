using UnityEngine;
using Chameleon;

public class Doorway : PrimitiveDoorway{
	[SerializeField] BoxCollider cExit;
	[SerializeField] float colliderThickness;
	[SerializeField] GameObject gDoor;
	public int doorId;

	protected override void realign(){
		base.realign();
		cExit.size = cExit.size.newZ(colliderThickness);
		cExit.transform.localPosition = new Vector3(
			doorPosition,
			v2DoorSize.y/2,
			cExit.size.z/2 + wallThickness/2
		);
		cExit.transform.localScale = new Vector3(
			v2DoorSize.x,
			v2DoorSize.y,
			cExit.transform.localScale.z
		);
	}
	void OnTriggerEnter(Collider other){

	}
}

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(Doorway))]
class DoorwayEditor : PrimitiveDoorway.PrimitiveDoorwayEditor{ }
#endif
