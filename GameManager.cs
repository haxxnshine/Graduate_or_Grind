using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //GameManager에 Player를 만들어서 관리한다. 
    //GameManager를 정적변수로 만들어서 관리하기 편하도록 할것이다. 
    //정적변수는 즉시 클래스에서 부를 수 있다는 편리함이 있다. 
    //Static으로 만든 변수는 하이라키에 보이지 않는다. 
    //GameManager를 메모리에 올려서 관리한다. 
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive; //시간이 정지해 있는지를 판단하기 위해서 변수 선언
    
    //레벨을 조정하기 위해서 게임시간과 최대게임시간 변수를 선언
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;   
    public int kill;
    public int exp;
    public int[] nextExp = {3, 5, 10, 100, 150, 210, 280, 360, 450, 600}; 
        //각 레벨의 필요 경험치를 보관할 배열 변수 선언 및 초기화

    [Header("#Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoy;
    public GameObject enemyCleaner;


    //초기화를 스크립트 내에서 해줘야햠 
    void Awake()
    {
        instance = this; //정적인 변수 자기자신을 집어넣어야 함
        Application.targetFrameRate = 60; 
        //게임매니저에서 targetFrameRate속성을 직접 설정, 지정해주지 않으면 기본 30
    }

    public void GameStart(int id) //int 변수를 추가
    {
        playerId = id;
        
        health = maxHealth;

        //플레이어 활성화
        player.gameObject.SetActive(true);

        //기본 무기 지급을 위한 함수 호출에서 인자 값을 캐릭터 ID로 변경
        uiLevelUp.Select(playerId % 2);

        Resume();

        //배경음 시작을 게임 시작부분에 호출 
        AudioManager.instance.PlayBgm(true);
        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    //딜레이를 위해 게임오버 코루틴도 작성
    IEnumerator GameOverRoutine()
    {
        //작동을 멈추기
        isLive = false;
        //0.5초를 기다리고
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        //완전히 다 멈추기
        Stop();

        //배경음 종료를 게임 종료부분에 호출 
        AudioManager.instance.PlayBgm(false);
        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    //딜레이를 위해 게임오버 코루틴도 작성
    IEnumerator GameVictoryRoutine()
    {
        //작동을 멈추기
        isLive = false;
        enemyCleaner.SetActive(true);
        //0.5초를 기다리고
        yield return new WaitForSeconds(1.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        //완전히 다 멈추기
        Stop();

        //배경음 종료를 게임 종료부분에 호출 
        AudioManager.instance.PlayBgm(false);
        //효과음을 재생할 부분마다 재생함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
        //LoadScene : 이름 혹은 인덱스로 장면을 다시 새롭게 부르는 함수
    }

    public void GameQuit()
    {
        Application.Quit(); //게임을 종료하는 함수 실행 
        //에디터를 종료하는 기능이 아니므로, 빌드 버전에서만 작동
    }

     void Update()
    {
        if(!isLive)
            return; //isLive가 아닌데 Update 시에 시간이 추가되지 않도록 조건 추가
        
        gameTime += Time.deltaTime;

        //0.2초에 적 한마리씩 생성
        if (gameTime > maxGameTime){
            gameTime = maxGameTime;
            GameVictory();
        }

    }

    public void GetExp()
    {
        if(!isLive) //EnemyCleaner 발동시 경험치 얻지 못하게 처리
            return;

        exp++;

        //if 조건으로 최대 필요 경험치에 도달하면 최대필요 경험치로 계속해서 레벨 업하도록 작성
        //레벨업 시 필요 경험치의 최대치가 100이면 그 다음 레벨업 시에도 똑같이 100으로 진행된다. 
        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]){
            level ++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; //timeScale : 유니티의 시간 속도(배율) 0배가 됨
        uiJoy.localScale = Vector3.zero; // 멈췄을 때 조이스틱 안보이게
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1; //시간 속도 다시 1배 
        uiJoy.localScale = Vector3.one; // 다시 시작했을 때 조이스틱 보이게 
    }
}
