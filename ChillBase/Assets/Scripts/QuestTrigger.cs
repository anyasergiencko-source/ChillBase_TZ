using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    // Ссылка на главный менеджер квестов. 
    // Скрипт попытается найти его автоматически, если вы забыли перетащить его в инспекторе.
    [SerializeField] private QuestManager questManager;

    void Awake()
    {
        // Если ссылка в инспекторе не указана, ищем QuestManager на сцене
        if (questManager == null)
        {
            questManager = Object.FindFirstObjectByType<QuestManager>();

            if (questManager == null)
            {
                Debug.LogError($"На сцене не найден QuestManager! Скрипт триггера на объекте {gameObject.name} не сможет работать.");
            }
        }
    }

    // Этот метод вызывается Unity автоматически, когда любой объект с Rigidbody заезжает в наш триггер
    private void OnTriggerEnter(Collider other)
    {
        if (questManager != null)
        {
            // Передаем менеджеру: 
            // 1. gameObject — это сам триггер (эта точка)
            // 2. other.gameObject — это тот, кто в него въехал (например, машина или игрок)
            questManager.OnQuestTriggerEntered(gameObject, other.gameObject);
        }
    }
}