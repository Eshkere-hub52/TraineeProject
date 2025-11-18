using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyScript : MonoBehaviour
{

    void Start()
    {
        
    }

    private int myField = 5;

    public int Test() 
    {
        Debug.Log(myField);
        return myField;
    }

}
