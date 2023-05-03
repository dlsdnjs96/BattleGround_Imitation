using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Bullet bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        transform.position += new Vector3(y, 0, x);

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position += transform.right * 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            transform.position += transform.up * 50 * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q))
            transform.position += transform.up * 50 * Time.deltaTime;


        float horizontal = Input.GetAxis("Mouse X") * 50 * Time.deltaTime;
        float vertical = Input.GetAxis("Mouse Y") * 50 * Time.deltaTime;

        transform.eulerAngles += new Vector3(-vertical, horizontal, 0);

        if (Input.GetKeyDown(KeyCode.G))
        {
            Bullet bt = (Bullet)Instantiate(bullet);
            bt.SetBulletTransform(transform.position, transform.forward);
        }
    }
}
