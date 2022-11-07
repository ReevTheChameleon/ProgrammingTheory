using UnityEngine;
using Chameleon;

public class Jump : MonoBehaviour{
	[SerializeField] AnimationCurve curve;
	[SerializeField] float jumpDistance;
	[SerializeField] float jumpPeriod;
	RectTransform rt;
	private float originalY;
	private float t;
	void Awake(){
		rt = (RectTransform)transform;
	}
	void OnEnable(){
		originalY = rt.anchoredPosition.y;
		t = 0.0f;
	}
	void Update(){
		t = t+(Time.deltaTime/jumpPeriod);
		while(t > 1.0f)
			t -= 1.0f;
		rt.anchoredPosition = rt.anchoredPosition.newY(originalY+jumpDistance*curve.Evaluate(t));
	}
}
