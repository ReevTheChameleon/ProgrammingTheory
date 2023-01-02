using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

using Object = UnityEngine.Object;

namespace Chameleon{

[Flags]
public enum eColorChannel{
	None=0,
	R=1,
	G=2,
	B=4,
	RGB=7,
	A=8,
	All=15
}

public static class TextureManipulator{
	public static Texture2D invert(this Texture2D tex2d,eColorChannel channel=eColorChannel.RGB,
		bool bOverwrite=false) //if overwrite, will write to src texture and save some memory
	{
		if(!tex2d){
			return null;}
		if(!bOverwrite){
			tex2d = tex2d.clone();}
		Color[] aColor = tex2d.GetPixels();
		if(channel.HasFlag(eColorChannel.R)){
			for(int i=0; i<aColor.Length; ++i){
				aColor[i].r = 1.0f-aColor[i].r;}
		}
		if(channel.HasFlag(eColorChannel.G)){
			for(int i=0; i<aColor.Length; ++i){
				aColor[i].g = 1.0f-aColor[i].g;}
		}
		if(channel.HasFlag(eColorChannel.B)){
			for(int i=0; i<aColor.Length; ++i){
				aColor[i].b = 1.0f-aColor[i].b;}
		}
		if(channel.HasFlag(eColorChannel.A)){
			for(int i=0; i<aColor.Length; ++i){
				aColor[i].a = 1.0f-aColor[i].a;}
		}
		tex2d.SetPixels(aColor);
		tex2d.Apply();
		return tex2d;
	}

	/* There is a need to copy texture of different formats to each other, which
	Graphics.CopyTexture cannot do, so we need to write our own function.*/
	public static Texture2D clone(this Texture2D src){
		Texture2D dst = new Texture2D(src.width,src.height);
		Color[] aColorSrc = src.GetPixels();
		Color[] aColorDst = dst.GetPixels();
		for(int i=0; i<aColorSrc.Length; ++i){
			aColorDst[i] = aColorSrc[i];}
		dst.SetPixels(aColorDst);
		dst.Apply();
		return dst;
	}
	public static bool copyPasteColorChannel(
		Texture2D src,eColorChannel srcChannel,Texture2D dst,eColorChannel dstChannel)
	{
		if(src && (src.width!=dst.width || src.height!=dst.height)){
			Debug.LogError("Textures are not the same size!");
			return false;
		}
		if(!Algorithm.hasOneBitSet((int)srcChannel) || !Algorithm.hasOneBitSet((int)dstChannel)){
			Debug.Log((int)srcChannel+" "+Algorithm.hasOneBitSet((int)srcChannel));
			Debug.Log((int)dstChannel+" "+Algorithm.hasOneBitSet((int)dstChannel));
			Debug.LogError("Can only copy-paste from ONE channel to ANOTHER");
			return false;
		}
		Color[] aColorDst = dst.GetPixels();
		if(!src){
			switch(dstChannel){
				case eColorChannel.R:
					for(int i=0; i<aColorDst.Length; ++i){
						aColorDst[i].r = 0;}
					break;
				case eColorChannel.G:
					for(int i=0; i<aColorDst.Length; ++i){
						aColorDst[i].g = 0;}
					break;
				case eColorChannel.B:
					for(int i=0; i<aColorDst.Length; ++i){
						aColorDst[i].b = 0;}
					break;
				case eColorChannel.A:
					for(int i=0; i<aColorDst.Length; ++i){
						aColorDst[i].a = 0;}
					break;
			}
		}
		else{
			Color[] aColorSrc = src.GetPixels();
			/* There seems to be no better way than nested switch with 16 possibilities.
			Alternatives are map/array of function pointers, using single switch on, say,
			srcChannel*16+dstChannel (Credit: Nawaz & James Kanze, SO).
			It would have been much easier if we can assume the layout of data in Color class
			and access to address/reference manipulation. */
			/* The reason to put for-loop in the switch is to avoid condition check every pixel. */
			switch(srcChannel){
				case eColorChannel.R:
					switch(dstChannel){
						case eColorChannel.R:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].r = aColorSrc[i].r;}
							break;
						case eColorChannel.G:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].g = aColorSrc[i].r;}
							break;
						case eColorChannel.B:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].b = aColorSrc[i].r;}
							break;
						case eColorChannel.A:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].a = aColorSrc[i].r;}
							break;
					}
					break;
				case eColorChannel.G:
					switch(dstChannel){
						case eColorChannel.R:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].r = aColorSrc[i].g;}
							break;
						case eColorChannel.G:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].g = aColorSrc[i].g;}
							break;
						case eColorChannel.B:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].b = aColorSrc[i].g;}
							break;
						case eColorChannel.A:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].a = aColorSrc[i].g;}
							break;
					}
					break;
				case eColorChannel.B:
					switch(dstChannel){
						case eColorChannel.R:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].r = aColorSrc[i].b;}
							break;
						case eColorChannel.G:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].g = aColorSrc[i].b;}
							break;
						case eColorChannel.B:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].b = aColorSrc[i].b;}
							break;
						case eColorChannel.A:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].a = aColorSrc[i].b;}
							break;
					}
					break;
				case eColorChannel.A:
					switch(dstChannel){
						case eColorChannel.R:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].r = aColorSrc[i].a;}
							break;
						case eColorChannel.G:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].g = aColorSrc[i].a;}
							break;
						case eColorChannel.B:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].b = aColorSrc[i].a;}
							break;
						case eColorChannel.A:
							for(int i=0; i<aColorSrc.Length; ++i){
								aColorDst[i].a = aColorSrc[i].a;}
							break;
					}
					break;
			}
		}
		dst.SetPixels(aColorDst);
		dst.Apply();
		return true;
	}

	[MenuItem("CONTEXT/TextureImporter/Invert Texture")]
	static void invertRGB(MenuCommand menuCommand){
		TextureImporter textureImporter = (TextureImporter)menuCommand.context;
		string pathTexture = AssetDatabase.GetAssetPath(menuCommand.context);
		string pathOut = EditorUtility.SaveFilePanel(
			"Save Inverted Texture",
			Path.GetDirectoryName(pathTexture),
			"Out.png",
			"png"
		);
		if(pathOut.Length == 0){
			return;} //cancelled
		
		bool bSavedReadable = textureImporter.isReadable;
		if(!bSavedReadable){
			textureImporter.isReadable = true;
			textureImporter.SaveAndReimport();
		}

		Texture2D src = AssetDatabase.LoadAssetAtPath<Texture2D>(pathTexture);
		Texture2D dst = src.clone();
		dst.invert(eColorChannel.R,true);
		dst.invert(eColorChannel.G,true);
		dst.invert(eColorChannel.B,true);
		
		byte[] texPNG = dst.EncodeToPNG();
		if(texPNG != null){
			File.WriteAllBytes(pathOut,texPNG);
			string relativePath = pathOut.Substring(Application.dataPath.Length-"Assets".Length);
			AssetDatabase.ImportAsset(relativePath);
		}
		if(!bSavedReadable){
			textureImporter.isReadable = false;
			textureImporter.SaveAndReimport();
		}
	}

	public class TextureChannelAssembler : EditorWindow{
		private Texture2D tex2d_R;
		private Texture2D tex2d_G;
		private Texture2D tex2d_B;
		private Texture2D tex2d_A;

		private eColorChannel srcChannel_R = eColorChannel.R;
		private eColorChannel srcChannel_G = eColorChannel.G;
		private eColorChannel srcChannel_B = eColorChannel.B;
		private eColorChannel srcChannel_A = eColorChannel.A;

		private bool bInvert_R;
		private bool bInvert_G;
		private bool bInvert_B;
		private bool bInvert_A;

		private static string lastPath;

		[MenuItem("Chameleon/Texture Channel Assembler")]
		public static void showWindow(){
			GetWindowWithRect<TextureChannelAssembler>(
				new Rect(0.0f,0.0f,420.0f,200.0f),
				true,
				"Texture Channel Assembler",
				true
			);
		}
		public void OnGUI(){
			float savedLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 50.0f;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10.0f);
			drawChannel("R",ref tex2d_R,ref srcChannel_R,ref bInvert_R);
			drawChannel("G",ref tex2d_G,ref srcChannel_G,ref bInvert_G);
			drawChannel("B",ref tex2d_B,ref srcChannel_B,ref bInvert_B);
			drawChannel("A",ref tex2d_A,ref srcChannel_A,ref bInvert_A);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(10.0f);
			EditorGUIUtility.labelWidth = savedLabelWidth;
			if(GUILayout.Button("Save")){
				Texture2D tex2dSize = null;
				if(!(tex2dSize=getRepresentativeSizeTexture(tex2d_R,tex2d_G,tex2d_B,tex2d_A))){
					Debug.LogError("Source Texture2D are of different sizes");
					return;
				}
				if(lastPath?.Length==0){
					lastPath = null;}
				string path = EditorUtility.SaveFilePanel(
					"Save Texture",
					Path.GetDirectoryName(lastPath) ?? Application.dataPath,
					Path.GetFileName(lastPath) ?? "Texture.png",
					"png"
				);
				if(path.Length != 0){
					Texture2D tex2d = new Texture2D(tex2dSize.width,tex2dSize.height);
					bool bReadable_R = tex2d_R==null || tex2d_R.isReadable;
					bool bReadable_G = tex2d_G==null || tex2d_G.isReadable;
					bool bReadable_B = tex2d_B==null || tex2d_B.isReadable;
					bool bReadable_A = tex2d_A==null || tex2d_A.isReadable;

					if(!bReadable_R){
						setTextureAssetReadable(tex2d_R,true);}
					if(!bReadable_G){
						setTextureAssetReadable(tex2d_G,true);}
					if(!bReadable_B){
						setTextureAssetReadable(tex2d_B,true);}
					if(!bReadable_A){
						setTextureAssetReadable(tex2d_A,true);}

					copyPasteColorChannel(
						bInvert_R ? tex2d_R?.invert(srcChannel_R,false) : tex2d_R,
						srcChannel_R,
						tex2d,
						eColorChannel.R
					);
					copyPasteColorChannel(
						bInvert_G ? tex2d_G?.invert(srcChannel_G,false) : tex2d_G,
						srcChannel_G,
						tex2d,
						eColorChannel.G
					);
					copyPasteColorChannel(
						bInvert_B ? tex2d_B?.invert(srcChannel_B,false) : tex2d_B,
						srcChannel_B,
						tex2d,
						eColorChannel.B
					);
					copyPasteColorChannel(
						bInvert_A ? tex2d_A?.invert(srcChannel_A,false) : tex2d_A,
						srcChannel_A,
						tex2d,
						eColorChannel.A
					);

					byte[] texPNG = tex2d.EncodeToPNG();
					if(texPNG != null){
						File.WriteAllBytes(path,texPNG);
						string relativePath = path.Substring(Application.dataPath.Length-"Assets".Length);
						AssetDatabase.ImportAsset(relativePath);
					}
					lastPath = path;

					if(!bReadable_R){
						setTextureAssetReadable(tex2d_R,bReadable_R);}
					if(!bReadable_G){
						setTextureAssetReadable(tex2d_G,bReadable_G);}
					if(!bReadable_B){
						setTextureAssetReadable(tex2d_B,bReadable_B);}
					if(!bReadable_A){
						setTextureAssetReadable(tex2d_A,bReadable_A);}
				}
			}
		}
		private void drawChannel(string text,ref Texture2D tex2d,ref eColorChannel channel,ref bool bInvert){
			EditorGUILayout.BeginVertical(); //Credit idea: DougRichardson, UA
			//GUIStyle styleCenter = new GUIStyle(GUI.skin.label);
			//styleCenter.alignment = TextAnchor.UpperCenter;
			EditorGUILayout.LabelField(text);
			tex2d = (Texture2D)EditorGUILayout.ObjectField(
				"",
				tex2d,
				typeof(Texture2D),
				false,
				GUILayout.Width(70.0f)
			);
			EditorGUILayout.LabelField("Channel:");
			channel = (eColorChannel)EditorGUILayout.EnumPopup(
				"",channel,GUILayout.Width(70.0f));
			bInvert = EditorGUILayout.Toggle("Invert",bInvert,GUILayout.Width(40.0f));
			EditorGUILayout.EndVertical();
		}
		private Texture2D getRepresentativeSizeTexture(params Texture2D[] aTex2d){
			Texture2D tex2dFirst = null;
			int i=0;
			while(i<aTex2d.Length){
				if(tex2dFirst=aTex2d[i]){
					break;}
				++i;
			}
			while(++i<aTex2d.Length){
				if(!aTex2d[i]){
					continue;}
				if(tex2dFirst.width!=aTex2d[i].width || tex2dFirst.height!=aTex2d[i].height){
					return null;}
			}
			return tex2dFirst;
		}
		private void setTextureAssetReadable(Texture2D tex2d,bool bReadable){
			TextureImporter textureImporter = AssetImporter.GetAtPath(
				AssetDatabase.GetAssetPath(tex2d)) as TextureImporter;
			if(textureImporter){
				textureImporter.isReadable = bReadable;
				textureImporter.SaveAndReimport();
			}
		}
	}
}

} //end namespace Chameleon
