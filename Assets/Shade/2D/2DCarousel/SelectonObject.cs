/// <summary>
/// Developed by pier shaw piershaw@gmail.com
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Selecton Object Menu object being selected
/// </summary>
public class SelectonObject : MonoBehaviour
{

    void Start()
    {
        GameObject globalObject = GameObject.Find("GlobalObject");
        globalObject.GetComponent<GlobalObject>().selectionDelegate += everyone;
    }
    // all the Selecton Objects get this message 
    public void everyone(string name)
    {
        Debug.Log("i "+this.gameObject.name +" understand that the selection anmed "+ name + " was called ");
    }

}
