using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public DoorClose door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player In");
            door.CloseDoors();
        }
    }
}
