using UnityEngine;
using TMPro;
using Chameleon;
using System.Collections;

public class SceneEndManager : StoryManager{
	[TextArea][SerializeField] string[] aTextWin;
	[TextArea][SerializeField] string[] aTextLose;

	[SerializeField] TextMeshProUGUI txtDigit;
	[SerializeField] float durationFadeInDigit;
	
	protected override string[] AText{
		get{return SceneMainManager.IsWin ? aTextWin : aTextLose;}
	}
	protected override IEnumerator rfRunStory(string[] aText){
		Debug.Log("start rf");
		txtDigit.gameObject.SetActive(true);
		float alphaTxtDigit = txtDigit.alpha;
		Color32 colorEnd = txtDigit.color;
		yield return txtDigit.tweenVerticesColor(
			new Color32(0,0,0,colorEnd.a),
			colorEnd,
			durationFadeInDigit,
			bFirstFrame: true
		);
		yield return base.rfRunStory(aText);
	}
}
