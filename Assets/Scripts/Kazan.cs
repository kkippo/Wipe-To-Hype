using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KazanTrigger : MonoBehaviour
{
    // Tags of objects that should be destroyed when entering the trigger
    public string[] destroyableTags = { "Coin", "Cube", "Break" };
    
    // If true, will only destroy objects with tags in the list above
    // If false, will destroy any object (legacy behavior)
    public bool useTagFilter = true;

    // Alembic animation settings
    public GameObject activeAlembic;
    public GameObject breakAlembic;
    public float breakAnimationDuration = 1f;

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

        // Switch alembic animations if they exist
        if (activeAlembic != null && breakAlembic != null)
        {
            StartCoroutine(PlayBreakAnimationAlembic(activeAlembic, breakAlembic, targetObject));
        }
        else
        {
            // No alembics assigned, destroy immediately
            NotifyAndDestroyObject(targetObject);
        }
    }

    private IEnumerator PlayBreakAnimationAlembic(GameObject activeAlembic, GameObject breakAlembic, GameObject targetObject)
    {
        // Disable active alembic, enable break alembic
        activeAlembic.SetActive(false);
        breakAlembic.SetActive(true);

        // Reset AlembicStreamPlayer by disabling and enabling
        breakAlembic.SetActive(false);
        yield return null;
        breakAlembic.SetActive(true);

        // Notify dialogue
        NotifyObjectCollected(targetObject);

        // Wait for break animation to finish
        yield return new WaitForSeconds(breakAnimationDuration);

        // Restore if object still exists
        if (targetObject != null)
        {
            breakAlembic.SetActive(false);
            activeAlembic.SetActive(true);
        }

        // Finally destroy
        if (targetObject != null)
        {
            Destroy(targetObject);
        }
    }

    private void NotifyObjectCollected(GameObject targetObject)
    {
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
    }

    private void NotifyAndDestroyObject(GameObject targetObject)
    {
        NotifyObjectCollected(targetObject);
        Destroy(targetObject);
    }

    // Clean up processed objects occasionally (when they're destroyed)
    void Update()
    {
        // Optional: clean up IDs of destroyed objects to save memory (if needed)
        // This is rare but prevents memory leak in very long gameplay
    }
}