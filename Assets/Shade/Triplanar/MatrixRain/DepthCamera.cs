// for seek a job : 13860624608 lin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DepthCamera : MonoBehaviour
{
    CommandBuffer commandBuffer;
    Material material;
    float transition_timer;
    int cap;
    float transition_direction = 0.0f;

    public Texture font;
    public bool colored=false;    
    public static int midx=0;

    void RunCompute()
    {             
        //Camera.main.depthTextureMode = DepthTextureMode.Depth;   
        if(!material)        
        {
            Shader shader=Shader.Find("Unlit/DepthCamera");                
            material=new Material(shader);
            material.name="uem"+midx++;
            if(font!=null)
            {
                material.SetTexture("font_texture",font);            
            }
        }
        //material.SetInt("_screen_width",Camera.main.pixelWidth);
        //material.SetInt("_screen_height",Camera.main.pixelHeight);        
        material.SetTexture("white_noise",WhiteNoiseTex.GetTexture());
        material.SetInt("_session_rand_seed",Random.Range(0,int.MaxValue));
        if(colored)
            material.SetInt("colored",1);
        else
            material.SetInt("colored",0);

        // Renderer renderer=GetComponent<Renderer>();
        // renderer.material=material;

        commandBuffer=new CommandBuffer();
        commandBuffer.name="commandBuffer";        
        commandBuffer.DispatchCompute(WhiteNoiseTex.noise_shader,WhiteNoiseTex.kernel,512/8,512/8,1);
        //commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive,BuiltinRenderTextureType.RenderTexture,material);
        //commandBuffer.Blit(white_noise,BuiltinRenderTextureType.CameraTarget);
        //commandBuffer.Blit(WhiteNoiseTex.GetTexture(),BuiltinRenderTextureType.CameraTarget);
        //commandBuffer.Blit(WhiteNoiseTex.GetTexture(),BuiltinRenderTextureType.Depth);
        Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque,commandBuffer);
    }
    

    // Start is called before the first frame update
    void Start()
    {        
        StartCoroutine(init());
    }

    IEnumerator init()
    {
        yield return new WaitForSeconds(0.3f);
        RunCompute(); 
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if      (cap >= 1) transition_direction = -1.0f;
            else if (cap <= 0) transition_direction =  1.0f;

            bool ShouldCount = true;

            if(((transition_direction == -1.0f) && (transition_timer > (float)(cap))))
                 ShouldCount = false;

            if (((transition_direction == 1.0f) && (transition_timer < (float)(cap+1))))
                ShouldCount = false;

            if(ShouldCount)
            cap += (int) Mathf.Sign(transition_direction) * 1;
        }

        transition_timer += Time.deltaTime * transition_direction * 0.4f;
        transition_timer = Mathf.Clamp(transition_timer, cap , cap +1);


        Shader.SetGlobalFloat ("_Global_Transition_value", transition_timer);
        Shader.SetGlobalVector("_Global_Effect_center",    Camera.main.transform.position);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
            Graphics.Blit(source, destination, material);
        else
            Graphics.Blit(source, destination);
    }
}
