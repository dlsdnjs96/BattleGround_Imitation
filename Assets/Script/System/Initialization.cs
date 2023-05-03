using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initialization : MonoBehaviour
{
    [SerializeField]
    GameObject UndestoryableObj;

    void Start()
    {
        DontDestroyOnLoad(UndestoryableObj);

        SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
