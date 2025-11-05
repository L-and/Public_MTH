using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
    public DoorManager door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player In");
            door.OpenDoors();
        }
    }
}
