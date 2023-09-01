using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // Скорость движения
    public float maxX = 10f; // Максимальная позиция по X справа
    public float minX = -10f; // Максимальная позиция по X слева
    [SerializeField]
    private bool canMove = false;

    private Vector3 _targetPos; // Целевая позиция для перемещения

    private void Update ( ) {
        // Проверяем, зажата ли кнопка мыши (или экрана на мобильных устройствах)
        if ( Input.GetMouseButton (0) && canMove ) {
            // Получаем текущую позицию мыши (или пальца на мобильных устройствах)
            Vector3 mousePosition = Input.mousePosition;

            // Определяем целевую позицию в мировых координатах
            _targetPos = Camera.main.ScreenToWorldPoint (mousePosition);
            _targetPos.y = transform.position.y; // Убираем изменение по вертикали
            _targetPos.z = transform.position.z; // Убираем изменение по оси Z
            _targetPos.x = Mathf.Clamp (_targetPos.x, minX, maxX); // Ограничиваем по X

            // Перемещаемся к целевой позиции
            transform.position = Vector3.MoveTowards (transform.position, _targetPos, moveSpeed * Time.deltaTime);
        }
    }

    public void startPosition ( Transform transform ) {
        gameObject.transform.position = transform.position;
    }

    public void ChangeControlerMovement ( bool canmove ) {
        canMove = canmove;
    }
}
