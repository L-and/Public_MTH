using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float time = 0f;
    void Start()
    {
        Destroy(this.gameObject, time);
    }

}
