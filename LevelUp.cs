using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    //보이는 함수 생성
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        //스케일을 1,1,1 로 만들어서 원상복귀
        GameManager.instance.Stop();

        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        //배경음 잠깐 끄기
        AudioManager.instance.EffectBgm(true);
    }

    //숨기는 함수 생성
    public void Hide()
    {
        rect.localScale = Vector3.zero; 
        //스케일을 0,0,0으로 만들어서 창 크기를 아예 0으로 없애버림

        GameManager.instance.Resume();

        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        //배경음 다시 켜기
        AudioManager.instance.EffectBgm(false);
    }
    
    public void Select(int index)
    {
        items[index].OnClick();

    }

    void Next()
    {
        //1. 모든 아이템 비활성화
        foreach (Item item in items){
            item.gameObject.SetActive(false);
        }

        //2. 그 중에서 랜덤 3개 아이템 활성화
        int[] ran = new int[3];//랜덤으로 활성화 할 아이템의 인덱스 3개를 담을 배열 선언
        while(true){
            ran[0] = UnityEngine.Random.Range(0, items.Length);
            //0부터 4번까지 랜덤으로 하나 정하기
			//버전업 때문에 UnityEngine.Random이라고 안하면 모호하다는 에러가 뜸
			//본래는 그냥 Random.Range(0, items.Length);
            ran[1] = UnityEngine.Random.Range(0, items.Length);
            ran[2] = UnityEngine.Random.Range(0, items.Length);

            if(ran[0] != ran[1] && ran[1]!= ran[2] && ran[0]!= ran[2])
                break;
        }

        for (int index=0; index < ran.Length; index++){
            Item ranItem = items[ran[index]];

            //3. 만렙 아이템의 경우는 소비아이템으로 대체
            if(ranItem.level == ranItem.data.damages.Length){
                items[4].gameObject.SetActive(true);
                //소비아이템이 여러개인 경우 (예: 4번부터 7번까지 소비아이템일때)
                //items[Random.Range(4,7)].gameObject.setActive(true);
            }
            else{
                ranItem.gameObject.SetActive(true);
            }
            
        }
        
    }
}
