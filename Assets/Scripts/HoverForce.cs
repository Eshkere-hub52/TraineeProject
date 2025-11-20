using UnityEngine;

public class HoverForce : MonoBehaviour
{
    [Header("Hover Settings")]
    [Tooltip("Сила відштовхування від землі. Збільште для компенсації ваги.")]
    [SerializeField] private float hoverForce = 8000f;
    [Tooltip("Висота, на якій об'єкт повинен зависати.")]
    [SerializeField] private float hoverHeight = 0.7f;
    [Tooltip("Сила гасіння вертикальних коливань.")]
    [SerializeField] private float springDamper = 5f;

    private Rigidbody rb;
    // Обмеження, що блокує нахил, але дозволяє повертати
    private const RigidbodyConstraints FreezeTilt = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("HoverForce requires a Rigidbody component on the same GameObject.");
            enabled = false;
            return;
        }

        // Гарантуємо, що об'єкт не нахиляється
        rb.constraints = FreezeTilt;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        // Стріляємо променем вниз
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverHeight))
        {
            // 1. Пропорційна відстань (0, якщо на hoverHeight; 1, якщо на землі)
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;

            // 2. Сила відштовхування
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;

            // 3. Сила демпфірування (гасіння коливань)
            Vector3 verticalVelocity = Vector3.Project(rb.velocity, Vector3.up);
            Vector3 damperForce = -verticalVelocity * springDamper * rb.mass;

            // 4. Застосування загальної сили
            rb.AddForce(appliedHoverForce + damperForce, ForceMode.Force);
        }
    }
}