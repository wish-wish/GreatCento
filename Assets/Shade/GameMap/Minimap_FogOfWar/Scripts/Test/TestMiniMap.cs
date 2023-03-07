using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMiniMap : MonoBehaviour
{

    public Image mask;
    public Image map;
    public RawImage maskImage;
    public Transform player;

    //private Material minimapMat;

    //private Texture2D maskTexture;
    
	void Start ()
	{
	    //if (map != null)
	    //{
	    //    minimapMat = new Material(Shader.Find("UI/FogOfWar/MinimapMask"));
	    //    map.material = minimapMat;
	    //    map.material.SetColor("_FogColor", FogOfWarEffect.Instance.fogColor);
	        
	    //}
		Debug.Log(mask.GetComponent<RectTransform>().rect.size+":"+map.GetComponent<RectTransform>().rect.size+":"+maskImage.GetComponent<RectTransform>().rect.size);
	}
	
	void Update () {
	    if (player != null)
	    {
	        if (maskImage.texture == null)
	        {
	            maskImage.texture = FogOfWarEffect.Instance.minimapMask;
	        }
//	        //if (maskTexture == null)
//	        {
//	            maskTexture = FogOfWarEffect.Instance.fowMaskTexture;
//                map.material.SetTexture("_MiniMap", maskTexture);
//            }
	        Vector2 p = FogOfWarEffect.WorldPositionTo2DLocal(player.position);
			Vector2 border=mask.rectTransform.rect.size/2;
	        p.x = -p.x*map.rectTransform.rect.width;
	        p.y = -p.y*map.rectTransform.rect.height;

			// if(p.x<-map.rectTransform.rect.width+border.x)
			// {
			// 	p.x=-map.rectTransform.rect.width+border.x;
			// }
			// if(p.x>map.rectTransform.rect.width-border.x)
			// {
			// 	p.x=map.rectTransform.rect.width-border.x;
			// }
			// if(p.y>-map.rectTransform.rect.height+border.y)
			// {
			// 	p.y=-map.rectTransform.rect.height+border.y;
			// }
			// if(p.y<map.rectTransform.rect.height-border.y)
			// {
			// 	p.y=map.rectTransform.rect.height-border.y;
			// }
			
            map.rectTransform.anchoredPosition = p;
	    }
	}

    void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label(map.rectTransform.anchoredPosition.ToString("f4"));
    }
}
