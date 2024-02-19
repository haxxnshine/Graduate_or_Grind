using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //프리팹을 보관할 변수
    public GameObject[] prefabs; // [] : 여러개를 넣을 수 있는 배열 형태인 것을 지정
    //풀 다담을 하는 리스트들
    List<GameObject>[] pools; //List<타입>[배열] 

    void Awake()
    {
        //풀을 담는 배열 초기화
        pools = new List<GameObject>[prefabs.Length];
        //배열 안에 들어있는 각각의 리스트도 순회하면서 초기화
        for (int index = 0; index < pools.Length; index++){ //index가 0부터 시작하므로 프리팹이 2개이면 0,1로 2번 반복
            pools[index] = new List<GameObject>(); //생성자의 함수 의미로 (); 
        }

        //Debug.Log(pools.Length); //콘솔창에 pools.Length 출력

        
    }

    public GameObject Get(int index) //가져올 오브젝트의 종류를 결정하는 매개변수 추가
        {
            GameObject select = null;
            
            // 선택한 풀의 놀고 (비활성화된) 있는 게임오브젝트 접근
            

            foreach (GameObject item in pools[index]){ // 배열, 리스트를 순회해서 순차적으로 데이터에 접근하는 반복문
                if(!item.activeSelf){ ///내용물 오브젝트가 비활설화(대기상태)인지 확인
                    // 발견하면 select 변수에 할당
                    select = item;
                    select.SetActive(true);
                    break;
                } 
            } 
            
            // 못찾았으면 ? 
            if(select == null){  //select == null 은 !select로 해도 됨
                //새롭게 생성하고 select 변수에 할당
                select = Instantiate(prefabs[index], transform); // Instantiate : 원본 오브젝트를 복제하여 장면에 생성하는 함수 
                                                                /// transform : 내 자신 안에다가 넣겠다 
                                                                /// (없으면 hierachy창에 추가되어 보기 지저분)
                pools[index].Add(select);
            }
            return select;
        }
}
