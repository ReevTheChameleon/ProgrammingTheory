using UnityEngine;
using Chameleon;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpikeTrigger : MonoBehaviour{
	Renderer rendererSpikeFloor;
	[SerializeField] Transform tSpike; 
	[SerializeField] float delayTrigger;
	[SerializeField] float alphaDanger;
	[SerializeField][GrayOnPlay] float heightSpike;
	[SerializeField] float durationPopSpike;
	[SerializeField] float durationRetractSpike;
	[SerializeField] Collider cSpikeDamageZone;
	[Bakable][Tag] const string sTagPlayer = "Player";
	private LoneCoroutine routinePopSpike = new LoneCoroutine();
	private Material matInstFloor = null;
	private TweenRoutineUnit<Vector3> subitrPopSpike;

	public bool IsSpiking{ get{return cSpikeDamageZone.enabled;} }
	void Awake(){
		rendererSpikeFloor = GetComponent<Renderer>();
		subitrPopSpike = tSpike.tweenPosition(
			tSpike.position,
			tSpike.position + new Vector3(0.0f,heightSpike,0.0f),
			durationPopSpike
		);
		cSpikeDamageZone.enabled = false;
	}
	void OnTriggerEnter(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		subitrPopSpike.bReverse = false;
		subitrPopSpike.Reset(durationPopSpike);
		routinePopSpike.start(this,rfSpikeOn());
	}
	void OnTriggerExit(Collider other){
		if(!other.CompareTag(sTagPlayer)){
			return;}
		cSpikeDamageZone.enabled = false;
		matInstFloor.color = matInstFloor.color.newA(0.0f);
		subitrPopSpike.bReverse = true;
		subitrPopSpike.Reset(durationRetractSpike);
		routinePopSpike.start(this,subitrPopSpike);
	}
	private IEnumerator rfSpikeOn(){
		//this instantiate new material, and needed to be destroyed later
		matInstFloor = rendererSpikeFloor.material;
		matInstFloor.color = matInstFloor.color.newA(alphaDanger);
		yield return new WaitForSeconds(delayTrigger); //if this was replaced by resettable IEnumerator..., will do later
		yield return subitrPopSpike;
		cSpikeDamageZone.enabled = true;
	}
	void OnDestroy(){
		if(matInstFloor){
			Destroy(matInstFloor);}
	}
}

[CustomEditor(typeof(SpikeTrigger))]
class SpikeTriggerEditor : MonoBehaviourBakerEditor{ }
