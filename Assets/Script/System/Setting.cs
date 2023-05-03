using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public static Setting Instance;


    public float mouseSensitivity;

    private void Awake()
    {
        Instance = this;
    }

    public float GetMouseSensitivity() { return Instance.mouseSensitivity; }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
