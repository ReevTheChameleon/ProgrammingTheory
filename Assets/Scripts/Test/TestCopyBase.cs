using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TestCopyBase{
	static SerializedObject so;

	[MenuItem("CONTEXT/Component/TestCopy")]
	public static void testCopy(MenuCommand menuCommand){
		so = new SerializedObject(menuCommand.context);
	}
	[MenuItem("CONTEXT/Component/TestPaste")]
	public static void testPaste(MenuCommand menuCommand){
		SerializedObject so2 = new SerializedObject(menuCommand.context);
		SerializedProperty sp = so.GetIterator();
		while(sp.Next(true)){
			so2.CopyFromSerializedProperty(sp);
		}
	}
}
