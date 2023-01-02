using UnityEngine;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
public class MeshOffsetRandomizer : MonoBehaviour{
	[SerializeField] Vector2 v2Tiling = Vector2.one;
	[SerializeField] Vector2 rangeOffsetX;
	[SerializeField] Mesh meshOriginal;
	MeshFilter meshFilter;
	private Mesh mesh;
	/* reason to store this is because MeshFilter Component may be destroyed before
	this Component, and thus leads to both Mesh leak and NullReference exception.
	Hence, we explicitly Destroy stored mesh in OnDestroy(). */

	void Reset(){
		meshFilter = GetComponent<MeshFilter>();
		meshOriginal = meshFilter.sharedMesh;
	}
	public void apply(){
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh; //also instantiate own mesh if not already exists
		Debug.Log(mesh.name);
		Vector2[] aUV = mesh.uv;
		Vector2[] aUVOriginal = meshOriginal.uv;
		Debug.Log(aUV.Length+" "+aUVOriginal.Length);
		Vector2 v2Offset = new Vector2(
			RandomExtension.range(rangeOffsetX),
			0.0f //can randomize this too, but need to make sure seam matches, so overkill here
		);
		for(int i=0; i<aUV.Length; ++i){
			aUV[i] = Vector2.Scale(aUVOriginal[i],v2Tiling);
			aUV[i] += v2Offset;
		}
		mesh.uv = aUV;
		Debug.Log(meshFilter.mesh.name);
	}
	public void revert(){
		if(meshOriginal){
			GetComponent<MeshFilter>().sharedMesh = meshOriginal;}
	}
	void OnEnable(){
		apply();
	}
	void OnDestroy(){
		if(mesh){
			Destroy(mesh);}
	}
}

[CustomEditor(typeof(MeshOffsetRandomizer))]
class MeshOffsetRandomizerEditor : Editor{
	MeshOffsetRandomizer targetAs;
	void OnEnable(){
		targetAs = (MeshOffsetRandomizer)target;
	}
	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		if(GUILayout.Button("Revert")){
			targetAs.revert();}
	}
}
