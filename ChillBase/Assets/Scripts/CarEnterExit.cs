using UnityEngine;
using UnityEngine.InputSystem;

public class CarEnterExit : MonoBehaviour
{
    [Header("Персонаж")]
    [Tooltip("Объект самого персонажа (с контроллером движения)")]
    public GameObject character;
    [Tooltip("Объект камеры персонажа")]
    public GameObject characterCamera;

    [Header("Транспорт")]
    [Tooltip("Компонент контроллера машины (например, CarController)")]
    public MonoBehaviour carController;
    [Tooltip("Объект камеры машины")]
    public GameObject carCamera;

    [Header("UI Интерфейс")]
    [Tooltip("Переменная для вашей таблички / подсказки на экране (GameObject)")]
    public GameObject promptTable;

    [Header("Настройки")]
    [Tooltip("Точка, куда персонаж телепортируется при выходе из машины")]
    public Transform exitPoint;

    private bool isInsideCar = false;
    private bool isPlayerZone = false; // Находится ли игрок в зоне этого триггера

    void Start()
    {
        Debug.Log($"[{gameObject.name}] Скрипт запущен на объекте с триггером.");

        // Гарантируем правильные стартовые состояния
        SetCarActive(false);

        if (promptTable != null)
        {
            promptTable.SetActive(false); // Выключаем табличку на старте
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Внимание: Переменная promptTable (табличка) не назначена!");
        }
    }

    void Update()
    {
        // Проверяем нажатие клавиши E (через New Input System Unity 6)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isInsideCar)
            {
                Debug.Log($"[{gameObject.name}] Нажата E. Выходим из машины.");
                ExitCar();
            }
            else if (isPlayerZone)
            {
                Debug.Log($"[{gameObject.name}] Нажата E внутри триггера. Садимся в машину.");
                EnterCar();
            }
        }
    }

    // Срабатывает, когда кто-то входит в триггер на ЭТОМ объекте
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел именно наш персонаж (или объект с тегом Player)
        if (other.gameObject == character || other.CompareTag("Player"))
        {
            isPlayerZone = true;
            Debug.Log($"[{gameObject.name}] Игрок вошел в триггер. Включаем табличку.");

            if (promptTable != null)
                promptTable.SetActive(true);
        }
    }

    // Срабатывает, когда кто-то выходит из триггера на ЭТОМ объекте
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == character || other.CompareTag("Player"))
        {
            isPlayerZone = false;
            Debug.Log($"[{gameObject.name}] Игрок вышел из триггера. Выключаем табличку.");

            if (promptTable != null)
                promptTable.SetActive(false);
        }
    }

    void EnterCar()
    {
        isInsideCar = true;
        isPlayerZone = false; // Сбрасываем, так как игрок теперь внутри машины

        // Прячем табличку, чтобы она не мешала ехать
        if (promptTable != null) promptTable.SetActive(false);

        // Выключаем человека и его камеру
        character.SetActive(false);
        characterCamera.SetActive(false);

        // Включаем машину и её камеру
        SetCarActive(true);
        Debug.Log($"[{gameObject.name}] Вход завершен. Машина активна.");
    }

    void ExitCar()
    {
        isInsideCar = false;

        // Отключаем управление машиной
        SetCarActive(false);

        // Перемещаем персонажа в точку выхода
        if (exitPoint != null)
        {
            character.transform.position = exitPoint.position;
        }
        else
        {
            character.transform.position = transform.position + transform.right * 2f;
        }

        // Включаем человека обратно
        character.SetActive(true);
        characterCamera.SetActive(true);
        Debug.Log($"[{gameObject.name}] Выход завершен. Игрок активен.");

        // При выходе физика Unity сама мгновенно пересчитает триггер, 
        // и если игрок окажется внутри зоны, OnTriggerEnter сработает снова и включит табличку.
    }

    void SetCarActive(bool active)
    {
        if (carController != null) carController.enabled = active;
        if (carCamera != null) carCamera.SetActive(active);
    }
}