using UnityEngine;
using Chameleon;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Light))]
public class Portal : MonoBehaviour{
	Light lightPortal;
	Material matPortal;
	[SerializeField] ShaderPropertyID_float shaderPropEmission;
	[SerializeField] AnimationCurve curveEmissionxDistance;
	[SerializeField] float factorLight;

	void Awake(){
		lightPortal = GetComponent<Light>();
		matPortal = GetComponent<Renderer>().sharedMaterial;
	}
	void Update(){
		float distancePlayer = Vector2.Distance(
			PlayerController.Instance.transform.position.xz(),transform.position.xz());
		float emission = curveEmissionxDistance.Evaluate(distancePlayer);
		lightPortal.intensity = emission*factorLight;
		matPortal.setFloat(shaderPropEmission,emission);
	}
}
