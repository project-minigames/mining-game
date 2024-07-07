using UnityEngine;

public class HeadLamp : MonoBehaviour
{
  private Light lamp;
  public Light secondaryLamp;
  
  public AudioSource audioSource;
  public AudioClip clickOnSound;
  public AudioClip clickOffSound;

  void Start()
  {
    lamp = GetComponent<Light>();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F))
    {
      lamp.enabled = !lamp.enabled;
      secondaryLamp.enabled = lamp.enabled;
      audioSource.PlayOneShot(lamp.enabled ? clickOnSound : clickOffSound);
    }
  }
}
