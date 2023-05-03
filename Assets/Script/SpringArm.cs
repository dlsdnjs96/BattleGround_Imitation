using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField]
    private Vector3 arm;

    private GameObject spring;
    private RaycastHit hit;


    void Awake()
    {
        spring = transform.Find("Spring").gameObject;
    }
    void OnEnable()
    {
        StartCoroutine("UpdateSpringArm");
    }

    void OnDisable()
    {
        StopCoroutine("UpdateSpringArm");
    }

    IEnumerator UpdateSpringArm()
    {
        yield return new WaitForSeconds(0.1f);
       // int layerMask = (-1) - (1 << LayerMask.NameToLayer("Player"));
        int layerMask = (1 << LayerMask.NameToLayer("Map"));
        if (Physics.Raycast(transform.position, arm, out hit, Vector3.Distance(Vector3.zero, arm), layerMask))
            spring.transform.localPosition = arm.normalized * Vector3.Distance(transform.position, hit.point);
        else
            spring.transform.localPosition = arm;

        StartCoroutine(UpdateSpringArm());
    }
}
