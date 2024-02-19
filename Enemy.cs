using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait; //다음 fixedUpdate가 될때까지 기다리는 변수

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
            return;
        
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return; //만약 몬스터가 죽은 상태이거나 맞는 상태이면(넉백위해서) 작동하지 않음

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; //다음에 가야할 위치의 양
        rigid.MovePosition(rigid.position + nextVec);
        //플레이어의 키입력 값을 더한 이동 = 몬스터의 방향 값을 더한 이동
        rigid.velocity = Vector2.zero; //몬스터와 플레이어가 부딪힐 때 발생하는 속도를 (0,0)으로 고정
    }   

    void LateUpdate()
    {
        if(!GameManager.instance.isLive)
            return;
        
        if(!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        //재활용 하기 위해 dead상태에서 다시 원상복구하기
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data) //매개변수로 소환데이터 하나 지정
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    //무기와 적이 다았을때 이벤트 시스템
    void OnTriggerEnter2D(Collider2D collision)
    { //collision = 지금 충돌한 상대
        if (!collision.CompareTag("Bullet") || !isLive) 
            //지금 충돌한게 "Bullet"이 맞습니까라고 확인 & 사망 로직이 연달아 실행되는 것을 방지하기 위해 조건 추가
            return;
        
        health -= collision.GetComponent<Bullet>().damage; //맞은 무기의 데미지만큼 체력에서 깎기
        //코루틴은 StartCoroutine으로 호출
        StartCoroutine(KnockBack()); //StartCoroutine("KnockBack") 도 가능

        if (health > 0) {
            anim.SetTrigger("Hit");
            //효과음을 재생할 부분마다 재생함수 호출
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else{
            isLive = false;
            coll.enabled = false; //컴포넌트 비활성화
            rigid.simulated = false; //rigidbody 물리적 비활성화
            spriter.sortingOrder = 1; //스프라이트 랜더러의 sorting order(보이는 순서) 감소
            anim.SetBool("Dead", true); //setBool 함수를 통해 죽는 애니메이션으로 전환
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive){
                //효과음을 재생할 부분마다 재생함수 호출
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);}
        }

        
    }

    //코루틴 Coroutine : 생명 주기와 비동기처럼 실행되는 함수
    //IEnumerator : 코루틴만의 반환형 인터페이스
    IEnumerator KnockBack()
    {
        //yield : 코루틴의 반환 키워드
        //yield return new WaitForSeconds(2f); //2초 쉬기 
        yield return wait; //다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); //순간적인 힘이므로 ForceMode2D.Impulse속성 추가
                                        // 넉백 받는 힘 곱해서 추가

    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
}
