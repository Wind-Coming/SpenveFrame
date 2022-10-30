using System.Collections;
using System.Collections.Generic;
using Spenve;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoroutineSystem.Instance.StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        // int a = 5;
        // int b = 7;
        yield return null;
        //int c = a + b;
    }
}
