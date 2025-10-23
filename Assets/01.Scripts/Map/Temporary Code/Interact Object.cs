using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractObject : MonoBehaviour, IInteractable
{
  public void Interact()
  {
    Scene currentScene = SceneManager.GetActiveScene();
    SceneManager.LoadScene(currentScene.buildIndex);
  }
}
