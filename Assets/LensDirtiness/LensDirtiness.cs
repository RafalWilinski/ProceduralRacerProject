/*=============================================================================
CHANGELOG 
			- april 2014
				* Removed manual UV flip, not needed anymore, fixed in shader
				* Fixed texture format
				* Bloom code is now inside a loop
                * Added shader keyword for bloom tint

=============================================================================*/

using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
[AddComponentMenu("Image Effects/Lens Dirtiness")]

public class LensDirtiness : MonoBehaviour
{
	private Shader Shader_Dirtiness;
	private Material Material_Dirtiness;
	RenderTextureFormat RTT_Format;
	RenderTexture RTT_BloomThreshold, RTT_1, RTT_2, RTT_3, RTT_4, RTT_Bloom_1, RTT_Bloom_2;
	private int ScreenX = 1280, ScreenY = 720;
	public bool ShowScreenControls = false, SceneTintsBloom = true;
	public Texture2D DirtinessTexture;
    enum Pass
    {   
        Threshold = 0,
        Kawase = 1,
        Compose = 2,
    };
	//Effect parameters
	public float gain = 1.0f, threshold = 1.0f, BloomSize = 5.0f, Dirtiness = 1.0f;
	public Color BloomColor = Color.white;

    void OnEnable()
	{
		//Create Material
		Shader_Dirtiness = Shader.Find ("Hidden/LensDirtiness");
		if (Shader_Dirtiness == null)
			Debug.Log ("#ERROR# Hidden/LensDirtiness Shader not found");
		Material_Dirtiness = new Material (Shader_Dirtiness);
        Material_Dirtiness.hideFlags = HideFlags.HideAndDontSave;
		TextureFormat();
        SetKeyword();
	}

    void TextureFormat()
    {
        //Default Unity formats

        if (Camera.main.hdr)
            RTT_Format = RenderTextureFormat.ARGBHalf;
        else
            RTT_Format = RenderTextureFormat.ARGB32;
    }

    void SetKeyword()
    {
        if (!Material_Dirtiness)
            return;
        if (SceneTintsBloom)
            Material_Dirtiness.EnableKeyword("_SCENE_TINTS_BLOOM");
        else
            Material_Dirtiness.DisableKeyword("_SCENE_TINTS_BLOOM");

    }

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
        #if UNITY_EDITOR
        TextureFormat();
        SetKeyword();
        #endif

		ScreenX = source.width;
		ScreenY = source.height;
		
		//Send Parameters
		Material_Dirtiness.SetFloat ("_Gain", gain);
		Material_Dirtiness.SetFloat ("_Threshold", threshold);
		
		//Get first sample for thersholding
		RTT_BloomThreshold = RenderTexture.GetTemporary (ScreenX / 2, ScreenY / 2, 0, RTT_Format);
        RTT_BloomThreshold.name = "RTT_BloomThreshold";
		//Apply thereshold Effect
		Graphics.Blit (source, RTT_BloomThreshold, Material_Dirtiness, (int)Pass.Threshold);
		
		//Downsampling
        Material_Dirtiness.SetVector("_Offset", new Vector4(1f/ScreenX, 1f/ScreenY, 0, 0)*2);

		RTT_1 = RenderTexture.GetTemporary (ScreenX / 2, ScreenY / 2, 0, RTT_Format);
		Graphics.Blit (RTT_BloomThreshold, RTT_1, Material_Dirtiness, (int)Pass.Kawase);
		RenderTexture.ReleaseTemporary (RTT_BloomThreshold);
		
		RTT_2 = RenderTexture.GetTemporary (ScreenX / 4, ScreenY / 4, 0, RTT_Format);
        Graphics.Blit(RTT_1, RTT_2, Material_Dirtiness, (int)Pass.Kawase);
		RenderTexture.ReleaseTemporary (RTT_1);
		
		RTT_3 = RenderTexture.GetTemporary (ScreenX / 8, ScreenY / 8, 0, RTT_Format);
        Graphics.Blit(RTT_2, RTT_3, Material_Dirtiness, (int)Pass.Kawase);
		RenderTexture.ReleaseTemporary (RTT_2);
		
		RTT_4 = RenderTexture.GetTemporary (ScreenX / 16, ScreenY / 16, 0, RTT_Format);
        Graphics.Blit(RTT_3, RTT_4, Material_Dirtiness, (int)Pass.Kawase);
		RenderTexture.ReleaseTemporary (RTT_3);
        
        RTT_1.name = "RTT_1";
        RTT_2.name = "RTT_2";
        RTT_3.name = "RTT_3";
        RTT_4.name = "RTT_4";

		//Bloom
        RTT_Bloom_1 = RenderTexture.GetTemporary(ScreenX/16, ScreenY/16, 0, RTT_Format);
        RTT_Bloom_1.name = "RTT_Bloom_1";
        RTT_Bloom_2 = RenderTexture.GetTemporary(ScreenX/16, ScreenY/16, 0, RTT_Format);
        RTT_Bloom_2.name = "RTT_Bloom_2";

        Graphics.Blit(RTT_4, RTT_Bloom_1);
        RenderTexture.ReleaseTemporary(RTT_4);   

        for (int iteration = 1; iteration <= 8; ++iteration)
        {
            float OffsetX = BloomSize * iteration / ScreenX;
            float OffsetY = BloomSize * iteration / ScreenY;

            Material_Dirtiness.SetVector("_Offset", new Vector4(OffsetX, OffsetY, 0, 0));
            Graphics.Blit(RTT_Bloom_1, RTT_Bloom_2, Material_Dirtiness, (int)Pass.Kawase);
            Graphics.Blit(RTT_Bloom_2, RTT_Bloom_1, Material_Dirtiness, (int)Pass.Kawase);            
            
        }

        RenderTexture.ReleaseTemporary(RTT_Bloom_1);
        RenderTexture.ReleaseTemporary(RTT_Bloom_2);             
		
		//Compose
		Material_Dirtiness.SetTexture ("_Bloom", RTT_Bloom_2);
		Material_Dirtiness.SetFloat ("_Dirtiness", Dirtiness);
		Material_Dirtiness.SetColor ("_BloomColor", BloomColor);
		Material_Dirtiness.SetTexture ("_DirtinessTexture", DirtinessTexture);

        Graphics.Blit(source, destination, Material_Dirtiness, (int)Pass.Compose);
        
	}
	
	void OnGUI ()
	{

		if (ShowScreenControls) {
			float SliderLeftMargin = 150;

			GUI.Box (new Rect (15, 15, 250, 200), "");
			

			
			//Gain
			GUI.Label (new Rect (25, 25, 100, 20), "Gain= " + gain.ToString ("0.0"));
			gain = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 30, 100, 20), gain, 0.0f, 10.0f);
			
			//threshold
			GUI.Label (new Rect (25, 45, 100, 20), "Threshold= " + threshold.ToString ("0.0"));
			threshold = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 50, 100, 20), threshold, 0.0f, 10.0f);
			
			//BloomSize
			GUI.Label (new Rect (25, 65, 100, 20), "BloomSize= " + BloomSize.ToString ("0.0"));
			BloomSize = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 70, 100, 20), BloomSize, 0.0f, 10.0f);
			
			//Dirtiness
			GUI.Label (new Rect (25, 85, 100, 20), "Dirtiness= " + Dirtiness.ToString ("0.0"));
			Dirtiness = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 90, 100, 20), Dirtiness, 0.0f, 10.0f);
						
			//Color
			GUI.Label (new Rect (25, 125, 100, 20), "R= " + (BloomColor.r * 255).ToString ("0."));
			GUI.color = new Color (BloomColor.r, 0, 0);
			BloomColor.r = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 130, 100, 20), BloomColor.r, 0.0f, 1.0f);
			GUI.color = Color.white;
			GUI.Label (new Rect (25, 145, 100, 20), "G= " + (BloomColor.g * 255).ToString ("0."));
			GUI.color = new Color (0, BloomColor.g, 0);
			BloomColor.g = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 150, 100, 20), BloomColor.g, 0.0f, 1.0f);
			GUI.color = Color.white;
			GUI.Label (new Rect (25, 165, 100, 20), "R= " + (BloomColor.b * 255).ToString ("0."));
			GUI.color = new Color (0, 0, BloomColor.b);
			BloomColor.b = GUI.HorizontalSlider (new Rect (SliderLeftMargin, 170, 100, 20), BloomColor.b, 0.0f, 1.0f);
			GUI.color = Color.white;
		}
	}
}
