using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

struct KillLog
{
    public KillLog(GameObject _logObj, float _addedTime) { logObj = _logObj; addedTime = _addedTime; }
    public GameObject logObj;
    public float addedTime;
}


public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance = null;



    [SerializeField]
    public Image hp;
    private GameObject killLogPlane;
    private GameObject killLogPrefab;

    private TextMeshProUGUI notice;
    private TextMeshProUGUI bulletNotice;
    private TextMeshProUGUI isMaster;




    private Queue<KillLog> queKillLog;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        queKillLog = new Queue<KillLog>();
        notice = GameObject.Find("Notice").GetComponent<TextMeshProUGUI>();
        bulletNotice = GameObject.Find("BulletNotice").GetComponent<TextMeshProUGUI>();
        isMaster = GameObject.Find("IsMaster").GetComponent<TextMeshProUGUI>();

        killLogPlane = GameObject.Find("KillLogPlane");
        hp = GameObject.Find("hp").GetComponent<Image>();
        killLogPrefab = Resources.Load<GameObject>("KillLog");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateKillLog();

        //if (PhotonNetwork.IsMasterClient) 
        //    isMaster.text = "It's master";

    }

    private void UpdateKillLog()
    {
        KillLog killLog;
        while (queKillLog.Count > 0)
        {
            killLog = queKillLog.Peek();
            if (killLog.addedTime + 3.0f < Time.time)
            {
                queKillLog.Dequeue();
                GameObject.Destroy(killLog.logObj);
            }
            else
                break;
        }

    }

    public void AddKillLog(string _murderer, string _victom)
    {
        GameObject killLog  = Instantiate(killLogPrefab);

        killLog.transform.SetParent(killLogPlane.transform);
        killLog.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250.0f, 0.0f);

        killLog.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _murderer;
        killLog.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _victom;

        queKillLog.Enqueue(new KillLog(killLog, Time.time));
    }

    public void SetHp(float _percent)
    {
        hp.fillAmount = _percent;
    }

    public void ShowNotice(string _notice, float _duration)
    {
        notice.text = _notice;
        if (_duration > 0f)
            StartCoroutine(ClearNotice(_duration));
    }

    public void ShowBulletNotice(int _cur, int _max)
    {
        bulletNotice.text = _cur.ToString() + " / " + _max.ToString();
    }

    IEnumerator ClearNotice(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        notice.text = "";
    }
}
