using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTime : MonoBehaviour
{
    public float DeadTme=2.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,DeadTme);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
