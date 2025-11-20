using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Block Setup")]
    [Tooltip("Об'єкт кристала, який має випасти.")]
    public GameObject crystalPrefab;

    [Tooltip("Кількість ударів, необхідна для руйнування.")]
    [SerializeField]
    private int requiredHits = 5;

    [Header("Current State")]
    [Tooltip("Поточна кількість отриманих ударів.")]
    private int currentHits = 0;

    [Tooltip("Чи вже блок зруйновано.")]
    private bool isDestroyed = false;

    // --- Метод для отримання удару киркою ---

    // ⭐ Цю функцію має викликати ваш скрипт кирки, коли вона потрапляє в блок
    public void HitByPickaxe()
    {
        // Якщо блок уже зруйновано, ігноруємо удари
        if (isDestroyed)
        {
            return;
        }

        // Збільшуємо лічильник ударів
        currentHits++;
        Debug.Log($"Блок отримав удар. Залишилося ударів: {requiredHits - currentHits}");

        // Перевіряємо, чи досягнуто необхідної кількості ударів
        if (currentHits >= requiredHits)
        {
            DestroyBlockAndDropCrystal();
        }
    }

    // --- Логіка руйнування та випадання ---

    private void DestroyBlockAndDropCrystal()
    {
        isDestroyed = true;

        // 1. Випадання кристала
        if (crystalPrefab != null)
        {
            // Створюємо кристал трохи вище позиції блоку
            Vector3 dropPosition = transform.position + Vector3.up * 0.5f;

            // Створення (спавн) кристала
            GameObject crystal = Instantiate(crystalPrefab, dropPosition, Quaternion.identity);

            // Якщо кристал має Rigidbody, додаємо йому невелику силу для відскоку
            Rigidbody rb = crystal.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("Кристал не призначено в Інспекторі! Призначте Crystal Prefab.");
        }

        // 2. Знищення самого блоку
        Destroy(gameObject);
    }
}