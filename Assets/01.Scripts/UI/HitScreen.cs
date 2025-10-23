using UnityEngine;

public class HitScreen : MonoBehaviour
{
    ScreenDamage script;
    void Start()
    {
        script = GetComponent<ScreenDamage>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            Hit();
        }
    }
    public void Hit()
    {
        script.blurDuration = 0.1f;
        script.blurFadeSpeed = 6f;
        script.ShowDamage(70);
    }

}
