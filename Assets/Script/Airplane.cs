using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Airplane : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 dir;
    Vector3 curPos;




    [SerializeField]
    private GameObject electronicField;

    private AudioSource audioPlayer;
    public AudioClip flyingClip;

    const float flyingHeight = 600.0f;
    float flyingTime;
    float passedTime;

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        print("Airplane " + PhotonNetwork.IsMasterClient);

        passedTime = 0.0f;
        print(startPoint + " " + endPoint);
        if (PhotonNetwork.IsMasterClient)
        {
            MakeAirPath();
            print(startPoint + " " + endPoint);
            photonView .RPC("SetPath", RpcTarget.All, startPoint, endPoint, flyingTime);
        }
        print(startPoint + " " + endPoint);
        //transform.position = startPoint;
        //transform.rotation = Quaternion.LookRotation(dir);// Quaternion.Euler(0, -180.0f * Mathf.Atan2(dir.x, dir.z) / Mathf.PI, 0);

        StartCoroutine(BoardPlane());
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            passedTime += Time.deltaTime;

            if (passedTime > flyingTime)
            {
                PhotonNetwork.Instantiate("ForceField", Vector3.zero, Quaternion.identity);
                //Instantiate(electronicField);
                GameObject.Destroy(this);
                return;
            }
            transform.position += dir * Time.deltaTime;

            //transform.position = Vector3.Lerp(startPoint, endPoint, passedTime / flyingTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 1.0f);
        }

        //DestroyObject(this);
    }

    IEnumerator BoardPlane()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (var it in GameObject.FindGameObjectsWithTag("Player"))
        {
            it.SendMessage("BoardPlane");
        }
    }

    private void MakeAirPath()
    {
        int rand = Random.Range(0, 4);

        Vector3 centerPoint;
        centerPoint = Util.GetRandomPointInCircle(new Vector3(-650.0f, 0.0f, -650.0f), 500.0f);


        startPoint.y = flyingHeight;
        centerPoint.y = flyingHeight;


        startPoint.x = Random.Range(-1800, 500);
        startPoint.z = Random.Range(-1800, 500);

        if (rand == 0) startPoint.x = -1800;
        else if (rand == 1) startPoint.x = 500;
        else if (rand == 2) startPoint.z = -1800;
        else startPoint.z = 500;

        dir = centerPoint - startPoint;
        endPoint = startPoint + (dir * 10.0f);
        dir.Normalize();
        dir *= 100.0f;

        Vector3 hit = new Vector3(0, flyingHeight, 0);
        if (GetIntersectPoint(centerPoint, endPoint, new Vector3(-1800, 0, -1800), new Vector3(500, 0, -1800), ref hit)) 
            endPoint = hit;
        if (GetIntersectPoint(centerPoint, endPoint, new Vector3(-1800, 0, 500), new Vector3(500, 0, 500), ref hit)) 
            endPoint = hit;
        if (GetIntersectPoint(centerPoint, endPoint, new Vector3(-1800, 0, -1800), new Vector3(-1800, 0, 500), ref hit)) 
            endPoint = hit;
        if (GetIntersectPoint(centerPoint, endPoint, new Vector3(500, 0, -1800), new Vector3(500, 0, 500), ref hit)) 
            endPoint = hit;

        flyingTime = Vector3.Distance(startPoint, endPoint) / 100.0f;
    }

    [PunRPC]
    void SetPath(Vector3 _startPoint, Vector3 _endPoint, float _distance)
    {
        transform.position = _startPoint;
        startPoint = _startPoint;
        endPoint = _endPoint;
        flyingTime = _distance;
        transform.rotation = Quaternion.LookRotation(_endPoint - _startPoint);
        audioPlayer.PlayOneShot(flyingClip);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            //transform.position = (Vector3)stream.ReceiveNext();
            //HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }

    public bool GetIntersectPoint(Vector3 AP1, Vector3 AP2, Vector3 BP1, Vector3 BP2, ref Vector3 IP) 
    {
        float t;
        float s;
        float under = (BP2.z - BP1.z) * (AP2.x - AP1.x) - (BP2.x - BP1.x) * (AP2.z - AP1.z);
        if(under==0) return false;

        float _t = (BP2.x - BP1.x) * (AP1.z - BP1.z) - (BP2.z - BP1.z) * (AP1.x - BP1.x);
        float _s = (AP2.x - AP1.x) * (AP1.z - BP1.z) - (AP2.z - AP1.z) * (AP1.x - BP1.x);

        t = _t/under;
        s = _s/under; 

        if(t<0.0 || t>1.0 || s<0.0 || s>1.0) return false;
        if(_t==0 && _s==0) return false; 

        IP.x = AP1.x + t* (float) (AP2.x-AP1.x);
        IP.z = AP1.z + t* (float) (AP2.z-AP1.z);

        return true;
    }
}
