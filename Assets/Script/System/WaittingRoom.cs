using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class WaittingRoom : MonoBehaviourPunCallbacks
{
    public string roomName;
    private readonly int startTimerCount = 2;
    private readonly int maxPlayerCount = 6;
    private float leftTime;
    private int prevLeftTime;
    private bool startTimer;
    private int prevPlayerCount;



    [SerializeField]
    private GameObject centerNotice;
    private TextMeshProUGUI centerText;
    [SerializeField]
    private GameObject upRightNotice;


    void Start()
    {
        leftTime = 0.0f;
        startTimer = false;
        prevPlayerCount = 0;
        //timer = transform.Find("Timer").gameObject;

        centerText = centerNotice.GetComponent<TextMeshProUGUI>();


        PhotonNetwork.Instantiate("Soldier", new Vector3(Random.Range(-720, -680), 50, Random.Range(-1020, -980)), Quaternion.identity);//.GetComponent<PlayerModel>().InitializeNickname( 
        //GameObject.Find("NetworkManager").GetComponent<NetworkManager>().nickname); 
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= maxPlayerCount) {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            return;
        } else if (!startTimer && PhotonNetwork.CurrentRoom.PlayerCount >= startTimerCount) {
            startTimer = true;
            leftTime = 10.0f;
        }

        if (startTimer && PhotonNetwork.IsMasterClient)
        {
            leftTime -= Time.deltaTime;
            if (prevLeftTime != (int)leftTime)
            {
                prevLeftTime = (int)leftTime;
                if (centerText) centerText.text = prevLeftTime.ToString() + "sec left";
                photonView.RPC("UpdateTime", RpcTarget.Others, prevLeftTime);
                if (prevLeftTime < 1)
                {
                    PhotonNetwork.Instantiate("Airplane", Vector3.zero, Quaternion.identity);
                    photonView.RPC("StartGame", RpcTarget.All);
                    return;
                }
            }
        }

        if (prevPlayerCount != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            prevPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        }
    }


    [PunRPC]
    void UpdateTime(int _time)
    {
        centerText.text = _time.ToString() + "sec left";
    }

    [PunRPC]
    void StartGame()
    {
        centerText.text = "";
        GameObject.Find("InGameManager").GetComponent<InGameManager>().StartGame();
        gameObject.SetActive(false);
    }
}
