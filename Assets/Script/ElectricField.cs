using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

struct EFieldData
{
    public int level;
    public int damage;
    public Vector3 pos;
    public float radius;
};

public class ElectricField : MonoBehaviourPun
{
    private GameObject localPlayer;
    private Vector3 prevPos;
    private Vector3 targetPos;
    private EFieldData[] EFieldList = new EFieldData[7];
 


    private int curLevel;
    private bool isNarrowing;
    private float passedTime;

    void Start()
    {

        passedTime = 0.0f;
        isNarrowing = false;
        curLevel = 1;

        FindLocalPlayer();
        MakeEFieldList();
        
        StartCoroutine(ElectornicDamage());
        transform.localScale = new Vector3(2000.0f, 2000.0f, 2000.0f);
    }

    // 1 : 800, 2 : 500, 3 : 300, 4 : 100, 5 : 20, 6 : 1
    // 30 sec each
    void Update()
    {
        passedTime += Time.deltaTime;

        if (passedTime >= 10.0f)
        {
            isNarrowing = !isNarrowing;
            passedTime = 0.0f;

            if (!isNarrowing)
            {
                if (curLevel < 6)
                    curLevel++;
            }
        }

        if (isNarrowing)
        {
            float len = Mathf.Lerp(EFieldList[curLevel - 1].radius, EFieldList[curLevel].radius, passedTime / 10.0f);
            transform.position = Vector3.Lerp(EFieldList[curLevel - 1].pos, EFieldList[curLevel].pos, passedTime / 10.0f);
            transform.localScale = new Vector3(len, len, len);
        }
    }

    void MakeEFieldList()
    {
        for (int i = 1;i < 7;i++)
        {
            EFieldList[i].level = i;
            EFieldList[i].damage = 2 + (i * 2);
        }
        EFieldList[0].radius = 2000.0f;
        EFieldList[1].radius = 1600.0f;
        EFieldList[2].radius = 1000.0f;
        EFieldList[3].radius = 500.0f;
        EFieldList[4].radius = 200.0f;
        EFieldList[5].radius = 50.0f;
        EFieldList[6].radius = 1.0f;

        EFieldList[0].pos = new Vector3(-700, 0, -600);

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i < 7; i++)
            {
                EFieldList[i].pos = Util.GetRandomPointInCircle(EFieldList[i - 1].pos, EFieldList[i - 1].radius - EFieldList[i].radius);
                photonView.RPC("SetEFieldPosition", RpcTarget.OthersBuffered, i, EFieldList[i].pos);
            }
        }
    }

    void FindLocalPlayer()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.GetComponent<PlayerCombat>().pv.IsMine)
            {
                localPlayer = go;
                break;
            }
        }
        return;
    }

    [PunRPC]
    void SetEFieldPosition(int _level, Vector3 _pos)
    {
        EFieldList[_level].pos = _pos;
    }

    IEnumerator ElectornicDamage()
    {
        if (localPlayer)
        {
            if (Vector3.Distance(localPlayer.transform.position, transform.position) > transform.localScale.x)
                localPlayer.SendMessage("DamageByField", Random.Range(EFieldList[curLevel].damage - 5f, EFieldList[curLevel].damage));
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(ElectornicDamage());
    }
}
