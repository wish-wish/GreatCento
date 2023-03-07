// for seek a job : 13860624608 lin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderMan : MonoBehaviour
{
    public Texture font;
    List<CommandBuffer> commandBuffers;    
    Material material;

    public bool iscameraset=false;
    public bool iscolor=true;
    public static int  index=1;

    void RunComputeInCamera()
    {        
        if(!material)        
        {
            Shader shader=Shader.Find("Unlit/ScreenMatrix");
            material=new Material(shader);
            material.name="uem"+index++;
            if(font!=null)
            {
                material.SetTexture("_font_texture",font);            
            }
        }
        material.SetInt("_screen_width",Camera.main.pixelWidth);
        material.SetInt("_screen_height",Camera.main.pixelHeight);        
        material.SetTexture("_white_noise",WhiteNoiseTex.GetTexture());
        material.SetInt("_session_rand_seed",Random.Range(0,int.MaxValue));
        if(iscolor)
            material.SetInt("_color",1);
        else
            material.SetInt("_color",0);
        
        commandBuffers.Add(new CommandBuffer());        
        commandBuffers[commandBuffers.Count-1].name="commandBuffer";
        commandBuffers[commandBuffers.Count-1].DispatchCompute(WhiteNoiseTex.noise_shader,WhiteNoiseTex.kernel,512/8,512/8,1);
        commandBuffers[commandBuffers.Count-1].Blit(BuiltinRenderTextureType.None,BuiltinRenderTextureType.RenderTexture,material);
        //commandBuffers[commandBuffers.Count-1].Blit(white_noise,BuiltinRenderTextureType.CameraTarget);
        Camera.main.AddCommandBuffer(CameraEvent.AfterEverything,commandBuffers[commandBuffers.Count-1]);
    }

    void RunCompute()
    {        

        if(!material)        
        {
            Shader shader=Shader.Find("Unlit/ScreenMatrix");                
            material=new Material(shader);
            material.name="uem"+index++;
            if(font!=null)
            {
                material.SetTexture("_font_texture",font);            
            }
        }
        material.SetInt("_screen_width",Camera.main.pixelWidth);
        material.SetInt("_screen_height",Camera.main.pixelHeight);        
        material.SetTexture("_white_noise",WhiteNoiseTex.GetTexture());
        material.SetInt("_session_rand_seed",Random.Range(0,int.MaxValue));
        if(iscolor)
            material.SetInt("_color",1);
        else
            material.SetInt("_color",0);

        Renderer renderer=GetComponent<Renderer>();
        renderer.material=material;        
        
        commandBuffers.Add(new CommandBuffer());
        commandBuffers[commandBuffers.Count-1].name="commandBuffer";        
        commandBuffers[commandBuffers.Count-1].DispatchCompute(WhiteNoiseTex.noise_shader,WhiteNoiseTex.kernel,512/8,512/8,1);
        commandBuffers[commandBuffers.Count-1].Blit(BuiltinRenderTextureType.GBuffer0,BuiltinRenderTextureType.RenderTexture,material);
        //commandBuffers[commandBuffers.Count-1].Blit(white_noise,BuiltinRenderTextureType.CameraTarget);
        Camera.main.AddCommandBuffer(CameraEvent.BeforeImageEffects,commandBuffers[commandBuffers.Count-1]);
    }

    void OnDestroy()
    {        
        for(int i=commandBuffers.Count-1;i>=0;i--)
        {
            commandBuffers[i].Clear();
            // 释放插入buffer
            commandBuffers[i].Release();
            commandBuffers.Remove(commandBuffers[i]);            
        }
        if(material)
        {
            Object.Destroy(material);
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(init());
    }

    IEnumerator init()
    {
        yield return new WaitForSeconds(0.3f);
        commandBuffers=new List<CommandBuffer>();           
        if(iscameraset)
        {
            RunComputeInCamera();
        }else
        {
            RunCompute();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
