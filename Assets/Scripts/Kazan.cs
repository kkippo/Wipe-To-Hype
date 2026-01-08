using UnityEngine;
using System.Collections.Generic;

public class KazanTrigger : MonoBehaviour
{
    // Tags of objects that should be destroyed when entering the trigger
    public string[] destroyableTags = { "Coin", "Cube", "Break" };
    
    // If true, will only destroy objects with tags in the list above
    // If false, will destroy any object (legacy behavior)
    public bool useTagFilter = true;

    private DialogueVR dialogue;
    private Collider _collider;
    // Track objects already processed to prevent multiple triggers from same object
    private HashSet<int> processedObjectIds = new HashSet<int>();

    void Start()
    {
        dialogue = FindFirstObjectByType<DialogueVR>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;

        GameObject targetObject = other.gameObject;
        if (targetObject == null)
            return;

        int objectId = targetObject.GetInstanceID();

        // Skip if this object was already processed by another collider in this Kazan
        if (processedObjectIds.Contains(objectId))
            return;

        // Check tag filter
        if (useTagFilter)
        {
            bool hasDestroyableTag = false;
            foreach (string tag in destroyableTags)
            {
                if (targetObject.CompareTag(tag))
                {
                    hasDestroyableTag = true;
                    break;
                }
            }

            if (!hasDestroyableTag)
            {
                return;
            }
        }

        // Mark as processed
        processedObjectIds.Add(objectId);

        // Notify dialogue before destroying
        if (dialogue != null)
        {
            if (targetObject.CompareTag("Coin"))
            {
                dialogue.OnCoinCollected();
            }
            else if (targetObject.CompareTag("Cube"))
            {
                dialogue.OnCubeCollected();
            }
            else if (targetObject.CompareTag("Break"))
            {
                dialogue.OnBottleCleaned();
            }
        }

        Destroy(targetObject);
    }

    // Clean up processed objects occasionally (when they're destroyed)
    void Update()
    {
        // Optional: clean up IDs of destroyed objects to save memory (if needed)
        // This is rare but prevents memory leak in very long gameplay
    }
}