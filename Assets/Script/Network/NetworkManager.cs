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
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = "nick";// NickNameInput.text;
    }



    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) => print("연결끊김");



    public void JoinLobby() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby() => print("로비접속완료");



    public void CreateRoom() => PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxConnect });

    public void JoinRoom() => PhotonNetwork.JoinRoom(roomName);

    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = maxConnect }, null);

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnCreatedRoom() => print("방만들기완료");

    public override void OnJoinedRoom()
    {
        print("방참가완료"); }

    public override void OnCreateRoomFailed(short returnCode, string message) => print("방만들기실패");

    public override void OnJoinRoomFailed(short returnCode, string message) { 
        print("방참가실패");
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) => print("방랜덤참가실패");

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


    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
}