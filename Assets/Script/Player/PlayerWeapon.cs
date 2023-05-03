using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPun
{
    public Gun gun; // ����� ��
    public Transform gunPivot; // �� ��ġ�� ������
    public Transform leftHandMount; // ���� ���� ������, �޼��� ��ġ�� ����
    public Transform rightHandMount; // ���� ������ ������, �������� ��ġ�� ����

    private PlayerControl playerControl; // �÷��̾��� �Է�
    private Animator playerAnimator; // �ִϸ����� ������Ʈ


    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;


        // �Է��� �����ϰ� �� �߻��ϰų� ������
        if (playerControl.fire)
        {
            // �߻� �Է� ������ �� �߻�
            gun.Fire();
        }
        else if (playerControl.reload)
        {
            // ������ �Է� ������ ������
            if (gun.Reload())
            {
                // ������ �����ÿ��� ������ �ִϸ��̼� ���
                playerAnimator.SetTrigger("Reload");
            }
        }

        // ���� ź�� UI�� ����
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (gun != null && PlayerUI.instance != null)
        {
            // UI �Ŵ����� ź�� �ؽ�Ʈ�� źâ�� ź��� ���� ��ü ź���� ǥ��
            //PlayerUI.getInstance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }
}
