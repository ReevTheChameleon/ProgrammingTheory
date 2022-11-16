using UnityEngine;
using System;
using Chameleon;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DigitMessage : MessageInteractable{
	DigitAligner digitAligner;
	[Bakable] private const string sRichTextColorBull = "yellow";
	[Bakable] private const string sRichTextColorCow = "green";

	void Awake(){
		digitAligner = GetComponentInParent<DigitAligner>();
	}
	public void initText(int[] aDigit,eDigitType[] aDigitType){
		lText.Clear();
		if(aDigit?.Length<3 || aDigitType?.Length<3) //if null all comparison except != will be false 
			return;
		int bullCount = 0;
		int cowCount = 0;
		for(int i=0; i<3; ++i){
			switch(aDigitType[i]){
				case eDigitType.Bull: ++bullCount; break;
				case eDigitType.Cow: ++cowCount; break;
			}
		}
		if(bullCount==0 && cowCount==0)
			lText.Add("These numbers do not seem to mean anything...");
		else{
			eDigitType prevDigitType = eDigitType.Normal;
			bool bAlso = false;
			for(int i=0; i<3; ++i){
				switch(aDigitType[i]){
					case eDigitType.Bull:
						bAlso = prevDigitType==eDigitType.Bull;
						lText.Add("Number <color="+sRichTextColorBull+">"+
							aDigit[i]+"</color> " + (bAlso ? "also" : "") +
							" somehow feels strongly linked to the exit...");
						prevDigitType = aDigitType[i];
						break;
					case eDigitType.Cow:
						bAlso = prevDigitType==eDigitType.Cow;
						lText.Add("Number <color="+sRichTextColorCow+">"
							+aDigit[i]+"</color> " + (bAlso ? "also" : "") +
							" seems to hint something about the exit, but it"+
							(bAlso?", too,":"") + " looks misplaced...");
						prevDigitType = aDigitType[i];
						break;
				}
			}
			if(bullCount == 3)
				lText.Add("Maybe this is <color=yellow>the room</color>!?");
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DigitMessage))]
class DigitMessageEditor : MonoBehaviourBakerEditor{ }
#endif
