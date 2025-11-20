using UnityEngine;

public class PickaxeController : MonoBehaviour
{
    [Header("Pickaxe Settings")]
    [Tooltip("Максимальна відстань удару.")]
    public float hitDistance = 2.5f;

    void Update()
    {
        // Приклад: Удар киркою при натисканні лівої кнопки миші
        if (Input.GetMouseButtonDown(0))
        {
            PerformHitCheck();
        }
    }

    void PerformHitCheck()
    {
        RaycastHit hit;

        // Стріляємо променем від позиції камери/кирки вперед
        // (Припускаємо, що цей скрипт знаходиться на об'єкті, який дивиться вперед)
        if (Physics.Raycast(transform.position, transform.forward, out hit, hitDistance))
        {
            // Перевіряємо, чи об'єкт, у який ми влучили, має наш скрипт DestructibleBlock
            DestructibleBlock block = hit.transform.GetComponent<DestructibleBlock>();

            if (block != null)
            {
                // ⭐ ВИКЛИК: Викликаємо функцію удару на знайденому блоці
                block.HitByPickaxe();
            }
        }
    }
}