using UnityEngine;
using Chameleon;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamageZone : MonoBehaviour{
	[Bakable][Tag] const string sTagPlayer = "Player";
	[SerializeField] float damageFrequency; //per seconds
	[SerializeField] float damageAmount;
	private LoneCoroutine routineDamagePlayer = new LoneCoroutine();

	void OnTriggerEnter(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		routineDamagePlayer.start(this,rfDamage());
	}
	void OnTriggerExit(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		routineDamagePlayer.stop();
	}
	void OnDisable(){
		routineDamagePlayer.stop();
	}
	private IEnumerator rfDamage(){
		WaitForSeconds w = new WaitForSeconds(1/damageFrequency);
		while(true){
			PlayerController.Instance.damagePlayer(damageAmount);
			yield return w;
		}
	}
}

[CustomEditor(typeof(DamageZone))]
class SpikeDamageZoneEditor : MonoBehaviourBakerEditor{ }
