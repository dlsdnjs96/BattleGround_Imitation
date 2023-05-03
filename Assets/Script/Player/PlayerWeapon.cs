using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPun
{
    public Gun gun; // 사용할 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private PlayerControl playerControl; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트


    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;


        // 입력을 감지하고 총 발사하거나 재장전
        if (playerControl.fire)
        {
            // 발사 입력 감지시 총 발사
            gun.Fire();
        }
        else if (playerControl.reload)
        {
            // 재장전 입력 감지시 재장전
            if (gun.Reload())
            {
                // 재장전 성공시에만 재장전 애니메이션 재생
                playerAnimator.SetTrigger("Reload");
            }
        }

        // 남은 탄약 UI를 갱신
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (gun != null && PlayerUI.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            //PlayerUI.getInstance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }
}
