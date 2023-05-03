using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notice : MonoBehaviour
{
    private TextMesh textNotice;

    void Start()
    {
        textNotice = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowNotice(string _text, float _period)
    {
        StopCoroutine(NoticeText(_text, _period));
        StartCoroutine(NoticeText(_text, _period));
    }

    IEnumerator NoticeText(string _text, float _period)
    {
        textNotice.text = _text;

        yield return new WaitForSeconds(_period);
        textNotice.text = "";
    }
}
