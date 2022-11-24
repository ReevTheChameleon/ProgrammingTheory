using UnityEngine;
using Chameleon;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(DigitInspectable))]
public class DigitAligner : MonoBehaviour{
	/* These have to be direct children of DigitAligner.gameObject */
	[SerializeField] TMP_Text[] aDigit;
	[SerializeField] Renderer[] aSlotRenderer;
	
	#if UNITY_EDITOR
	[SerializeField] float digitHeight;
	[SerializeField] float digitDistance;
	[SerializeField] float fontSize;
	[SerializeField][Range(0,9)] int[] number;
	#endif
	
	DigitInspectable digitMessage;

	void Awake(){
		digitMessage = GetComponent<DigitInspectable>();
	}
	//Let throw if aDigit/format is not correct because game can't run with that
	public int getDigit(int index){
		return int.Parse(aDigit[index].text);
	}
	public void setDigit(int index,int value){
		aDigit[index].text = value.ToString();
	}
	public void setMaterial(int index,Material material){
		aSlotRenderer[index].sharedMaterial = material;
	}

	#if UNITY_EDITOR
	void OnValidate(){
		/* Check aDigit validity */
		if(aDigit.Length<3 || aSlotRenderer.Length<3)
			return;
		for(int i=0; i<3; ++i)
			if(!aDigit[i] || !aSlotRenderer[i])
				return;

		for(int i=0; i<3; ++i){
			setDigit(i,number[i]);
			aDigit[i].fontSize = fontSize;
			aSlotRenderer[i].transform.localScale = new Vector3(
				digitDistance/10.0f,
				1.0f,
				digitHeight/10.0f
			);
		}
		
		aSlotRenderer[0].transform.localPosition = aDigit[0].transform.localPosition
			= new Vector3(-digitDistance,0.0f,0.0f);
		aSlotRenderer[1].transform.localPosition = aDigit[1].transform.localPosition
			= Vector3.zero;
		aSlotRenderer[2].transform.localPosition = aDigit[2].transform.localPosition
			= new Vector3(digitDistance,0.0f,0.0f);

		/* set text height, if done directly here will show warning "SendMessage
		cannot be called during...", probably due to how TextMeshPro handles things.
		Also can't share looping local variable i as it will have gone out of bound. */
		EditorApplication.delayCall += ()=>{
			if(aDigit[0]) aDigit[0].rectTransform.setHeight(digitHeight);
			if(aDigit[1]) aDigit[1].rectTransform.setHeight(digitHeight);
			if(aDigit[2]) aDigit[2].rectTransform.setHeight(digitHeight);
		};
		BoxCollider cTrigger = GetComponentInChildren<BoxCollider>();
		cTrigger.center = Vector3.zero;
		cTrigger.size = new Vector3(3*digitDistance,1.0f,digitHeight);
	}
	#endif
}

