using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
            interactable.ShowUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
            interactable.ShowUI(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed!");
            Interactable interactable = GetClosestInteractable();
            if (interactable != null)
                interactable.Interact();
        }
    }

    private Interactable GetClosestInteractable()
    {
        Interactable[] interactables = Object.FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        Interactable closest = null;
        float minDistance = Mathf.Infinity;
        foreach (var i in interactables)
        {
            float dist = Vector3.Distance(transform.position, i.transform.position);
            if (dist < 100f && dist < minDistance) 
            {
                minDistance = dist;
                closest = i;
            }
        }
        return closest;
    }
}
