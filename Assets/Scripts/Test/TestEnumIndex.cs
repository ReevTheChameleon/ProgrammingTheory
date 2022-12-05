using UnityEngine;
using Chameleon;
using System;

public class TestEnumIndex : MonoBehaviour{
	enum eWhat{What,Why }
	[EnumIndex(typeof(eWhat))] public int[] aI;
}
