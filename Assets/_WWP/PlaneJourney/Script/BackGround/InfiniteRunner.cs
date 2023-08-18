using System.Collections;
using UnityEngine;

public class InfiniteRunner : MonoBehaviour {
    public float initialMoveSpeed = 5f; // �������� �������� ��������
    public float maxY = 10f; // ������������ ������� �� Y
    public float minY = -10f; // ����������� ������� �� Y

    private bool movingUp = true; // ���� ����������� ��������

    private Vector3 originalPosition; // �������� �������

    private float moveSpeed; // ������� �������� ��������
    private float speedIncreaseInterval = 30f; // �������� ���������� ��������
    private float lastSpeedIncreaseTime; // ����� ���������� ���������� ��������

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

                // ������������ �� �������� �������
                if ( !movingUp ) {
                    transform.position = originalPosition;
                }
            }

            // ���������, ������ �� ���������� ������� ��� ���������� ��������
            if ( Time.time - lastSpeedIncreaseTime >= speedIncreaseInterval ) {
                IncreaseSpeed ();
            }

            yield return null;
        }
    }

    private void IncreaseSpeed ( ) {
        moveSpeed += 1f; // ����������� �������� �� 1 �������
        lastSpeedIncreaseTime = Time.time; // ��������� ����� ���������� ���������� ��������
    }
}
