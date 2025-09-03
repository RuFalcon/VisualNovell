// Файл: DialogueNode.cs (Обновленная версия)
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("Настройка")]
    public CharacterProfile character;
    public Sprite backgroundSprite; // Новое поле для фона

    [Header("Содержимое Узла")]
    [TextArea(3, 10)]
    public string dialogueText;

    [Header("Переходы")]
    public Choice[] choices;
    public DialogueNode nextNode;

    [Header("Настройки Концовки")] // <-- НОВЫЙ РАЗДЕЛ
    public bool isEnding = false; // Эта галочка будет означать, что узел - концовка
    [TextArea(3, 10)]
    public string endingText; // Текст, который появится на экране Game Over

    [Header("Настройки Звука")] // <-- НОВЫЙ РАЗДЕЛ
    public AudioClip backgroundMusicClip; // Музыка, которая начнет играть на этом узле
    public bool stopCurrentMusic = false; // Поставить галочку, чтобы остановить музыку
    public AudioClip soundEffectClip;     // SFX, который проиграется один раз при появлении узла

    [Header("Игровые Состояния")] // <-- НОВЫЙ РАЗДЕЛ
    // Имя флага, который будет установлен в 'true' при показе этого узла выдаётся предмет
    public string conditionToSet;

    [Header("Достижения")]
    // Уникальный ID (slug) достижения, которое откроется на этом узле
    public string achievementId;
}

// Класс Choice остается без изменений
[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogueNode nextNode;
    [Header("Условие для выбора")] // <-- НОВОЕ ПОЛЕ ВНУТРИ ВЫБОРА
    // Имя флага, который должен быть 'true', чтобы этот выбор был активен
    public string requiredCondition;
}