using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
 
    public void Init(float damage, int per, Vector3 dir) //안에 있는거 받는 값 즉 parameter
    {
        this.damage = damage;  //this : 해당 클래스의 변수로 접근  
            // this.damage = Bullet 함수 내에 damage, 그냥 damage = Init함수에 받아오는 매개변수 damage
        this.per = per;

        if(per >= 0) { //관통이 무한이 아니면 원거리
            rigid.velocity = dir * 15f;  //속도를 제어
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Enemy") || per == -100) // || 는 or 이다
            return;
        
        per --;

        if (per < 0) {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    //총알이 맵 밖으로 나가는 경우 없어지도록 처리 
    void OnTriggerExit2D(Collider2D collision)
    {
        //OnTriggerExit2D 이벤트와 Area를 활용하여 쉽게 비활성화
        if(!collision.CompareTag("Area") || per == -100)
            return;
        
        gameObject.SetActive(false);
    }
}
