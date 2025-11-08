using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BossDoorTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    public GameObject outside;
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            outside.SetActive(false);
            door.SetActive(true);
        }
    }
}
