using UnityEngine;

public class BreakOnImpact : MonoBehaviour
{
    public GameObject brokenVersion;
    public float breakForce = 5f;
    public float explosionForce = 200f;
    public float explosionRadius = 2f;

    private bool isBroken = false;   // ✅ защита

    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;

        if (collision.relativeVelocity.magnitude > breakForce)
        {
            isBroken = true;
            Break();
        }
    }

    void Break()
    {
        GameObject broken = Instantiate(brokenVersion, transform.position, transform.rotation);

        foreach (Rigidbody rb in broken.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Notify dialogue about bottle break
        DialogueVR dialogue = FindFirstObjectByType<DialogueVR>();
        if (dialogue != null)
        {
            dialogue.OnBottleBreak();
        }

        // Удаляем текущий кусок
        Transform parent = transform.parent;
        Destroy(gameObject);

        // Если родитель (префаб бутылки) опустел — удаляем его, чтобы не копились пустышки
        if (parent != null && parent.childCount == 0)
        {
            Destroy(parent.gameObject);
        }
    }
}
