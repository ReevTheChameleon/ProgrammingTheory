using UnityEngine;

public class TestRange : MonoBehaviour{
	[Range(0,100)] public int x;

	void Start(){
		x=500;
	}
	void Update(){
		Debug.Log(x);
	}
}
