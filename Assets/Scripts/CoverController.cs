using UnityEngine;

public class CoverController : MonoBehaviour{
	[SerializeField] Transform tCover;

	public void setup(float lengthSide,float height,float wallThickness){
		tCover.transform.localScale =
			new Vector3(2*lengthSide,1.0f,Mathf.Sqrt(3)*lengthSide);
		tCover.transform.localPosition = new Vector3(0.0f,height,-wallThickness);
	}
}
