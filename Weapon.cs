using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // poolManaager에서 받은 무기를 모양새 있게 관리하는 역할
    public int id; //무기 id
    public int prefabId; //프리펩 id
    public float damage;//데미지
    public int count; //개수. 몇 개의 무기를 배치할 것이냐
    public float speed; //회전속도 혹은 연사 속도

    float timer;
    Player player;

    void Awake(){
        player = GameManager.instance.player; //게임메니저 활용으로 초기화
    }

    void Update()
    {
        if(!GameManager.instance.isLive)
            return;
        
        //무기 id에 따라 로직을 분리할 switch문 작성
        switch (id){
            case 0: //근접무기 : 삽
                transform.Rotate(Vector3.back * speed * Time.deltaTime); //회전 속도에 맞춰서 돌도록 하기
                break;
            default:
                timer += Time.deltaTime; //deltaTime : 한 프레임이 소비하는 시간

                if (timer > speed) {
                    timer = 0f; //speed 보다 커지면 초기화하면서 발사
                    Fire();
                }
                break;
        }

        //TestCode
        if (Input.GetButtonDown("Jump")) 
            LevelUp(10, 1);
    }

    public void LevelUp(float damage,int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); 
        //아이템을 누를 때 데미지 카운트가 적용되므로, 기어 데미지가 초기화 될 수 있기 때문에 적용
    }

    //초기화함수
    public void Init(ItemData data) //Waepon 초기화 함수에 스크립트블 오브젝트를 매개변수로 받아 활용
    {
        //Basic Set
        name = "Weapon " + data.itemId; //Weapon n 으로 만들어서 
        transform.parent = player.transform; //부모 오브젝트를 플레이어로 지정
        transform.localPosition = Vector3.zero; //지역 위치인 localPosition을 원점으로 변경

        //Property Set
        //각종 무기 속성 변수들을 스크립트블 오브젝트 데이터로 초기화
        id = data.itemId; 
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        //for문으로 프리팹아이디를 풀링 매니저의 변수에서 찾아서 초기화
        //스크립트블 오브젝트의 독립성을 위해서 인덱스가 아닌 프리펩으로 설정
        for (int index = 0 ; index < GameManager.instance.pool.prefabs.Length; index++){
            if (data.projectile == GameManager.instance.pool.prefabs[index]){
                prefabId = index;
                break;
            }
        }
        
        //무기 id에 따라 로직을 분리할 switch문 작성
        switch (id){
            case 0: //근접무기 : 삽
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;
            
            default: //원거리 무기 : 총
                speed = 0.5f * Character.WeaponRate;
                break;
        }
        //Head Set
        Hand hand = player.hands[(int)data.itemType]; //enum의 데이터는 정수 형태로도 사용 가능
                                                    //enum 값 앞에 int 타입을 작성하여 강제 형 변환
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);


        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); 
        //BroadcastMessage : 특정 함수 호출을 모든 자식에게 방송하는 함수
        ///player가 가지고 있는 모든 기어에 한해서 applyGear가 되도록 하는 것
        ///오류를 막기 위해 DontRequireReceiver를 두번째 인자값으로 추가
    }

    void Batch() //Batch : 자료를 모아 두었다가 일괄해서 처리하는 자료처리의 형태
    {
        for (int index = 0; index < count; index++){ 
            Transform bullet; 
            
            //bullet 초기화
            if (index < transform.childCount) {//자신의 자식 오브젝트 개수 확인은 childCount속성 
                bullet = transform.GetChild(index); 
                    //기존 오브젝트를 먼저 활용하고 모자란 것은 풀링에서 가져오기
                    //index가 아직 childCount 범위 내라면 GetChild 함수로 가져오기
            }
            else {
                bullet = GameManager.instance.pool.Get(prefabId).transform;  
                //poolManager에서 원하는 프리팹을 가져오고 무기의 개수(count) 만큼 돌려서 배치
                bullet.parent = transform; //parent 속성을 통해 부모를 내 자신(스크립트가 들어간 곳)으로 변경
            }
                
            
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World); //이동 방향은 Space World 기준으로 
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); //근접 무기는 계속 관통하기 때문에 per(관통)을 무한으로 관통하게 -100로 설정
                                                            //-100  is Infinity Per.
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget) //만약 가장 가까운 타깃이 없다면 실행하지 않음
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position; //크기가 포함된 방향 : 목표 위치 - 나의 위치
        dir = dir.normalized; //normalized : 현재 벡터의 방향은 유지하고 크기를 1로 변환하는 속성

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position; //플레이어 위치에서 쏘는 것으로 고정
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); //FromToRotation : 지정된 축을 중심으로 목표를 향해 회전하는 함수
        bullet.GetComponent<Bullet>().Init(damage, count, dir); //원거리 함수에 맞게 초기화 함수 호출하기

        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
