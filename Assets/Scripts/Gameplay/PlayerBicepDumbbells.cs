using UnityEngine;

public class PlayerDumbbells : MonoBehaviour
{
    [Header("Player's dumbbells")]
    public GameObject dumbbell_R;
    public GameObject dumbbell_L;
    public Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found on this GameObject!");
        }

        if (dumbbell_R != null) dumbbell_R.SetActive(false);
        if (dumbbell_L != null) dumbbell_L.SetActive(false);
    }

    public void ShowDumbbells()
    {
        if (dumbbell_R != null) dumbbell_R.SetActive(true);
        if (dumbbell_L != null) dumbbell_L.SetActive(true);
    }

    public void HideDumbbells()
    {
        if (dumbbell_R != null) dumbbell_R.SetActive(false);
        if (dumbbell_L != null) dumbbell_L.SetActive(false);
    }

    private void LateUpdate()
    {
        if ((dumbbell_R != null && dumbbell_R.activeSelf) || (dumbbell_L != null && dumbbell_L.activeSelf))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("BicepCurl") && !state.IsName("FrontRaises"))
            {
                HideDumbbells();
            }
        }
    }
}
