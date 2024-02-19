using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")] 
//CreateAssetMenu : 커스텀 메뉴를 생성하는 속성, 이렇게 하면 우클릭시에 scriptable object -> ItemData로 가면 itemData를 편하게 생성할 수 있음
public class ItemData : ScriptableObject
{
    //아이템 타입을 간단하게 관리할 수 있도록 enum으로 저장
    public enum ItemType {Melee, Range, Glove, Shoe, Heal} //근접공격, 원거리공격, 글러브, 신발, 체력포션

    [Header("# Main Info")]
    //아이템의 각 속성을 변수로 적어주기
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon; //스프라이트를 담을 수 있어야 함

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;
    
    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;
}
