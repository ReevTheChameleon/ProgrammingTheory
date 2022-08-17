using UnityEngine;
using Chameleon;

public class RoomScaler : MonoBehaviour{
	[SerializeField] float lengthSide;
	[SerializeField] float wallThickness;
	[SerializeField] float height;
	public float LengthSide{ get{return lengthSide;} }
	public float WallThickness{ get{return wallThickness;} }
	public float Height{ get{return height;} }

	#if UNITY_EDITOR
	[SerializeField] Transform tHexagon;
	[SerializeField] Doorway[] aDoor;
	[SerializeField] float doorHeight;
	[SerializeField] float doorWidth;
	[SerializeField] float floorThickness;
	[SerializeField] float offsetAngle; //degrees

	void OnValidate(){
		if(tHexagon)
			tHexagon.localScale = new Vector3(lengthSide,floorThickness,lengthSide);
		float distanceToDoor = Mathf.Sqrt(3)*lengthSide/2.0f;
		for(int i=0; i<aDoor.Length; ++i){
			float angle = offsetAngle + 60.0f*i;
			if(aDoor[i]){
				aDoor[i].WallThickness = wallThickness;
				aDoor[i].WallSize = new Vector2(lengthSide,height);
				aDoor[i].DoorSize = new Vector2(doorWidth,doorHeight);
				aDoor[i].DoorPosition = 0.0f;
				aDoor[i].transform.localEulerAngles =
					new Vector3(0.0f,angle,0.0f);
				aDoor[i].transform.localPosition = Vector3.zero;
				aDoor[i].transform.Translate( //relative to self
					new Vector3(0.0f,0.0f,distanceToDoor-wallThickness/2.0f)
				);
			}
		}
	}
	#endif
}
