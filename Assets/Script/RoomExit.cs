using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomExit : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameObject.Find("Canvas").transform.Find("BlurEffect").gameObject.SetActive(true);
        //GameObject.Find("BlurEffect").SetActive(true);
    }

    private void OnDisable()
    {
        GameObject.Find("Canvas").transform.Find("BlurEffect").gameObject.SetActive(false);
        //GameObject.Find("BlurEffect").SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        PhotonNetwork.LoadLevel("Menu");
    }
}
