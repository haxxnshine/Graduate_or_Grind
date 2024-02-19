using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; //UI 컴포넌트를 사용할 때는 UnityEngine.UI 네임스페이스 사용

public class HUD : MonoBehaviour
{
    public enum InfoType{Exp, Level, Kill, Time, Health} //다루게 될 데이터를 미리 열거형 enum으로 선언
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch(type){
            case InfoType.Exp:
                //슬라이더에 적용할 값 : 현재 경험치 / 최대 경험치
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length-1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}",GameManager.instance.level);
                //Format : 각 숫가 인자값을 지정된 형태의 문자열로 만들어주는 함수
                //Format(바꿔서 쓰는 값, 바꾸는 값) F0, F1, F2, ..., : 소수점 자리를 지정
                //{인덱스번호 : 숫자에 대한 형태}
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}",GameManager.instance.kill);
                break;
            case InfoType.Time:
                //흐르는 시간이 아닌 남은 시간부터 구하기
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                //남은 시간을 분과 초로 분리
                int min = Mathf.FloorToInt(remainTime / 60); //60으로 나누어 분을 구하되 FloorToInt로 소수점 버리기
                int sec = Mathf.FloorToInt(remainTime % 60); //60으로 나누고 남은 나머지 
                
                myText.text = string.Format("{0:D2}:{1:D2}",min,sec);
                //D0, D1, D2, ..., : 자리 수를 지정 -> 00:00형식으로 보이게 지정
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
