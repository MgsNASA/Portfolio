using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // �������� ��������
    public float maxX = 10f; // ������������ ������� �� X ������
    public float minX = -10f; // ������������ ������� �� X �����
    [SerializeField]
    private bool canMove = false;

    private Vector3 _targetPos; // ������� ������� ��� �����������

    private void Update ( ) {
        // ���������, ������ �� ������ ���� (��� ������ �� ��������� �����������)
        if ( Input.GetMouseButton (0) && canMove ) {
            // �������� ������� ������� ���� (��� ������ �� ��������� �����������)
            Vector3 mousePosition = Input.mousePosition;

            // ���������� ������� ������� � ������� �����������
            _targetPos = Camera.main.ScreenToWorldPoint (mousePosition);
            _targetPos.y = transform.position.y; // ������� ��������� �� ���������
            _targetPos.z = transform.position.z; // ������� ��������� �� ��� Z
            _targetPos.x = Mathf.Clamp (_targetPos.x, minX, maxX); // ������������ �� X

            // ������������ � ������� �������
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
