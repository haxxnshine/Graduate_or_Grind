using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    
    void OnTriggerExit2D(Collider2D collisioin)
    {
        if (!collisioin.CompareTag("Area"))
            return;
        
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        

        switch (transform.tag){
            case "Ground" : 
                //두 오브젝트의 위치 차이를 활용한 로직
                float diffX = playerPos.x - myPos.x; // 플레이어의 거리와 나의 거리 차이 확인함
                float diffY = playerPos.y - myPos.y;

                float dirX = diffX < 0 ? -1 : 1; //3항연산자 (조건) ? (참일때 값) : (거짓일때 값)
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX); //절댓값으로 환산
                diffY = Mathf.Abs(diffY);

                if(diffX > diffY){
                    transform.Translate(Vector3.right * dirX * 40);
                }
                else if(diffY > diffX){
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy" : 
                if (coll.enabled){
                    Vector3 dist = playerPos - myPos;
                    //두 오브젝트의 거리를 그대로 활용하는 것이 포인트
                    Vector3 ran = new Vector3(UnityEngine.Random.Range(-3,3),UnityEngine.Random.Range(-3,3) ,0);
                    transform.Translate(ran + dist * 2); 
                    //랜덤 벡터를 더하여 퍼져있는 몬스터 재배치 만들기 +
                    //기존 위치의 2배를 더해서 이동하게 하면 반대방향으로 나오므로 해당 로직 활용
                }
                break;
        }
    }
}
