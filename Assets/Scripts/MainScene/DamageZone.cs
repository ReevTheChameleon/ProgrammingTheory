using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamageZone : MonoBehaviour{
	[Bakable][Tag] const string sTagPlayer = "Player";
	[SerializeField] float damageFrequency; //per seconds
	[SerializeField] float damageAmount;
	private float timeTilDamage = 0.0f;

	void Start(){
		this.enabled = false;
	}
	void OnTriggerEnter(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		timeTilDamage = Time.deltaTime;
		this.enabled = true;
	}
	void OnTriggerExit(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		this.enabled = false;
	}
	void Update(){
		PlayerController playerController = PlayerController.Instance;
		//Polling. Can change to something more efficient, but overkill for this project
		if(playerController.InputMode != eInputMode.MainGameplay)
			return; //do not do damage during cutscene and pause
		timeTilDamage -= Time.deltaTime;
		if(timeTilDamage <= 0.0f){
			playerController.damagePlayer(damageAmount);
			timeTilDamage += 1/damageFrequency;
		}
	}
}

[CustomEditor(typeof(DamageZone))]
class SpikeDamageZoneEditor : MonoBehaviourBakerEditor{ }
