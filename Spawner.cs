using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    float timer;
    public float levelTime; //소환 레벨 구간을 결정하는 변수

    int level;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); //Transform은 어느 오브젝트나 존재하기 때문에 0이 본인의 Transform이므로
                                                            //원하는 위치를 찾으려면 1부터 시작해야 한다.
        levelTime = GameManager.instance.maxGameTime / spawnData.Length; 
        //최대 시간에 몬스터 데이터 크기로 나누어 자동으로 구간 시간 계산 
    }
    
    void Update()
    {
        if(!GameManager.instance.isLive)
            return;
        
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length -1); //게임 시간에 levelTime을 나누어 레벨 설정
        //FloorToInt : 소수점 아래는 버리고 int형으로 만드는 함수 CellToInt : 소수점 올림

        //소환시간 조건을 소환데이터로 변경
        if (timer > spawnData[level].spawnTime){
            timer = 0;
            Spawn();
        }

        // if (Input.GetButtonDown("Jump")) {
        //     GameManager.instance.pool.Get(1); //0번 해골 1번 좀비
        // }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0); 

        //적의 위치를 spwanpoint를 통해 지정
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;

        enemy.GetComponent<Enemy>().Init(spawnData[level]); 

    }
}
[System.Serializable] 
    //직렬화 (Serialization) : 개체를 저장 혹은 전송하기 위해 변환 Inspeactor 창에서도 볼 수 있도록 설정
public class SpawnData
{
    
    public float spawnTime; //소환 시간
    public int spriteType; //적의 종류
    public int health; //체력
    public float speed; //속도


}
