// Файл: SoundButton.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] // Гарантирует, что на объекте всегда будет компонент Button
public class SoundButton : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void OnEnable()
    {
        // Подписываемся на событие нажатия кнопки
        _button.onClick.AddListener(PlayClickSound);
    }

    void OnDisable()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        _button.onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        // Проверяем, существует ли SoundManager и вызываем его метод
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClickSound();
        }
    }
}