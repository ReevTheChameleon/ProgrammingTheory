using UnityEngine;
using Chameleon;
using UnityEngine.InputSystem;
using System.Collections;

public class SceneMainManager : LoneMonoBehaviour<SceneMainManager>{
	[SerializeField] ObjectPooler poolerRoom;
	[SerializeField] Material matBullDigit;
	[SerializeField] Material matCowDigit;
	[SerializeField] Material matNormalDigit;

	private int[] aDigitExit = new int[3];
	private int[] aDigitCurrent = new int[3];
	
	private readonly float sqrt3 = Mathf.Sqrt(3);
	private float roomSize;
	
	[SerializeField] GameObject gCover;
	[SerializeField] float coverFadeDuration;
	[SerializeField] float panDuration;
	private GameObject gRoomCurrent;
	private Material matCover;
	
	protected override void Awake(){
		base.Awake();
		/* For longer sequence, you can either use Linear Congruence Generator (a^x=b mod p)
		or Linear-Feedback Shift Generator to avoid storing long list.
		(Credit: gbarry & starblue, SO) */
		int[] aNumber = {0,1,2,3,4,5,6,7,8,9};
		Algorithm.shuffle(aNumber);
		aDigitExit[0] = aNumber[0]; //unroll loop because it is short enough
		aDigitExit[1] = aNumber[1];
		aDigitExit[2] = aNumber[2];
		Algorithm.shuffle(aNumber); //for total mismatch, use aNumber[3,4,5] for current
		aDigitCurrent[0] = aNumber[0];
		aDigitCurrent[1] = aNumber[1];
		aDigitCurrent[2] = aNumber[2];
		matCover = gCover.GetComponent<Renderer>().sharedMaterial;
	}
	void Start(){
		RoomScaler rawRoomScaler =
			poolerRoom.getObjectRawInactive().GetComponent<RoomScaler>();
		roomSize = rawRoomScaler.LengthSide;
		gRoomCurrent = spawnRoom(Vector3.zero,aDigitCurrent);
		Debug.Log(aDigitExit[0]+" "+aDigitExit[1]+" "+aDigitExit[2]);

		gCover.transform.position =
			new Vector3(0.0f,rawRoomScaler.Height/2.0f,0.0f);
		gCover.transform.localScale = new Vector3(
			roomSize*sqrt3,
			rawRoomScaler.Height+0.2f, //offset a bit
			roomSize*2.0f
		);
		gCover.SetActive(false);
	}
	void Update(){
		if(Keyboard.current.eKey.wasPressedThisFrame){
			//int[] aSavedDigit = new int[3];
			//System.Array.Copy(aDigitCurrent,aSavedDigit,3);
			//for(int i=0; i<6; ++i){
			//	changeRoom(i);
			//	System.Array.Copy(aSavedDigit,aDigitCurrent,3);
			//}
			StartCoroutine(changeRoom(1));
		}
		else if(Keyboard.current.wKey.wasPressedThisFrame){
			StartCoroutine(changeRoom(2));
		}
		else if(Keyboard.current.qKey.wasPressedThisFrame){
			StartCoroutine(changeRoom(3));
		}
		else if(Keyboard.current.aKey.wasPressedThisFrame){
			StartCoroutine(changeRoom(4));
		}
		else if(Keyboard.current.sKey.wasPressedThisFrame){
			StartCoroutine(changeRoom(5));
		}
		else if(Keyboard.current.dKey.wasPressedThisFrame){
			StartCoroutine(changeRoom(0));
		}
	}
	private GameObject spawnRoom(Vector3 vPosition,int[] aDigit){
		GameObject gRoom = poolerRoom.getObject(vPosition);
		DigitAligner digitAligner = gRoom.GetComponentInChildren<DigitAligner>();
		for(int i=0; i<3; ++i){
			digitAligner.setDigit(i,aDigit[i]);
			if(aDigit[i]==aDigitExit[i])
				digitAligner.setMaterial(i,matBullDigit);
			else if(aDigit[i]==aDigitExit[(i+1)%3] || aDigitCurrent[i]==aDigitExit[(i+2)%3])
				digitAligner.setMaterial(i,matCowDigit);
			else
				digitAligner.setMaterial(i,matNormalDigit);
		}
		return gRoom;
	}
	private Vector3 getRoomOffset(int direction){
		Vector2 vOffset = Vector2Extension.fromPolar(roomSize*sqrt3,60.0f*direction);
		return new Vector3(vOffset.x,transform.position.y,vOffset.y);
	}
	/* This function maps direction angle with how digit changes.
	You can modify that behaviour here. */
	private void updateRoomNumber(int direction){
		switch(direction%6){
			case 1:
				stepDigitNoRepeat(aDigitCurrent,0,1);
				break;
			case 4:
				stepDigitNoRepeat(aDigitCurrent,0,9);
				break;
			case 0:
				stepDigitNoRepeat(aDigitCurrent,1,1);
				break;
			case 3:
				stepDigitNoRepeat(aDigitCurrent,1,9);
				break;
			case 5:
				stepDigitNoRepeat(aDigitCurrent,2,1);
				break;
			case 2:
				stepDigitNoRepeat(aDigitCurrent,2,9);
				break;
		}
	}
	private void stepDigitNoRepeat(int[] aDigit,int index,int step){
		int arrayLength = aDigit.Length;
		aDigit[index] = (aDigit[index]+step)%10;
		for(int i=1; i<arrayLength; ++i){
			if(aDigit[index] == aDigit[(index+i)%arrayLength]){
				aDigit[index] = (aDigit[index]+step)%10;
				i=1; //start over
			}
		}
	}
	private IEnumerator changeRoom(int direction){
		updateRoomNumber(direction);
		GameObject gRoomSpawn = spawnRoom(getRoomOffset(direction),aDigitCurrent);
		gCover.transform.localEulerAngles = new Vector3(
			0.0f,
			-direction*60.0f,
			0.0f
		);
		Vector3 vRoomSpawnPos = gRoomSpawn.transform.position;
		gCover.transform.setPosition(
			x:vRoomSpawnPos.x,
			z:vRoomSpawnPos.z
		);
		gCover.SetActive(true);
		yield return StartCoroutine(matCover.tweenAlpha(
			1.0f,0.0f,coverFadeDuration)
		);
		yield return StartCoroutine(Camera.main.transform.tweenPosition(
			Camera.main.transform.position,
			new Vector3(vRoomSpawnPos.x,Camera.main.transform.position.y,vRoomSpawnPos.z),
			panDuration)
		);
		gCover.transform.setPosition(x:0.0f,z:0.0f);
		yield return StartCoroutine(matCover.tweenAlpha(
			0.0f,1.0f,coverFadeDuration)
		);
		gCover.SetActive(false);
		gRoomCurrent.SetActive(false);
		gRoomCurrent = gRoomSpawn;
		gRoomCurrent.transform.position = Vector3.zero;
		Camera.main.transform.setPosition(x:0.0f,z:0.0f);
	}

	#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(SceneMainManager))]
	class SceneMainManagerEditor : UnityEditor.Editor{
		private SceneMainManager targetAs;
		void OnEnable(){
			targetAs = (SceneMainManager)target;
		}
		public override void OnInspectorGUI(){
			DrawDefaultInspector();
			if(GUILayout.Button("Generate Room"))
				((SceneMainManager)target).spawnRoom(Vector3.zero,targetAs.aDigitCurrent);
		}
	}
	#endif
}


