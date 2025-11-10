using DG.Tweening;
using UnityEngine;

public class BossDie : MonoBehaviour
{
    public ParticleSystem explosion;
    public GameObject heart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.DOMoveY(0, 3).SetEase(Ease.Linear).OnComplete(() => 
        {
            Destroy(heart);
            var particle = Instantiate(explosion);
            particle.transform.position = Vector3.zero;
            particle.transform.localScale = new Vector3(4, 4, 4);
        }); 
    }

}
