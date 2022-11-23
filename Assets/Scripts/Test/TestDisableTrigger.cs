using UnityEngine;

public class TestDisableTrigger : MonoBehaviour{
	void OnTriggerEnter(Collider other){
		Debug.Log("Enter");
	}
	void Update(){
	}
}
