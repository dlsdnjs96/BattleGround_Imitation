using System.Collections;
using Photon.Pun;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviourPunCallbacks, IPunObservable {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    public LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리
    public AudioClip pickupClip; 


    public float damage = 25; // 공격력
    public float fireDistance = 50f; // 사정거리
    
    public int magCapacity = 25; // 탄창 용량
    public int magAmmo; // 현재 탄창에 남아있는 탄약

    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간
    private float lastFireTime; // 총을 마지막으로 발사한 시점

    public PlayerModel playerModel;
    private PhotonView pv;
    private Vector3 curPos;

    //public virtual void Shot() { }
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (pv.IsMine)
        {

        } else
        {
            //transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime);
        }
    }

    public void Shot()
    {
        // 실제 발사 처리는 호스트에게 대리
        //pv.RPC("ShotProcessOnServer", RpcTarget.MasterClient);
        ShotProcessOnServer();
        // 남은 탄환의 수를 -1
        magAmmo--;
        if (magAmmo <= 0)
        {
            // 탄창에 남은 탄약이 없다면, 총의 현재 상태를 Empty으로 갱신
            state = State.Empty;
        }
        PlayerUI.instance.ShowBulletNotice(magAmmo, playerModel.curBulletCount);
    }
    // 호스트에서 실행되는, 실제 발사 처리
    //[PunRPC]
    private void ShotProcessOnServer()
    {
        Vector3 hitPosition = Vector3.zero;

        RaycastHit hitRay;

        // 총구 위치에서 Raycast 시도
        if (Physics.Raycast(fireTransform.position, -fireTransform.up, out hitRay, fireDistance))
        {
            // 충돌한 오브젝트가 플레이어 일 경우
            if (hitRay.collider.gameObject.CompareTag("Player"))
            {
                PlayerCombat target = hitRay.collider.gameObject.GetComponent<PlayerCombat>();
                // RPC를 이용해 데미지 받는 내용을 동기화
                pv.RPC("RPCApplyDamage", RpcTarget.All, damage, playerModel.pv.ViewID, target.pv.ViewID);
            }

            hitPosition = hitRay.point;
        }
        else
        {
            // 레이가 다른 물체와 충돌하지 않았다면
            // 총알이 최대 사정거리까지 날아갔을때의 위치를 충돌 위치로 사용
            hitPosition = fireTransform.position +
                          fireTransform.forward * fireDistance;
        }

        // 발사 이펙트 재생, 이펙트 재생은 모든 클라이언트들에서 실행
        pv.RPC("ShotEffectProcessOnClients", RpcTarget.All, hitPosition);
    }

    // 이펙트 재생 코루틴을 랩핑하는 메서드
    [PunRPC]
    private void RPCApplyDamage(float _damage, int _attackerID, int _victomID)
    {
        PhotonNetwork.GetPhotonView(_victomID).GetComponent<PlayerCombat>().ApplyDamage(_damage, _attackerID);
    }
    [PunRPC]
    private void ShotEffectProcessOnClients(Vector3 hitPosition)
    {
        StartCoroutine(ShotEffect(hitPosition));
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        // 총구 화염 효과 재생
        muzzleFlashEffect.Play();
        // 탄피 배출 효과 재생
        //shellEjectEffect.Play();

        // 총격 소리 재생
        gunAudioPlayer.PlayOneShot(shotClip);

        // 선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, transform.position);
        // 선의 끝점은 입력으로 들어온 충돌 위치
        bulletLineRenderer.SetPosition(1, hitPosition);
        // 라인 렌더러를 활성화하여 총알 궤적을 그린다
        bulletLineRenderer.enabled = true;

        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }
    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.IsWriting)
        {
            stream.SendNext(magAmmo);
            stream.SendNext(state);
            //stream.SendNext(transform.position);
        }
        else
        {
            magAmmo = (int) stream.ReceiveNext();
            state = (State) stream.ReceiveNext();
            //transform.position = (Vector3)stream.ReceiveNext();
        }
    }



    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // 사용할 점을 두개로 변경
        bulletLineRenderer.positionCount = 2;
        // 라인 렌더러를 비활성화
        bulletLineRenderer.enabled = false;
    }


    private void OnEnable() {
        // 현재 탄창을 가득채우기
        magAmmo = 30;
        // 총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        state = State.Ready;
        // 마지막으로 총을 쏜 시점을 초기화
        lastFireTime = 0;

    }



    // 발사 시도
    public void Fire() {
        // 현재 상태가 발사 가능한 상태
        // && 마지막 총 발사 시점에서 timeBetFire 이상의 시간이 지남

        if (state == State.Ready
            && Time.time >= lastFireTime + timeBetFire)
        {
            // 마지막 총 발사 시점을 갱신
            lastFireTime = Time.time;
            // 실제 발사 처리 실행
            Shot();
        }
    }



    // 재장전 시도
    public bool Reload() {
        print("Reload " + state + " " + playerModel.curBulletCount + " " + magAmmo + " " + magCapacity);
        if (state == State.Reloading ||
            playerModel.curBulletCount <= 0 || magAmmo >= magCapacity)
        {
            // 이미 재장전 중이거나, 남은 총알이 없거나
            // 탄창에 총알이 이미 가득한 경우 재장전 할수 없다
            return false;
        }

        // 재장전 처리 실행
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        print("ReloadRoutine");
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;
        // 재장전 소리 재생
        gunAudioPlayer.PlayOneShot(reloadClip);

        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);

        // 탄창에 채울 탄약을 계산한다
        int ammoToFill = magCapacity - magAmmo;

        // 탄창에 채워야할 탄약이 남은 탄약보다 많다면,
        // 채워야할 탄약 수를 남은 탄약 수에 맞춰 줄인다
        if (playerModel.curBulletCount < ammoToFill)
        {
            ammoToFill = playerModel.curBulletCount;
        }

        // 탄창을 채운다
        magAmmo += ammoToFill;
        // 남은 탄약에서, 탄창에 채운만큼 탄약을 뺸다
        playerModel.curBulletCount -= ammoToFill;

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
        PlayerUI.instance.ShowBulletNotice(magAmmo, playerModel.curBulletCount);
    }
}