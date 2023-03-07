using UnityEngine;
using System.Collections;

public class CamBloodTest : MonoBehaviour
{
    public Camera cam;
    public float startVal;
    public float dur;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CamBlood.Inst.Create(cam, startVal, dur);
        }
    }
}
