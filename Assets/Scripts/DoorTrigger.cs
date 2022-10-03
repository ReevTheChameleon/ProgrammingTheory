using UnityEngine;

public class DoorTrigger : MonoBehaviour{
	Doorway doorway;

	void Awake(){
		doorway = GetComponentInParent<Doorway>();
	}
	void OnTriggerEnter(Collider other){
		//if(other.CompareTag("Player")){
		//	Debug.Log(other.name);
		//	SceneMainManager.Instance.changeRoom(doorway.doorId);
		//}
	}
}
