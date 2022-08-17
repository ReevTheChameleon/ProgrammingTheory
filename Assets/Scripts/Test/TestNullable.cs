using UnityEngine;
using Chameleon;
using UnityEngine.Profiling;
using System.Diagnostics;

public class TestNullable : MonoBehaviour{
	int count = 1000000;
	void Start(){
		Profiler.BeginSample("Transform Position Normal");
		//for(int i=0; i<count; ++i)
		//	transform.position = new Vector3(2.0f,transform.position.y,5.0f);
		Stopwatch sw = Stopwatch.StartNew();
		transformNormal();
		sw.Stop();
		UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
		Profiler.EndSample();
		
		Profiler.BeginSample("Transform SetX");
		//for(int i=0; i<count; ++i)
		//	transform.setX(2.0f);
		sw.Restart();
		transformSetX();
		sw.Stop();
		UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
		Profiler.EndSample();
		
		Profiler.BeginSample("Transform Nullable");
		//for(int i=0; i<count; ++i)
		//	transform.setPosition(x:2.0f,z:5.0f);
		sw.Restart();
		transformNullable();
		sw.Stop();
		UnityEngine.Debug.Log(sw.ElapsedMilliseconds);
		Profiler.EndSample();
	}
	void transformNormal(){
		for(int i=0; i<count; ++i)
			transform.position = new Vector3(2.0f,transform.position.y,5.0f);
	}
	void transformSetX(){
		for(int i=0; i<count; ++i)
			transform.setX(2.0f);
	}
	void transformNullable(){
		for(int i=0; i<count; ++i)
			transform.setPosition(x:2.0f,z:5.0f);
	}
}
