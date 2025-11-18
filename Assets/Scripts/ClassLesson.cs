using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassLesson : MonoBehaviour
{

    [SerializeField] private MyScript myScript;
    void Start()
    {
        myScript.Test();

    }

}
