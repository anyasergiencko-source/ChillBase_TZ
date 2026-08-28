using UnityEngine;

public class ObjectBobbing : MonoBehaviour
{
    [Header("Настройки движения вверх-вниз")]
    [SerializeField] private bool bobs = true;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float bobFrequency = 2f;

    // Храним стартовую позицию объекта
    private Vector3 startPosition;

    void Start()
    {
        // Запоминаем, где стрелка находилась изначально
        startPosition = transform.localPosition;
    }

    void Update()
    {
        if (bobs)
        {
            Vector3 tempPos = startPosition;
            tempPos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            transform.localPosition = tempPos;
        }
    }
}