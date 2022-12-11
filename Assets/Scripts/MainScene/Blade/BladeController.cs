using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BladeController : MonoBehaviour{
	[Bakable][Tag] const string sTagWall = "RoomBound";
	[SerializeField] float speedMove;
	[SerializeField] float speedRotate;
	
	Blade blade;
	ParticleSystem psSpark;
	private int forwardX = 1;
	private float deltaRotation = 0.0f;
	private int triggerEnteredCount = 0;

	void Awake(){
		blade = GetComponentInChildren<Blade>();
		psSpark = GetComponentInChildren<ParticleSystem>();
	}
	void OnTriggerEnter(Collider cOther){
		if(!cOther.CompareTag(sTagWall) || deltaRotation != 0.0f){
			return;}
		forwardX = -forwardX;
		Vector3 vDirectionHeading = transform.right*forwardX;
		Vector3 vPerpendicular = Vector3.Project(vDirectionHeading,cOther.transform.forward);
		//float hitAngle = Vector3.SignedAngle(transform.right,vPerpendicular,Vector3.up);
		//deltaRotation = Random.Range(-angleDeviation,angleDeviation);
		float angleToNorm = Vector3.SignedAngle(vDirectionHeading,vPerpendicular,Vector3.up);
		Vector3 vToPlayer = PlayerController.Instance.transform.position-transform.position;
		float angleNormToDest = Vector3.SignedAngle(vPerpendicular,vToPlayer,Vector3.up);
		//Debug.Log(angleToNorm + " "+angleNormToDest);
		/* If not, there is a chance that blade will bounce very short distance between
		2 adjacent sides, which looks and feels bad. */
		if(angleNormToDest<-30.0f || angleNormToDest>30.0f){
			//angleNormToDest = (Random.Range(0,2)==0?-1:1) * Mathf.Clamp(angleNormToDest,-30.0f,30.0f);
			angleNormToDest = -Mathf.Clamp(angleNormToDest,-30.0f,30.0f);
			/* This fixes a bug where blade bounces back and forth without attempting to reach
			player when it is near the opposite side to player.
			One side effect is that if it hit player once and player does not move, it will
			deflect away from player the next bounce, which feels OK as it can be seen as
			 giving player a chance to escape. */
		}
		if(Mathf.Abs(angleToNorm+angleNormToDest) < 0.1f){
			angleNormToDest = -angleNormToDest;}
		deltaRotation = angleToNorm + angleNormToDest;
		//deltaRotation = Vector3.SignedAngle(
		//	transform.right*forwardX,
		//	PlayerController.Instance.transform.position-transform.position,
		//	Vector3.up
		//);
		psSpark.gameObject.SetActive(false);
		psSpark.transform.localPosition = -psSpark.transform.localPosition;
		psSpark.transform.rotation =
			QuaternionExtension.mirror(psSpark.transform.rotation,Vector3.up);
		blade.reverse();
	}
	void OnTriggerExit(Collider cOther){
		if(cOther.CompareTag(sTagWall)){
			--triggerEnteredCount; }
	}
	void FixedUpdate(){
		Debug.DrawRay(transform.position,transform.right*forwardX*20.0f,Color.red,Time.fixedDeltaTime,false);
		if(deltaRotation != 0.0f){
			float rotateAmount = Mathf.Min(
				Mathf.Abs(deltaRotation),
				speedRotate*Time.fixedDeltaTime
			);
			rotateAmount *= Mathf.Sign(deltaRotation);
			transform.Rotate(0.0f,0.0f,rotateAmount,Space.Self);
			deltaRotation -= rotateAmount;
			if(deltaRotation == 0.0f){
				psSpark.gameObject.SetActive(true);}
		}
		else{
			transform.Translate(speedMove*forwardX*Time.fixedDeltaTime,0.0f,0.0f,Space.Self);}
		//float angleToPlayer = Vector3.SignedAngle(
		//	transform.right*forwardX,
		//	PlayerController.Instance.transform.position-transform.position,
		//	Vector3.up
		//);
		//if(angleToPlayer>=-angleTrackPlayer && angleToPlayer<=angleTrackPlayer){
		//	float maxRotate = speedRotate*Time.fixedDeltaTime;
		//	transform.Rotate(
		//		0.0f,
		//		0.0f,
		//		Mathf.Clamp(angleToPlayer,-maxRotate,maxRotate),
		//		Space.Self
		//	);
		//}
	}
}

[CustomEditor(typeof(BladeController))]
class BladeControllerEditor : MonoBehaviourBakerEditor{ }
