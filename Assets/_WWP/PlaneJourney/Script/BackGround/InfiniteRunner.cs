using System.Collections;
using UnityEngine;

public class InfiniteRunner : MonoBehaviour {
    public float initialMoveSpeed = 5f; // Исходная скорость движения
    public float maxY = 10f; // Максимальная позиция по Y
    public float minY = -10f; // Минимальная позиция по Y

    private bool movingUp = true; // Флаг направления движения

    private Vector3 originalPosition; // Исходная позиция

    private float moveSpeed; // Текущая скорость движения
    private float speedIncreaseInterval = 30f; // Интервал увеличения скорости
    private float lastSpeedIncreaseTime; // Время последнего увеличения скорости

    private void Start ( ) {
        originalPosition = transform.position;
        moveSpeed = initialMoveSpeed;
        lastSpeedIncreaseTime = Time.time;
        //StartCoroutine (MoveLoop ());
    }

    private IEnumerator MoveLoop ( ) {
        while ( true ) {
            float targetY = movingUp ? maxY : minY;

            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, targetY, transform.position.z), step);

            if ( transform.position.y == targetY ) {
                movingUp = !movingUp;

                // Возвращаемся на исходную позицию
                if ( !movingUp ) {
                    transform.position = originalPosition;
                }
            }

            // Проверяем, прошло ли достаточно времени для увеличения скорости
            if ( Time.time - lastSpeedIncreaseTime >= speedIncreaseInterval ) {
                IncreaseSpeed ();
            }

            yield return null;
        }
    }

    private void IncreaseSpeed ( ) {
        moveSpeed += 1f; // Увеличиваем скорость на 1 единицу
        lastSpeedIncreaseTime = Time.time; // Обновляем время последнего увеличения скорости
    }
}
