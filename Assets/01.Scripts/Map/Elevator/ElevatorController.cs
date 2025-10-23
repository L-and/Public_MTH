using UnityEngine;

public class ElevatorController : MonoBehaviour
{
  [SerializeField] private GameObject startPointObject;
  [SerializeField] private GameObject reloadButtonObject;

  public void SetupForStart()
  {
    if (startPointObject != null) startPointObject.SetActive(true);

    if (reloadButtonObject != null) reloadButtonObject.SetActive(false);
  }
  
  public void SetupForEnd()
  {
    if (startPointObject != null) startPointObject.SetActive(false);

    if (reloadButtonObject != null) reloadButtonObject.SetActive(true);    
  }
}
