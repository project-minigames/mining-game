using UnityEngine;

public class Player : MonoBehaviour
{
  private static readonly string AXIS_HORIZONTAL = "Horizontal";
  private static readonly string AXIS_VERTICAL = "Vertical";
  private static readonly string BUTTON_JUMP = "Jump";
  private static readonly string MOUSE_X = "Mouse X";
  private static readonly string MOUSE_Y = "Mouse Y";

  private Rigidbody rb;
  public GameObject cameraObject;
  public float movementSpeed = 375F;
  public float jumpForce = 20F;
  private Vector3 currentMovement;
  private bool jumping;
  private bool onGround;

  void Start()
  {
    rb = GetComponent<Rigidbody>();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
      return;
    }

    currentMovement = new Vector3(Input.GetAxis(AXIS_HORIZONTAL), 0, Input.GetAxis(AXIS_VERTICAL)).normalized;
    currentMovement = transform.TransformDirection(currentMovement);
    jumping = Input.GetButton(BUTTON_JUMP) && onGround;

    var mouseX = Input.GetAxis(MOUSE_X);
    var mouseY = Input.GetAxis(MOUSE_Y);

    var playerRotation = transform.localEulerAngles;
    playerRotation.y += mouseX;
    transform.localRotation = Quaternion.AngleAxis(playerRotation.y, Vector3.up);

    var cameraRotation = cameraObject.transform.localEulerAngles;
    cameraRotation.x -= mouseY;
    cameraRotation.x = Mathf.Clamp(cameraRotation.x, 0, 26);
    cameraObject.transform.localRotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.right);
  }

  void FixedUpdate()
  {
    var movement = new Vector3
    {
      x = currentMovement.x * movementSpeed,
      y = jumping ? currentMovement.y * jumpForce : currentMovement.y,
      z = currentMovement.z * movementSpeed
    };

    rb.velocity = movement * Time.fixedDeltaTime;

    if (jumping)
    {
      transform.position = transform.position + new Vector3(0, jumpForce * Time.fixedDeltaTime, 0);
      //       rb.velocity += new Vector3(0, jumpForce * Time.deltaTime, 0);
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    onGround = true;
  }

  void OnCollisionExit(Collision collision)
  {
    onGround = false;
  }
}
