using UnityEngine;

public class ResizeAndCenterObject : MonoBehaviour {
    private Vector3 initialScale;

    private void Start ( ) {
        // ���������� �������� ������� �������
        initialScale = transform.localScale;

        // �������� ����� ��� ��������� �������� � ���������������� �������
        ResizeAndRepositionObject ();
    }

    private void ResizeAndRepositionObject ( ) {
        // �������� ������� ������ � ������� �����������
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // ������������ ������ �� ������ ��������� �������� � �������� ������
        Vector3 newScale = new Vector3 (initialScale.x * screenWidth, initialScale.y * screenHeight, initialScale.z);

        // ��������� ����� �������
        transform.localScale = newScale;

        // ������������� ������ �� ������ ������
        transform.position = new Vector3 (0f, 0f, transform.position.z);
    }
}
