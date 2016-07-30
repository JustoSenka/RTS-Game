
Shader "Custom/ArrowShader" {

	Properties{
		_Color("Main Color", Color) = (0,1,0,1)
		_EmisColor("Emissive Color", Color) = (.2,.2,.2,0)
		_MainTex("Particle Texture", 2D) = "white" {}
	}

	SubShader{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Tags { "LightMode" = "Vertex" }
		Cull Off
		Lighting On
		Material { Emission[_EmisColor] }
		ColorMaterial AmbientAndDiffuse
		ZWrite Off
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .001
		Pass {
			AlphaTest Greater 0.5
			
			SetTexture[_MainTex] {
				combine primary * texture
			}
			
			SetTexture[_MainTex]{
				constantColor(1,1,1,1)
				combine constant lerp(texture) previous
			}
			
			SetTexture[_MainTex]{
				constantColor[_Color]
				combine previous * constant
			}
		}
	}
}