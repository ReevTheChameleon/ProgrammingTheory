using UnityEngine;
using Chameleon;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneStoryManager : StoryManager{
	[TextArea][SerializeField] string[] aText;
	[SerializeField] SceneIndex indexSceneMain;
	
	protected override string[] AText{
		get{return aText;}
	}
	protected override IEnumerator rfRunStory(string[] aText){
		Cursor.lockState = CursorLockMode.Locked;
		yield return base.rfRunStory(aText);
		advanceScene();
	}
	private void advanceScene(){
		SceneManager.LoadSceneAsync(indexSceneMain);
	}
}
