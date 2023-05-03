using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class InGameManager : MonoBehaviour
{
    PhotonView pv;

    [SerializeField]
    private GameObject electricField;
    [SerializeField]
    private GameObject airplane;

    [SerializeField]
    private GameObject centerNotice;
    private TextMeshProUGUI centerText;
    [SerializeField]
    private GameObject upRightNotice;
    private TextMeshProUGUI upRightText;

    private int totalPlayers;
    private int alivedPlayers;
    private int killCount;
    private bool isDead;

    private void Awake()
    {
        //gameObject.SetActive(false);
    }
    void Start()
    {
        pv = GetComponent<PhotonView>();
        centerText = centerNotice.GetComponent<TextMeshProUGUI>();
        upRightText = upRightNotice.GetComponent<TextMeshProUGUI>();
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            ShowChicken();
    }

    public void Dead()
    {
        isDead = true;
    }
    public void UpdatePlayerCount(int _alivedPlayers)
    {
        pv.RPC("UpdatePlayerCount", RpcTarget.All, alivedPlayers);
    }

    public void KillPlayer(int _victomID, int _attackerID)
    {
        alivedPlayers--;
        //killCount++;
        pv.RPC("RPCUpdatePlayerCount", RpcTarget.All, alivedPlayers);

        //if (alivedPlayers <= 1 && PhotonNetwork.GetPhotonView(_attackerID) && PhotonNetwork.GetPhotonView(_attackerID).IsMine)
        //{
        //    GameObject.Find("Canvas").transform.Find("ExitRoom").gameObject.SetActive(true);
        //    GameObject.Find("Chicken").GetComponent<TextMeshProUGUI>().enabled = true;
        //} else if (alivedPlayers <= 1 && !isDead)
        //{
        //    GameObject.Find("Canvas").transform.Find("ExitRoom").gameObject.SetActive(true);
        //    GameObject.Find("Chicken").GetComponent<TextMeshProUGUI>().enabled = true;
        //}
    }

    [PunRPC]
    public void RPCUpdatePlayerCount(int _alivedPlayers)
    {
        alivedPlayers = _alivedPlayers;
        GameObject.Find("PeopleCount").GetComponent<TextMeshProUGUI>().text = alivedPlayers + " / " + totalPlayers;
        if (alivedPlayers <= 1 && !isDead)
        {
            GameObject.Find("Canvas").transform.Find("ExitRoom").gameObject.SetActive(true);
            GameObject.Find("Chicken").GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    public void ShowChicken()
    {
        GameObject.Find("Canvas").transform.Find("ExitRoom").gameObject.SetActive(true);
        GameObject.Find("Chicken").GetComponent<TextMeshProUGUI>().enabled = true;
    }

    public void StartGame()
    {
        //Instantiate(airplane);
        if (PhotonNetwork.IsMasterClient)
            SpawnItems();

        alivedPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        killCount = 0;
        GameObject.Find("PeopleCount").GetComponent<TextMeshProUGUI>().text = alivedPlayers + " / " + totalPlayers;
        //UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        foreach(var it in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (it.GetComponent<PhotonView>().IsMine)
                it.GetComponent<PlayerModel>().InitializeNickname(it.GetComponent<PlayerModel>().playerName);
        }
    }

    private void SpawnItems()
    {
        print("SpawnItems");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 from = new Vector3();
        Vector3 dir = new Vector3();
        RaycastHit hit;

        int count = 0;

        while (count < 900)
        {
            from.x = Random.Range(-1500, 200);
            from.z = Random.Range(-1500, 200);
            from.y = 600f;
            dir = Vector3.down;
            if (Physics.Raycast(from, dir, out hit, 1000f, -LayerMask.NameToLayer("Map")))
            {
                if (hit.point.y > 50f)
                    continue;
                PhotonNetwork.InstantiateRoomObject(GetRandomItem(), hit.point + (Vector3.up * 0.2f), Quaternion.Euler(0f, 0f, 90f));//.transform.localScale = new Vector3(10f, 10f, 10f);
                count++;
            }
        }
    }

    private string GetRandomItem()
    {
        int rnd = Random.Range(0, 500);
        if (rnd % 6 == 0) return "Weapon/m4";
        if (rnd % 6 == 1) return "Weapon/M110";
        if (rnd % 6 == 2) return "Weapon/M249";
        if (rnd % 6 == 3) return "Weapon/mp5";
        else  return "BulletBox";
    }
}
