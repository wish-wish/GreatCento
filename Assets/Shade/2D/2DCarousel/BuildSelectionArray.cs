/// <summary>
/// Developed by pier shaw piershaw@gmail.com
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// This will build a dynamic array for the items in a circle 
/// </summary>
public class BuildSelectionArray : MonoBehaviour {

    // this is the distance from the center if the parint to z out
    private const float distance = 3.0f;
    // number of the selection objects
    private const int number = 51;
    // the position of selection object
    private Vector3 pos;
    //all the selection objects in the menu
    private GameObject[] items;
    // offsets the the space 
    private float offset;
    // distance space of selection objects from another
    private float offsetSpacing;
    // all the items
    private float showall;
    // the way the selection objects are placed on a rotation axis  
    private Vector3 rotationOffset;
    
    // Use this for initialization
    void Start(){
     
        //number++;//
        //notice that I get all objects by Resources.Load this is because source control systems mess up the library cache
        //and projects get messed up and can be impossible to put back together again if names dont match objects 
        rotationOffset = new Vector3(0, 0, 0);
        //
        for (int i = 0; i < number; i++)
        {
            items = new GameObject[number];
            pos = new Vector3(transform.position.x, transform.position.y, distance);
            offset += 1.0f;
            offsetSpacing += 0.48f;
            items[i] = Instantiate(Resources.Load("SelectionObject", typeof(GameObject)), pos, Quaternion.identity) as GameObject;
            items[i].GetComponent<TextMesh>().text = "SelectionObject " + i;
            items[i].transform.parent = transform;
            items[i].name = "SelectionObject " + i;
            items[i].transform.position = new Vector3(transform.position.x / 2, transform.position.y / 2, -100);
            
            //turns the rotation parent
            if (i >= 0)
            {
                rotationOffset = new Vector3(offset/offsetSpacing+5, 0, 0);
                transform.Rotate(rotationOffset);

                //face front
                items[i].transform.rotation = new Quaternion(0,0,0,0);
            }
        }
    }

    // Update is called once per frame
    void Update() {

        //this will make the menu rool once ant the start
        if (showall >= 25f) {
            // running
        } else {
            showall++;
            transform.Rotate(new Vector3(showall, 0, 0));
        }
        // this is backface culling
        for (int i = 0; i < number; i++) {
                GameObject c = GameObject.Find("SelectionObject " + i);
                c.transform.LookAt(GameObject.Find("Target").transform);
    
        }
    }


}
