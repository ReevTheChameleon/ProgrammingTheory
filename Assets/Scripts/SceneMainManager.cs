using UnityEngine;
using Chameleon;
using System.Collections;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class SceneMainManager : LoneMonoBehaviour<SceneMainManager>{
	[SerializeField] ObjectPooler poolerRoom;
	[SerializeField] ObjectPooler[] aPoolerDoor;

	[SerializeField] Material matBullDigit;
	[SerializeField] Material matCowDigit;
	[SerializeField] Material matNormalDigit;

	[SerializeField] GameObject gPlayer;

	private int[] aDigitExit = new int[3];
	private int[] aNumber = {0,1,2,3,4,5,6,7,8,9};
	
	private readonly float sqrt3 = Mathf.Sqrt(3);
	private float roomSize;
	private float doorHeight;

	private class RoomData{
		public GameObject gRoom;
		public GameObject[] aGDoor = new GameObject[6];
		public int connectDirection =-1;
		public int[] aDigit = new int[3];
	}
	private RoomData roomDataCurrent = new RoomData();
	private RoomData roomDataNeighbor = new RoomData();
	
	[SerializeField] GameObject gCover;
	[SerializeField] float coverFadeDuration;
	[SerializeField] float panDuration;
	private Material matCover;

	LoneCoroutine routineChangeRoom = new LoneCoroutine();

	[SerializeField] Canvas cvBalloon;
	public Canvas CanvasBalloon{ get{return cvBalloon;} }

	protected override void Awake(){
		base.Awake();
		/* For longer sequence, you can either use Linear Congruence Generator (a^x=b mod p)
		or Linear-Feedback Shift Generator to avoid storing long list.
		(Credit: gbarry & starblue, SO) */
		Algorithm.shuffle(aNumber);
		aDigitExit[0] = aNumber[0]; //unroll loop because it is short enough
		aDigitExit[1] = aNumber[1];
		aDigitExit[2] = aNumber[2];
		matCover = gCover.GetComponent<Renderer>().sharedMaterial;
	}
	void Start(){
		RoomScaler rawRoomScaler =
			poolerRoom.getObjectRawInactive().GetComponent<RoomScaler>();
		roomSize = rawRoomScaler.LengthSide;
		doorHeight = rawRoomScaler.DoorHeight;
		Algorithm.shuffle(aNumber); //for total mismatch, use aNumber[3,4,5] for current
		roomDataCurrent.gRoom = spawnRoom(Vector3.zero,aNumber);
		roomDataCurrent.aDigit[0] = aNumber[0];
		roomDataCurrent.aDigit[1] = aNumber[1];
		roomDataCurrent.aDigit[2] = aNumber[2];
		for(int i=0; i<6; ++i)
			roomDataCurrent.aGDoor[i] = spawnDoor(roomDataCurrent.gRoom,0,i);
		Debug.Log(aDigitExit[0]+" "+aDigitExit[1]+" "+aDigitExit[2]);

		//gCover.transform.position =
		//	new Vector3(0.0f,rawRoomScaler.Height/2.0f,0.0f);
		//gCover.transform.localScale = new Vector3(
		//	roomSize*sqrt3,
		//	rawRoomScaler.Height+0.2f, //offset a bit
		//	roomSize*2.0f
		//);
		//gCover.SetActive(false);
	}
	//void Update(){
		//if(Keyboard.current.eKey.wasPressedThisFrame){
		//	//int[] aSavedDigit = new int[3];
		//	//System.Array.Copy(aDigitCurrent,aSavedDigit,3);
		//	//for(int i=0; i<6; ++i){
		//	//	changeRoom(i);
		//	//	System.Array.Copy(aSavedDigit,aDigitCurrent,3);
		//	//}
		//	StartCoroutine(changeRoom(1));
		//}
		//else if(Keyboard.current.wKey.wasPressedThisFrame){
		//	StartCoroutine(changeRoom(2));
		//}
		//else if(Keyboard.current.qKey.wasPressedThisFrame){
		//	StartCoroutine(changeRoom(3));
		//}
		//else if(Keyboard.current.aKey.wasPressedThisFrame){
		//	StartCoroutine(changeRoom(4));
		//}
		//else if(Keyboard.current.sKey.wasPressedThisFrame){
		//	StartCoroutine(changeRoom(5));
		//}
		//else if(Keyboard.current.dKey.wasPressedThisFrame){
		//	StartCoroutine(changeRoom(0));
		//}
	//}
//--------------------------------------------------------------------------------------------
	#region ROOM
	public int getOppositeDirection(int direction){
		return (direction+3)%6;
	}
	private int getDoorDirection(Transform tRoom,Transform tDoor){
		Vector3 vDirection = tDoor.position-tRoom.position;
		float angleDeg = Mathf.Atan2(vDirection.z,vDirection.x)*Mathf.Rad2Deg;
		return ((int)angleDeg+389)/60 % 6;
	}
	public void updateCurrentRoom(){
		Vector3 vPlayerPos = gPlayer.transform.position;
		//If gRoom is null, let throw, because it should never happen
		if((vPlayerPos-roomDataCurrent.gRoom.transform.position).sqrMagnitude >
			(vPlayerPos-roomDataNeighbor.gRoom.transform.position).sqrMagnitude
		){
			RoomData temp = roomDataCurrent;
			roomDataCurrent = roomDataNeighbor;
			roomDataNeighbor = temp;
		}
	}
	public void prepareNeighbor(Transform tDoor){
		int direction = getDoorDirection(roomDataCurrent.gRoom.transform,tDoor);
		Debug.Log(direction);
		if(roomDataNeighbor.gRoom != null){
			if(roomDataCurrent.connectDirection == direction)
				return;
			discardNeighbor();
		}
		roomDataNeighbor.aDigit = getRoomNumber(roomDataCurrent.aDigit,direction);
		roomDataNeighbor.gRoom = spawnRoom(
			roomDataCurrent.gRoom.transform.position + getRoomOffset(direction),
			roomDataNeighbor.aDigit
		);
		roomDataCurrent.connectDirection = direction;
		int oppositeDirection = getOppositeDirection(direction);
		roomDataNeighbor.connectDirection = oppositeDirection;
		for(int i=0; i<6; ++i){
			if(i != oppositeDirection)
				roomDataNeighbor.aGDoor[i] = spawnDoor(roomDataNeighbor.gRoom,0,i);
		}
		roomDataNeighbor.aGDoor[oppositeDirection] = roomDataCurrent.aGDoor[direction];
	}
	public void discardNeighbor(){
		if(roomDataNeighbor.gRoom){
			for(int i=0; i<6; ++i){
				if(i != roomDataNeighbor.connectDirection)
					roomDataNeighbor.aGDoor[i].SetActive(false);
			}
			roomDataNeighbor.aGDoor[roomDataNeighbor.connectDirection]
				.GetComponent<IDoor>().reset();
			roomDataNeighbor.gRoom.SetActive(false);
			roomDataNeighbor.gRoom = null;
		}
	}
	private GameObject spawnRoom(Vector3 vPosition,int[] aDigit){
		GameObject gRoom = poolerRoom.getObject(vPosition);
		DigitAligner digitAligner = gRoom.GetComponentInChildren<DigitAligner>();
		for(int i=0; i<3; ++i){
			digitAligner.setDigit(i,aDigit[i]);
			if(aDigit[i]==aDigitExit[i])
				digitAligner.setMaterial(i,matBullDigit);
			else if(aDigit[i]==aDigitExit[(i+1)%3] || aDigit[i]==aDigitExit[(i+2)%3])
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
	private int[] getRoomNumber(int[] aDigitCurrent,int direction){
		int[] aDigit = new int[3];
		aDigit[0] = aDigitCurrent[0];
		aDigit[1] = aDigitCurrent[1];
		aDigit[2] = aDigitCurrent[2];
		switch(direction%6){
			case 1:
				stepDigitNoRepeat(aDigit,0,1);
				break;
			case 4:
				stepDigitNoRepeat(aDigit,0,9);
				break;
			case 0:
				stepDigitNoRepeat(aDigit,1,1);
				break;
			case 3:
				stepDigitNoRepeat(aDigit,1,9);
				break;
			case 5:
				stepDigitNoRepeat(aDigit,2,1);
				break;
			case 2:
				stepDigitNoRepeat(aDigit,2,9);
				break;
		}
		return aDigit;
	}
	private void stepDigitNoRepeat(int[] aDigit,int index,int step){
		int arrayLength = aDigit.Length;
		aDigit[index] = (aDigit[index]+step)%10;
		for(int i=1; i<arrayLength; ++i){
			if(aDigit[index] == aDigit[(index+i)%arrayLength]){
				aDigit[index] = (aDigit[index]+step)%10;
				i=0; //start over after ++i
			}
		}
	}
	private GameObject spawnDoor(GameObject gRoom,int doorType,int direction){
		Vector3 vDoorPos = gRoom.transform.position + getRoomOffset(direction)/2;
		vDoorPos.y = doorHeight/2;
		GameObject gDoor = aPoolerDoor[doorType].getObject(
			vDoorPos,
			Quaternion.Euler(0.0f,-60.0f*direction+90.0f,0.0f)
		);
		return gDoor;
	}
	#endregion
//--------------------------------------------------------------------------------------------
	#region FOOTER
	void Update(){
		if(Keyboard.current.spaceKey.wasPressedThisFrame){
			if(!FooterManager.Instance.IsShowing)
				FooterManager.Instance.showFooter(new string[]{
					"Hello, my name is Reev the Chameleon",
					"This is an example game",
				});
			else
				FooterManager.Instance.stepFooter();
		}
	}
	#endregion
//--------------------------------------------------------------------------------------------
}
