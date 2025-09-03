// Файл: DialogueManager.cs (Полностью обновленная версия)
using System.Collections; // Необходимо для корутин
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GamePush;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Image primaryBackground;
    [SerializeField] private Image secondaryBackground;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Настройки печати текста")]
    [SerializeField] private float typingSpeed = 0.04f; // Скорость появления букв

    [Header("Стартовый Узел")]
    [SerializeField] private DialogueNode currentNode;

    [Header("UI Элементы Концовки")] // <-- НОВЫЙ РАЗДЕЛ
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI endingTextComponent;
    [SerializeField] private Button restartButton;

    [Header("Настройки Эффектов")] // <-- НОВЫЙ РАЗДЕЛ
    [SerializeField] private float backgroundFadeDuration = 1.0f; // Длительность фейда в секундах
    private bool isPrimaryBackgroundActive = true; // Флаг, показывающий, какой фон сейчас главный
    private Sprite currentBackgroundSprite; // Запоминаем текущий фон
    private Coroutine backgroundFadeCoroutine; // Ссылка на корутину фейда

    private bool isTyping = false; // Флаг, показывающий, идет ли печать текста
    private Coroutine typingCoroutine; // Ссылка на запущенную корутину печати

    private void Start()
    {
        secondaryBackground.color = new Color(1, 1, 1, 0);
        dialoguePanel.SetActive(false);
        choicesContainer.SetActive(false);
        gameOverPanel.SetActive(false);
        if (currentNode != null)
        {
            // Устанавливаем самый первый фон без фейда
            if (currentNode.backgroundSprite != null)
            {
                primaryBackground.sprite = currentNode.backgroundSprite;
                currentBackgroundSprite = currentNode.backgroundSprite;
            }
            ShowNode(currentNode);
        }
    }

    private void Update()
    {
        // Обработка клика мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Если текст еще печатается - пропустить эффект и показать весь текст сразу
            if (isTyping)
            {
                CompleteText();
            }
            // Если печать завершена и нет активных выборов - перейти к следующему узлу
            else if (dialoguePanel.activeSelf && !choicesContainer.activeSelf)
            {
                if (currentNode.nextNode != null)
                {
                    ShowNode(currentNode.nextNode);
                    GP_Ads.ShowFullscreen();
                }
                else
                {
                    // Проверяем, является ли этот узел концовкой
                    if (currentNode.isEnding)
                    {
                        TriggerEnding(currentNode.endingText);
                    }
                    else // Если это просто конец ветки, но не концовка игры
                    {
                        EndDialogue();
                    }
                }
            }
        }
    }

    private void ShowNode(DialogueNode node)
    {
        // Прерываем предыдущую корутину, если она была
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentNode = node;
        dialoguePanel.SetActive(true);
        ClearChoices();

        // 1. Смена фона, если он указан в узле
        if (node.backgroundSprite != null && currentBackgroundSprite != node.backgroundSprite)
        {
            // Если предыдущий фейд еще идет, останавливаем его
            if (backgroundFadeCoroutine != null)
            {
                StopCoroutine(backgroundFadeCoroutine);
            }
            backgroundFadeCoroutine = StartCoroutine(FadeInBackground(node.backgroundSprite));
            currentBackgroundSprite = node.backgroundSprite;
        }

        // --- УПРАВЛЕНИЕ ЗВУКОМ ---
        if (SoundManager.instance != null)
        {
            // 1. Проигрываем SFX, если он есть
            if (node.soundEffectClip != null)
            {
                SoundManager.instance.PlaySFX(node.soundEffectClip);
            }

            // 2. Управляем фоновой музыкой
            // Сначала проверяем, нужно ли остановить текущую музыку
            if (node.stopCurrentMusic)
            {
                SoundManager.instance.StopMusic();
            }
            // Затем проверяем, нужно ли включить новую (else if, чтобы не остановить и тут же не включить)
            else if (node.backgroundMusicClip != null)
            {
                SoundManager.instance.PlayMusic(node.backgroundMusicClip);
            }
        }

        // --- УСТАНОВКА ИГРОВОГО СОСТОЯНИЯ ---
        if (!string.IsNullOrEmpty(node.conditionToSet))
        {
            GameStateManager.instance.SetCondition(node.conditionToSet, true);
        }

        // 2. Установка имени и цвета
        if (node.character != null)
        {
            nameText.text = node.character.characterName;
            nameText.color = node.character.nameColor;
        }
        else // Для текста "от автора"
        {
            nameText.text = "";
        }

        // 3. Запуск печати текста по буквам
        typingCoroutine = StartCoroutine(TypeDialogue(node.dialogueText));

        // Показ выборов, если они есть
        if (node.choices.Length > 0)
        {
            // Выборы появятся после завершения печати текста
            StartCoroutine(ShowChoicesAfterTyping(node.choices));
        }
    }

    private IEnumerator TypeDialogue(string sentence)
    {
        SoundManager.instance.StartTypingSound();
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        SoundManager.instance.StopTypingSound();
    }

    private void CompleteText()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = currentNode.dialogueText;
        isTyping = false;
        SoundManager.instance.StopTypingSound();
    }

    // Корутина, чтобы выборы появлялись только после того, как текст напечатался
    private IEnumerator ShowChoicesAfterTyping(Choice[] choices)
    {
        // Ждем, пока флаг isTyping не станет false
        yield return new WaitUntil(() => !isTyping);

        choicesContainer.SetActive(true);
        foreach (var choice in choices)
        {
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            Button buttonComponent = buttonGO.GetComponent<Button>();
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                ShowNode(choice.nextNode);
            });

            if (!string.IsNullOrEmpty(choice.requiredCondition))
            {
                // Если есть, проверяем его через GameStateManager
                bool conditionMet = GameStateManager.instance.CheckCondition(choice.requiredCondition);
                // Делаем кнопку активной или неактивной в зависимости от результата
                buttonComponent.interactable = conditionMet;
            }
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        choicesContainer.SetActive(false);
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicesContainer.SetActive(false);
        Debug.Log("Диалог завершен.");
    }

    private void TriggerEnding(string final_text)
    {
        if (!string.IsNullOrEmpty(currentNode.achievementId))
        {
            GP_Achievements.Unlock(currentNode.achievementId);
        }
        // Скрываем интерфейс диалога
        dialoguePanel.SetActive(false);
        choicesContainer.SetActive(false);

        // Показываем панель концовки
        gameOverPanel.SetActive(true);

        // Назначаем действие на кнопку
        restartButton.onClick.RemoveAllListeners(); // Очищаем старые слушатели
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0); // Загружаем сцену по имени
        });

        // Запускаем печать финального текста
        StartCoroutine(TypeEndingText(final_text));
    }

    private IEnumerator TypeEndingText(string text)
    {
        SoundManager.instance.StartTypingSound();
        endingTextComponent.text = "";
        foreach (char letter in text.ToCharArray())
        {
            endingTextComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Используем ту же скорость печати
        }
        SoundManager.instance.StopTypingSound();
    }

    private IEnumerator FadeInBackground(Sprite newSprite)
    {
        // Определяем, какой фон сейчас активен, а какой будет появляться
        Image activeImage = isPrimaryBackgroundActive ? primaryBackground : secondaryBackground;
        Image inactiveImage = isPrimaryBackgroundActive ? secondaryBackground : primaryBackground;

        // Устанавливаем новый спрайт на неактивный (прозрачный) фон
        inactiveImage.sprite = newSprite;

        float elapsedTime = 0f;
        while (elapsedTime < backgroundFadeDuration)
        {
            // Увеличиваем прозрачность нового фона и уменьшаем у старого
            float alpha = elapsedTime / backgroundFadeDuration;
            inactiveImage.color = new Color(1, 1, 1, alpha);
            activeImage.color = new Color(1, 1, 1, 1 - alpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // Устанавливаем финальные значения, чтобы избежать неточностей
        inactiveImage.color = Color.white;
        activeImage.color = new Color(1, 1, 1, 0);

        // Меняем флаг активности
        isPrimaryBackgroundActive = !isPrimaryBackgroundActive;
        backgroundFadeCoroutine = null;
    }
}