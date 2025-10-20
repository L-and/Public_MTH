using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractable
{
  public void Interact()
  {
    Debug.Log("버튼 누름");
    //TODO: 다음 씬으로 넘어감
  }
}
