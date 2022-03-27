using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mouseSesitivity;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private Vector3 moveDirection = Vector3.zero;
    private bool _isGrounded;
    private Rigidbody _rb;
    private Transform _mainCamera;
    //public MouseLook m_MouseLook;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        JumpLogic();
        MovementLogic();
    }

    private void Update()
    {
        _mainCamera = GetComponentInChildren<Camera>().transform;

        MouseControll();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        transform.Translate(movement * speed * Time.fixedDeltaTime);
    }

    private void JumpLogic()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            if (_isGrounded) _rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void MouseControll()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSesitivity, 0);
        _mainCamera.Rotate(-Input.GetAxis("Mouse Y") * mouseSesitivity, 0, 0);

        if (_mainCamera.localRotation.eulerAngles.y != 0)
            _mainCamera.Rotate(Input.GetAxis("Mouse Y") * mouseSesitivity, 0, 0);

        moveDirection = new Vector3(Input.GetAxis("Horizontal") * speed, moveDirection.y, Input.GetAxis("Vertical") * speed);
        moveDirection = transform.TransformDirection(moveDirection);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IsGroundUpate(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGroundUpate(collision, false);
    }

    private void IsGroundUpate(Collision collision, bool value)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            _isGrounded = value;
        }
    }
}
