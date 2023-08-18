using UnityEngine;

public class ResizeAndCenterObject : MonoBehaviour {
    private Vector3 initialScale;

    private void Start ( ) {
        // Запоминаем исходный масштаб объекта
        initialScale = transform.localScale;

        // Вызываем метод для изменения размеров и позиционирования объекта
        ResizeAndRepositionObject ();
    }

    private void ResizeAndRepositionObject ( ) {
        // Получаем размеры экрана в мировых координатах
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // Масштабируем объект на основе исходного масштаба и размеров экрана
        Vector3 newScale = new Vector3 (initialScale.x * screenWidth, initialScale.y * screenHeight, initialScale.z);

        // Применяем новый масштаб
        transform.localScale = newScale;

        // Позиционируем объект по центру экрана
        transform.position = new Vector3 (0f, 0f, transform.position.z);
    }
}
