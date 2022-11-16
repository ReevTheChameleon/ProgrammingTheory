using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Graphic))]
public class Blink : MonoBehaviour{
	Graphic graphic;
	[SerializeField] Color color1;
	[SerializeField] Color color2;
	[SerializeField] float time1;
	[SerializeField] float time2;

	void Awake(){
		graphic = GetComponent<Graphic>();
	}
	void OnEnable(){
		StartCoroutine(rfBlink());
	}
	void OnDisable(){
		StopAllCoroutines();
	}
	private IEnumerator rfBlink(){
		WaitForSeconds wait1 = new WaitForSeconds(time1);
		WaitForSeconds wait2 = new WaitForSeconds(time2);
		while(true){
			graphic.color = color1;
			yield return wait1;
			graphic.color = color2;
			yield return wait2;
		}
		/* This is a simple blink, not taking small time remainder
		after each wait into account. */
	}
}
