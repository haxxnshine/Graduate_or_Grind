using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    
    public float speed; //속도 관리 변수
    public Scanner scanner; //플레이어 스크립트에서 검색 클래스 타입 변수 선언 및 초기화
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon; //여러 애니메이터 컨트롤러를 저장할 배열 변수 

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true); //인자값에 true를 넣으면 비활성화된 오브젝트도 인식한다. 
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }

    // void Update()
    // {
    //      if(!GameManager.instance.isLive)
    //          return;
    //     inputVec.x = Input.GetAxisRaw("Horizontal");
    //     inputVec.y = Input.GetAxisRaw("Vertical");
    //     // GetAxisRaw로 더욱 명확한 컨트롤 가능 딱딱 숫자가 떨어지는 컨트롤
    //     // GetAxis로 하면 자동 보정됨, 좀 미끄러짐
    // }
    
    //FixedUpdate : 물리 연산 프레임마다 호출되는 생명주기 함수
    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
            return;
        
        // // 1. 힘을 준다
        // rigid.AddForce(inputVec);
        // // 2. 속도 제어
        // rigid.velocity = inputVec;

        // 3. 위치 이동
        // rigid.position : 현재 위치
        // 다른 프레임에도 이동 거리는 같아야 한다
        Vector2 nextVec = inputVec* speed * Time.fixedDeltaTime; 
                            // normalized : 벡터 값의 크기가 1이 되도록 좌표가 수정된 값 
                            // 만약 normalized를 쓰지 않으면 사실상 대각선을 갈 때 루트 2만큼 가므로 
                            // 더 빠르게 이동한다. 
                            // fixedDeltaTime : 물리 프레임 하나가 소비한 시간
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        if(!GameManager.instance.isLive)
            return;
        
        anim.SetFloat("Speed", inputVec.magnitude); //magnitude : 백터의 순수한 크기만 주는 값

        if (inputVec.x != 0){
            spriter.flipX = inputVec.x < 0; //비교연산자 
        }
    }

    //플레이어 피격 시 함수 설정    
    void OnCollisionStay2D(Collision2D collision)
    {
        if(!GameManager.instance.isLive)
            return;

        GameManager.instance.health -= Time.deltaTime * 10;
        //프레임마다 적용되어 너무 빠르게 피격피해가 나지 않도록 Time.deltaTime 활용

        if (GameManager.instance.health < 0)
        {
            //묘비 애니메이션을 보여줄 instance를 제외하고 비활성화
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}
