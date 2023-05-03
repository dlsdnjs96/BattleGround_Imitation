using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BlurEffect : MonoBehaviour
{
    PostProcessVolume m_Volume;
    Vignette m_Vignette;

    void Awake()
    {
        gameObject.SetActive(false);
    }
    void Start()
    {
    }
}