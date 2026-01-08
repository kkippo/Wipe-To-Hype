using UnityEngine;

public class RubFade : MonoBehaviour
{
    public float fadeRate = 0.3f;

    private Renderer rend;
    private Color color;
    private DialogueVR dialogue;
    private bool destroyed = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        color = rend.material.color;
        dialogue = FindFirstObjectByType<DialogueVR>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Plane"))
        {
            color.a -= fadeRate * Time.deltaTime;
            rend.material.color = color;

            if (color.a <= 0f && !destroyed)
            {
                destroyed = true;

                // Notify dialogue about cube collection
                if (dialogue != null && gameObject.CompareTag("Cube"))
                {
                    dialogue.OnCubeCollected();
                }

                Destroy(gameObject);
            }
        }
    }
}