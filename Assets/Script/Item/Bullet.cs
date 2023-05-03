using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    private Vector3 dir;

    [SerializeField]
    private GameObject footstep;
    private Collider collid;

    void Start()
    {
        collid = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        dir.y -= 9.8f * Time.deltaTime;
        dir.Normalize();
        dir *= 100.0f;

        // RigidBody�� �߷��� �����Ͽ� ��������� ������ �̵�
        transform.position += dir * Time.deltaTime;

        Instantiate(footstep, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
    }

    PlayerModel shooter;
    float damage;
    Vector3 hitPosition = Vector3.zero;

    public void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �÷��̾� �� ���
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerCombat target = collision.gameObject.GetComponent<PlayerCombat>();
            // RPC�� �̿��� ������ �޴� ������ ����ȭ
            shooter.pv.RPC("RPCApplyDamage", RpcTarget.All, damage, shooter.pv.ViewID, target.pv.ViewID);
        }

        hitPosition = collision.transform.position;
    }

    public void SetBulletTransform(Vector3 _pos, Vector3 _dir)
    {
        transform.position = _pos;
        transform.rotation = Quaternion.LookRotation(_dir);
        dir = _dir;
        dir.Normalize();
        dir *= 100.0f;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Map") BulletPool.ReturnObject(this);
        if (collision.tag == "Map") return;
    }


}
