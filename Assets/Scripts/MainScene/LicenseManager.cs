#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[System.Serializable]
public class AssetLicense{
	public Object asset;
	public string copyrightText;
	public string licenseType;
	public string licenseURL;
	public Object licenseFile;
	public string note;
}

[CustomPropertyDrawer(typeof(AssetLicense))]
class AssetLicenseDrawer : PropertyDrawer{
	public override float GetPropertyHeight(SerializedProperty property,GUIContent label){
		return EditorGUI.GetPropertyHeight(property,label,true);
	}
	public override void OnGUI(Rect position,SerializedProperty property,GUIContent label){
		EditorGUI.PropertyField(
			position,
			property,
			new GUIContent(
				property.FindPropertyRelative(nameof(AssetLicense.asset)).objectReferenceValue?.name ?? 
				"Missing"
			),
			true
		);
	}
}

public class LicenseManager : ScriptableObject{
	[SerializeField] AssetLicense[] aAssetLicense;

	[MenuItem("Assets/Create/Chameleon/License Manager",priority =-1)]
	static void createLicenseManagerAsset(){
		LicenseManager licenseManager = CreateInstance<LicenseManager>();
		AssetDatabase.CreateAsset(licenseManager,Application.dataPath);
		AssetDatabase.SaveAssets(); //may use newer AssetDatabase.SaveAssetIfDirty() if supported
	}
}

#endif
