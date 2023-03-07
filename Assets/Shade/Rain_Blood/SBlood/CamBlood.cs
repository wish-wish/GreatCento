using UnityEngine;
using System.Collections;

public class CamBlood : MonoBehaviour
{
    float startVal;

    Material curMat;
    int propIdColor;
    int propIdNoisePos;

    float durTime;
    float curTime;

    static CamBlood _inst;
    public static CamBlood Inst
    {
        get
        {
            if (_inst == null)
            {
                GameObject gobj = Instantiate(Resources.Load<GameObject>("Prefabs/effect/CamBlood")) as GameObject;
                _inst = gobj.GetComponent<CamBlood>();
            }
            return _inst;
        }
    }

    void Start() {
        
    }

    void Awake()
    {
        //CamBlood.Inst.Create(Camera.main, startVal, durTime);
        curMat = GetComponent<Renderer>().sharedMaterial;
        propIdColor = Shader.PropertyToID("_Color");
        propIdNoisePos = Shader.PropertyToID("_NoisePos");
    }

    void Update()
    {
        if (curTime <= durTime)
        {
            //显示中
            curTime += Time.deltaTime;
            float val = (1 - (curTime / durTime)) * this.startVal;
            curMat.SetColor(propIdColor, new Color(0.19f, 0.03f, 0.03f, val));
        }
        else if(durTime > 0)
        {
            curMat.SetColor(propIdColor, new Color(0.19f, 0.03f, 0.03f, 0f));
        }
    }

    public void Create(Camera camera, float val, float durTime)
    {
        this.startVal = val;
        _inst.transform.parent = camera.transform;
        _inst.transform.localPosition = new Vector3(0f, 0f, 1f);
        _inst.transform.localEulerAngles = Vector3.zero;
        curMat.SetVector(propIdNoisePos, new Vector4(0, 0, Random.Range(0f, 1f), 1));
        curMat.SetColor(propIdColor, new Color(0.19f, 0.03f, 0.03f, val));

        this.durTime = durTime;
        curTime = 0f;
    }
}
