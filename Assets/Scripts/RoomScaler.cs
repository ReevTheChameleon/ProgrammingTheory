using UnityEngine;
using Chameleon;

public class RoomScaler : MonoBehaviour{
	[SerializeField] float lengthSide;
	[SerializeField] float wallThickness;
	[SerializeField] float height;
	[SerializeField] float doorHeight;
	public float LengthSide{ get{return lengthSide;} }
	public float WallThickness{ get{return wallThickness;} }
	public float Height{ get{return height;} }
	public float DoorHeight{ get{return doorHeight;} }

	#if UNITY_EDITOR
	[SerializeField] PrimitiveDoorway[] aDoorway;
	[SerializeField] Transform tHexagon;
	[SerializeField] float doorWidth;
	[SerializeField] float floorThickness;
	[SerializeField] float offsetAngle; //degrees

	void OnValidate(){
		if(tHexagon)
			tHexagon.localScale = new Vector3(lengthSide,floorThickness,lengthSide);
		float distanceToDoor = Mathf.Sqrt(3)*lengthSide/2.0f;
		for(int i=0; i<aDoorway.Length; ++i){
			float angle = offsetAngle + 60.0f*i;
			if(aDoorway[i]){
				aDoorway[i].WallThickness = wallThickness;
				aDoorway[i].WallSize = new Vector2(lengthSide,height);
				aDoorway[i].DoorSize = new Vector2(doorWidth,doorHeight);
				aDoorway[i].DoorPosition = 0.0f;
				aDoorway[i].transform.localEulerAngles =
					new Vector3(0.0f,angle,0.0f);
				aDoorway[i].transform.localPosition = Vector3.zero;
				aDoorway[i].transform.Translate( //relative to self
					new Vector3(0.0f,0.0f,distanceToDoor-wallThickness/2.0f)
				);
			}
		}
	}
	#endif
}
