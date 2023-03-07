// for seek a job : 13860624608 lin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteNoiseTex : MonoBehaviour
{
    static RenderTexture white_noise;
    static public ComputeShader noise_shader;
    static public int kernel=0;

    public ComputeShader noise;
    bool isstart=false;

    // Start is called before the first frame update
    void Start()
    {    
        StartCoroutine(init());
    }

    IEnumerator init()
    {
        yield return new WaitForSeconds(0.1f);
        noise_shader=noise;
        kernel=noise_shader.FindKernel("Generate_White_Noise");
        noise_shader.SetTexture(kernel,"_white_noise",WhiteNoiseTex.GetTexture());
        noise_shader.SetInt("_session_rand_seed",Random.Range(0,int.MaxValue));
        isstart=true;
    }
    
    public static RenderTexture GetTexture()
    {
        if(!white_noise)
        {
            white_noise=new RenderTexture(512,512,0);
            white_noise.name="white_noise";
            white_noise.enableRandomWrite=true;  
            white_noise.useMipMap=false;      
            white_noise.filterMode=FilterMode.Point;
            white_noise.wrapMode=TextureWrapMode.Repeat;
            white_noise.Create();
        }
        return white_noise;        
    }        


    // Update is called once per frame
    void Update()
    {
        if(isstart)
            noise_shader.SetInt("_session_rand_seed",Mathf.CeilToInt(Time.time*7.0f));
    }

    void OnDestroy()
    {
        if(isstart&&!white_noise)
        {
            white_noise.Release();
            white_noise = null;
        }
    }
}
