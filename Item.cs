using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI; //이걸 받아오지 않으면 Image 사용 불가

public class Item : MonoBehaviour
{
    public ItemData data;  //설정한 아이템 데이터 가져오기
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1]; 
        //자식 오브젝트의 컴포넌트가 필요하므로 GetComponentsInChildren 사용
        //GetComponentsInChildren에서 두번째 값 가져오기 (첫번째는 자기자신)
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        //GetComponents의 순서는 inspector 상 계층 구조의 순서를 따라간다. 
        //따라서 inspector 상의 순서를 제대로 해줘야 text의 순서가 바뀌지 않는다. 
        textLevel = texts[0]; 
        textName = texts[1]; 
        textDesc = texts[2]; 
        textName.text = data.itemName;
    }

    //사라졌다가 나타났다가 할 것이므로 활성화되었을 때 자동으로 실행되는 이벤트 함수를 활용
    void OnEnable(){
        textLevel.text = "Lv." + (level + 1);
        //아이템 타입별로 설명 유무가 다르므로 switch로 케이스 나누기
        switch(data.itemType){
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                //데미지 % 상승을 보여줄 땐 100 곱하기
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level]);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }

    }
    //void LateUpdate()
    //{
        //// textLevel.text = "LV." + (level + 1); //이렇게 하면 레벨 1부터 시작
        //textLevel.text = "LV." + level; //이렇게 하면 레벨 0부터 시작
        //무기가 몇 레벨인지 측정
    //}

    public void OnClick()
    {
        switch (data.itemType){
            case ItemData.ItemType.Melee: 
            case ItemData.ItemType.Range:
            //여러개의 case를 붙여서 로직을 실행하게 할 수 있음
            //같은 로직이므로 같이 묶어서 코드 실행
            if (level == 0) {
                GameObject newWeapon = new GameObject(); //새로운 게임오브젝트를 코드로 생성
                weapon = newWeapon.AddComponent<Weapon>(); 
                //AddComponent<T> : 게임 오브젝트에 T 컴포넌트를 추가하는 모습
                //AddComponenet 함수 반환 값을 미리 선언한 변수에 저장
                weapon.Init(data); //weapon의 데이터를 초기화
            }
            else {
                float nextDamage = data.baseDamage;
                int nextCount = 0;

                //처음 이후의 레벨업은 데미지와 횟수를 계산
                nextDamage += data.baseDamage * data.damages[level];
                nextCount += data.counts[level];

                //weapon에 작성된 레벨업 함수를 활용하여 레벨업 적용
                weapon.LevelUp(nextDamage, nextCount);
            }
            level++;
            break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0) {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>(); //AddComponenet 함수 반환 값을 미리 선언한 변수에 저장
                    gear.Init(data);
                }
                else{
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
            level++;
            break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
            break;
        }

        //버튼이 최대레벨로 도달하면 버튼 클릭이 불가능하도록 설정
        if (level == data.damages.Length){
            GetComponent<Button>().interactable = false;
        }
    }
}
