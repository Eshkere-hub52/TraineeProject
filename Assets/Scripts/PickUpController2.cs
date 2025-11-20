using UnityEngine;

public class CartDragController : MonoBehaviour
{
    [Header("Dragging Settings")]
    [Tooltip("Сила притягування. Регулює, наскільки швидко об'єкт доганяє курсор.")]
    [SerializeField] private float dragForce = 300f; // Збільшено для кращого контролю

    [Tooltip("Опір при перетягуванні. Гасить коливання.")]
    [SerializeField] private float dragDamping = 10f;

    private Rigidbody draggedRB;
    private float distanceToCart;

    // Точка в локальних координатах об'єкта, за яку тягнемо
    private Vector3 localHitPoint;

    // Обмеження: блокуємо нахил (X, Z), дозволяємо поворот (Y)
    private const RigidbodyConstraints FreezeTilt = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (draggedRB != null)
        {
            ApplyDragForce();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    draggedRB = rb;
                    distanceToCart = Vector3.Distance(transform.position, draggedRB.position);

                    // ⭐ ЗБЕРІГАННЯ ТОЧКИ ЗАХОПЛЕННЯ
                    // Перетворюємо світову точку попадання на локальні координати візка
                    localHitPoint = draggedRB.transform.InverseTransformPoint(hit.point);

                    PrepareForDrag();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && draggedRB != null)
        {
            ReleaseCart();
        }
    }

    void PrepareForDrag()
    {
        draggedRB.useGravity = false;
        draggedRB.drag = dragDamping;
        draggedRB.angularDrag = dragDamping;

        // Встановлюємо обмеження: блокуємо нахил, дозволяємо поворот
        draggedRB.constraints = FreezeTilt;

        // Обнуляємо інерцію
        draggedRB.angularVelocity = Vector3.zero;
    }

    void ReleaseCart()
    {
        if (draggedRB != null)
        {
            draggedRB.useGravity = true;
            draggedRB.drag = 0.5f;
            draggedRB.angularDrag = 0.05f;

            // Зберігаємо обмеження нахилу для стабільності HoverForce
            draggedRB.constraints = FreezeTilt;

            draggedRB = null;
        }
    }

    void ApplyDragForce()
    {
        // 1. Обчислюємо цільову позицію курсора у 3D світі
        Vector3 targetWorldPosition = GetMouseWorldPosition();

        // 2. Обчислюємо поточну світову позицію точки захоплення на візку
        Vector3 currentHitPoint = draggedRB.transform.TransformPoint(localHitPoint);

        // 3. Вектор сили: від поточної точки захоплення до цільової точки курсора
        Vector3 forceDirection = targetWorldPosition - currentHitPoint;

        // 4. Обчислюємо силу
        Vector3 forceToApply = forceDirection * dragForce;

        // ⭐ ЗАСТОСУВАННЯ СИЛИ ДО ТОЧКИ: Створює крутний момент (обертання)
        draggedRB.AddForceAtPosition(forceToApply, currentHitPoint, ForceMode.Force);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.GetPoint(distanceToCart);
    }
}