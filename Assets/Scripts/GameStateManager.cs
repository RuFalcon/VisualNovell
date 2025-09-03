// Файл: GameStateManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    // Используем словарь для хранения всех условий/флагов в игре
    // Ключ (string) - это имя флага, например, "hasKey"
    // Значение (bool) - это его состояние, true (выполнено) или false (не выполнено)
    private Dictionary<string, bool> conditions = new Dictionary<string, bool>();

    void Awake()
    {
        // Классическая реализация синглтона
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Устанавливает состояние для определенного условия.
    /// </summary>
    /// <param name="conditionName">Имя условия (например, "hasKey")</param>
    /// <param name="value">Значение (true/false)</param>
    public void SetCondition(string conditionName, bool value)
    {
        // Если условие уже есть в словаре, обновляем его. Если нет - добавляем.
        conditions[conditionName] = value;
        Debug.Log($"Состояние '{conditionName}' установлено в '{value}'");
    }

    /// <summary>
    /// Проверяет, выполнено ли определенное условие.
    /// </summary>
    /// <param name="conditionName">Имя условия для проверки</param>
    /// <returns>True, если условие выполнено, иначе False.</returns>
    public bool CheckCondition(string conditionName)
    {
        // Проверяем, существует ли такое условие в словаре
        if (conditions.TryGetValue(conditionName, out bool value))
        {
            // Если существует, возвращаем его значение
            return value;
        }

        // Если условия нет в словаре, считаем его невыполненным
        return false;
    }
}