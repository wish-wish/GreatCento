/// <summary>
/// Developed by pier shaw piershaw@gmail.com
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Global Object
/// </summary>
public class GlobalObject : MonoBehaviour {

    //Selection Delegate made to send an event Delegate to all the callers registered
    public delegate void SelectionDelegate(string name);
    //Selection Delegate
    public SelectionDelegate selectionDelegate;
   
    void Awake()
    {
        DontDestroyOnLoad(this);
#if UNITY_ANDROID
        Debug.Log("UNITY_ANDROID");
#else
    //  Debug.LogError("this sould be in the android platform");
#endif

    }


}
