using UnityEngine;

public class Movement : MonoBehaviour
{
  public float speed = 20F;
  public float lookSpeed = 2F;
  private Vector2 velocity;
  private float yaw = 0F;
  private float pitch = 0F;

  void Balls()
  {
    /* camera movement 
    yaw += lookSpeed * Input.GetAxis("Mouse X");
    pitch -= lookSpeed * Input.GetAxis("Mouse Y");

    transform.eulerAngles = new Vector3(pitch, yaw, 0F);
    */
  }
}
