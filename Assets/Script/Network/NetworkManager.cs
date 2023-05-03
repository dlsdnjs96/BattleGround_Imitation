using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public readonly byte maxConnect = 4;
    public string roomName = "roomName";
    private int roomNumber;



    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "version 1.0";
        Screen.SetResolution(960, 540, false);
        StatusText = GetComponent<Text>();

        Connect();
        JoinLobby();
    }
    //void Update() => StatusText.text = PhotonNetwork.NetworkClientState.ToString();



    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = "nick";// NickNameInput.text;
    }



    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) => print("�������");



    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby() => print("�κ����ӿϷ�");



    public void CreateRoom() => PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxConnect });

    public void JoinRoom() => PhotonNetwork.JoinRoom(roomName);

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = maxConnect }, null);

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom() => print("�游���Ϸ�");

    public override void OnJoinedRoom()
    {
        print("�������Ϸ�"); }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");

    public override void OnJoinRoomFailed(short returnCode, string message) { 
        print("����������");
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) => print("�淣����������");

    private void JoinRoomRecursive()
    {
        if (PhotonNetwork.InRoom)
        {
            CancelInvoke("JoinRoomRecursive");
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                PhotonNetwork.LoadLevel("Room");
                PhotonNetwork.CurrentRoom.IsOpen = false;
                //PV.RPC("LoadRoomScene", RpcTarget.AllBuffered);
            }
            
            return;
        }
        roomName = "room" + roomNumber;
        roomNumber++;
        JoinRoom();
    }
    public void FindMatching()
    {
        roomNumber = 0;
        InvokeRepeating("JoinRoomRecursive", 1.0f, 1.0f);
    }

    public void CancelMatching()
    {
        CancelInvoke("JoinRoomRecursive");
    }


    [PunRPC]
    void RoomNotice(string _str)
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        //GameObject.Find("Notice").GetComponent<TextMesh>().text = _str;
    }


    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }
}