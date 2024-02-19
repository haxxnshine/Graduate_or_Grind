using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //오른쪽, 왼쪽 구분을 위한 변수 선언
    public bool isLeft;
    public SpriteRenderer spriter;

    SpriteRenderer player;
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35); //왼손의 각회전을 쿼터니언 형태로 저장
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135);
    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1]; // 2번째가 부모의 스프라이트 랜더러이다. 
    }
    
    void LateUpdate()
    {
        bool isReverse = player.flipX;

        if (isLeft){ //근접 무기
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse; //왼쪽 스프라이트는 Y축 반전
            spriter.sortingOrder = isReverse ? 4 : 6; //반전되었을때는 4번째 순서로 아니면 6번째
        }
        else { //원거리 무기
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse; //오른쪽 스프라이트는 X축 반전
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }
}
