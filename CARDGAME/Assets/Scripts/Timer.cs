using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Corou1");
    }

    IEnumerator Corou1()
    {
        //コルーチンの内容
        Debug.Log("スタート");
        yield return new WaitForSeconds(5.0f);
        Debug.Log("スタートから5秒後");
    }
}
