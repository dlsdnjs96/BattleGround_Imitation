using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account : MonoBehaviour
{
    public string nickname;


    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        nickname = "";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
