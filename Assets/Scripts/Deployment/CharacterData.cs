using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterID;
    public string characterName;
    public Sprite characterUISprite;  // 用于UI上显示的图片
    public Unit characterPrefab; // 部署到地图上时要实例化的预制体
}