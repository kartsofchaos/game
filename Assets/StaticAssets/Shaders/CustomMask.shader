Shader "Custom/CustomMask" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Mask ("Mask Texture", 2D) = "white" {}
	}
	SubShader {
		Tags {"Overlay"="Transperant"}
		Lighting On
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		
		Pass {
			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {combine texture, previous}
		}
	}
}
