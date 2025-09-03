// Файл: CharacterProfile.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character Profile")]
public class CharacterProfile : ScriptableObject
{
    public string characterName;
    public Color nameColor = Color.white; // Цвет имени по умолчанию - белый
}