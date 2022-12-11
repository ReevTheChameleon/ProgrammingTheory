using UnityEngine;

public class Blade : MonoBehaviour{
	[SerializeField] float rotateSpeed;
	[SerializeField] float rotateAcceleration;
	private float currentSpeed;
	private int rotateDirection = 1;

	void Start(){
		currentSpeed = rotateSpeed*rotateAcceleration;
	}
	void Update(){
		if(currentSpeed != rotateDirection*rotateSpeed){
			currentSpeed = Mathf.Clamp(
				currentSpeed + rotateDirection*rotateAcceleration,
				-rotateSpeed,
				rotateSpeed
			);
		}
		transform.Rotate(0.0f,currentSpeed*Time.deltaTime,0.0f,Space.Self);
	}
	public void reverse(){
		rotateDirection = -rotateDirection;
	}
}
