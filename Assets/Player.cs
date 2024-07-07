using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
  private static readonly string AXIS_HORIZONTAL = "Horizontal";
  private static readonly string AXIS_VERTICAL = "Vertical";
  private static readonly string MOUSE_X = "Mouse X";
  private static readonly string MOUSE_Y = "Mouse Y";
  private Rigidbody rigidbody;
  public GameObject cameraObject;
  public float speed = 375F;
  private Vector3 currentMovement;

  void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
  }

  void Update()
  {
    currentMovement = new Vector3(Input.GetAxis(AXIS_HORIZONTAL), 0, Input.GetAxis(AXIS_VERTICAL)).normalized;

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

  void FixedUpdate() {
    rigidbody.velocity = currentMovement * speed * Time.fixedDeltaTime;
  }
}
