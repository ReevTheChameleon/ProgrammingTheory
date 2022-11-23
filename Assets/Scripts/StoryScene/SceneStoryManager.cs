using UnityEngine;
using TMPro;
using Chameleon;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;

[RequireComponent(typeof(PlayerInput))]
public class SceneStoryManager : LoneMonoBehaviour<SceneStoryManager>{
	[Header("Story Routine")]
	[TextArea] public string[] aText;
	[SerializeField] TextMeshProUGUI txtStory;
	[SerializeField] float typewriteSpeed;
	private TypewriteRoutineUnit subitrRunStory;
	[SerializeField] GameObject gContinue;
	[SerializeField] float blurDuration;
	private ParallelEnumerator subitrBlur;
	private InterpolableKawaseBlurFeature renderFeatureKawaseBlur;
	[SerializeField] float cooldownSkip;
	private FrameTrigger triggerSkip = new FrameTrigger();

	[Header("Input")]
	[SerializeField] InputActionID actionIDInteract;
	PlayerInput playerInput;

	[Header("Scene")]
	[SerializeField] SceneIndex indexSceneMain;

	protected override void Awake(){
		base.Awake();
		subitrRunStory = txtStory.typewrite(typewriteSpeed);
		playerInput = GetComponent<PlayerInput>();
		renderFeatureKawaseBlur =
			((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset)
				.getRendererData(0).getRendererFeature<InterpolableKawaseBlurFeature>();
		renderFeatureKawaseBlur.SetActive(true);
		subitrBlur = new ParallelEnumerator(this,
			new TweenRoutineUnit(
				(float t) => {renderFeatureKawaseBlur.Pass.iteration = Mathf.Lerp(0,16,t);},
				blurDuration
			),
			txtStory.tweenVerticesAlpha(1.0f,0.0f,blurDuration)
		);
		gContinue.SetActive(false);
	}
	void OnEnable(){
		playerInput.actions[actionIDInteract].performed += onInputInteract;
	}
	void OnDisable(){
		playerInput.actions[actionIDInteract].performed -= onInputInteract;
	}
	void Start(){
		StartCoroutine(rfRunStory());
	}
	private IEnumerator rfRunStory(){
		Cooldown cooldown = new Cooldown();
		for(int i=0; i<aText.Length; ++i){
			renderFeatureKawaseBlur.Pass.iteration = 0;
			subitrBlur.Reset();

			subitrRunStory.Text = aText[i];
			cooldown.set(cooldownSkip);
			while(subitrRunStory.MoveNext()){
				yield return subitrRunStory.Current;
				if(triggerSkip && !cooldown){
					triggerSkip.clear();
					subitrRunStory.skip();
					break;
				}
			}
			cooldown.set(cooldownSkip);

			while(cooldown)
				yield return null;
			gContinue.SetActive(true);
			while(!triggerSkip)
				yield return null;
			gContinue.SetActive(false);
			triggerSkip.clear();

			while(subitrBlur.MoveNext())
				yield return subitrBlur.Current;
		}
		advanceScene();
	}
	private void onInputInteract(InputAction.CallbackContext context){
		triggerSkip.set();
	}
	private void advanceScene(){
		renderFeatureKawaseBlur.SetActive(false);
		SceneManager.LoadSceneAsync(indexSceneMain);
	}

	#if UNITY_EDITOR || DEVELOPMENT_BUILD
	void Update(){
		if(Keyboard.current.xKey.wasPressedThisFrame)
			advanceScene();
	}
	#endif
}
