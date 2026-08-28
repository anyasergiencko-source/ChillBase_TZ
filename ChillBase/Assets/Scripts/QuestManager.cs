using UnityEngine;

[System.Serializable]
public class QuestStep
{
    [Header("Название квеста (для удобства)")]
    public string questName;

    [Header("Объект триггера (куда надо приехать)")]
    public GameObject triggerObject;

    [Header("UI Canvas этого квеста (табличка с текстом)")]
    public GameObject questCanvasInfo;

    [Header("Стрелка навигации для этой точки")]
    public GameObject navigationArrow;
}

public class QuestManager : MonoBehaviour
{
    [Header("Список всех квестов (по порядку)")]
    public QuestStep[] quests;

    [Header("Теги объектов, которые могут выполнять квесты")]
    [Tooltip("Например: Player, Car, Truck. Если у вошедшего объекта есть этот тег, квест зачтется.")]
    public string[] allowedTags;

    // Индекс квеста, который активен прямо сейчас
    private int currentQuestIndex = 0;

    void Start()
    {
        // Проверяем, заполнили ли мы массив в инспекторе
        if (quests.Length > 0)
        {
            InitializeQuests();
            ActivateQuest(currentQuestIndex);
        }
        else
        {
            Debug.LogError("Массив квестов пуст! Добавьте квесты в инспекторе QuestManager.");
        }
    }

    // Выключаем вообще все квесты, чтобы на сцене не было каши
    private void InitializeQuests()
    {
        foreach (var quest in quests)
        {
            if (quest.triggerObject != null) quest.triggerObject.SetActive(false);
            if (quest.questCanvasInfo != null) quest.questCanvasInfo.SetActive(false);
            if (quest.navigationArrow != null) quest.navigationArrow.SetActive(false);
        }
    }

    // Включаем конкретный квест по его индексу
    private void ActivateQuest(int index)
    {
        if (index >= quests.Length)
        {
            // Если индекс вышел за пределы массива — значит, все квесты пройдены!
            FinishAllQuests();
            return;
        }

        QuestStep activeQuest = quests[index];

        // Активируем триггер, UI и стрелку текущего квеста
        if (activeQuest.triggerObject != null) activeQuest.triggerObject.SetActive(true);
        if (activeQuest.questCanvasInfo != null) activeQuest.questCanvasInfo.SetActive(true);
        if (activeQuest.navigationArrow != null) activeQuest.navigationArrow.SetActive(true);

        Debug.Log($"Активирован квест: {activeQuest.questName}");
    }

    // Метод, который вызывается при успешном прохождении триггера
    public void OnQuestTriggerEntered(GameObject triggeredZone, GameObject objectWhoEntered)
    {
        // 1. Проверяем, совпадает ли зона, в которую заехали, с зоной ТЕКУЩЕГО квеста
        if (quests[currentQuestIndex].triggerObject != triggeredZone)
        {
            return; // Если это триггер от другого (будущего) квеста, ничего не делаем
        }

        // 2. Проверяем, подходит ли тег объекта, который заехал в триггер
        if (!HasAllowedTag(objectWhoEntered))
        {
            Debug.Log($"Объект {objectWhoEntered.name} имеет тег {objectWhoEntered.tag}, которому нельзя сдавать квест.");
            return;
        }

        // 3. Если всё верно — сдаем текущий квест и выключаем его элементы
        QuestStep completedQuest = quests[currentQuestIndex];
        if (completedQuest.triggerObject != null) completedQuest.triggerObject.SetActive(false);
        if (completedQuest.questCanvasInfo != null) completedQuest.questCanvasInfo.SetActive(false);
        if (completedQuest.navigationArrow != null) completedQuest.navigationArrow.SetActive(false);

        Debug.Log($"Квест '{completedQuest.questName}' успешно выполнен!");

        // 4. Переходим к следующему квесту
        currentQuestIndex++;
        ActivateQuest(currentQuestIndex);
    }

    // Вспомогательная функция проверки тегов
    private bool HasAllowedTag(GameObject obj)
    {
        foreach (string allowedTag in allowedTags)
        {
            if (obj.CompareTag(allowedTag))
            {
                return true; // Тег совпал
            }
        }
        return false; // Тег не найден в списке разрешенных
    }

    // Логика завершения всей цепочки
    private void FinishAllQuests()
    {
        Debug.Log("Поздравляем! Все квесты в цепочке успешно выполнены!");
        // Здесь вы можете включить финальный экран победы или вызвать другое событие
    }
}