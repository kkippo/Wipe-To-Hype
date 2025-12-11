using UnityEngine;

public class RubFade : MonoBehaviour
{
    public float fadeRate = 0.3f;

    private Renderer rend;
    private Color color;

    void Start()
    {
        rend = GetComponent<Renderer>();
        color = rend.material.color;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Plane"))
        {
            color.a -= fadeRate * Time.deltaTime;
            rend.material.color = color;

            if (color.a <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}