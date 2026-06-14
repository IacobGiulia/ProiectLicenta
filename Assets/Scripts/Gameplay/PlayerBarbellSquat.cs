using UnityEngine;

public class PlayerBarbellSquat : MonoBehaviour
{
    [Header("Player Barbell (on back)")]
    public GameObject barbellSquat;
    public Animator animator;

    [Header("Back Anchor")]
    public Transform backBone; 

    [Header("Barbell Positioning")]
    public Vector3 barbellOffset = Vector3.zero;
    public Vector3 barbellRotationOffset = Vector3.zero;

    [Header("Timing")]
    public float showBarbellDelay = 0f;
    public string squatStateName = "Barbell Squat";

    private bool barbellActive;
    private float activationTime;
    private GameObject currentRackBarbell;
    private bool hasEnteredSquat = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        SetRenderers(barbellSquat, false);

        if (backBone == null)
            FindBackBone();
    }

    private void LateUpdate()
    {
        if (!barbellActive)
            return;

        float elapsed = Time.time - activationTime;

        if (!IsRendererVisible(barbellSquat) && elapsed >= showBarbellDelay)
        {
            SetRenderers(barbellSquat, true);
            return;
        }

        if (IsRendererVisible(barbellSquat))
        {
            UpdateBarbellTransform();

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName(squatStateName))
            {
                hasEnteredSquat = true;
                return;
            }

            if (hasEnteredSquat && !state.IsName(squatStateName))
            {
                hasEnteredSquat = false;
                HideBarbell();
            }
        }
    }

    #region Public API

    public void ShowBarbell(GameObject rackBarbellToHide)
    {
        if (rackBarbellToHide == null)
        {
            return;
        }

        currentRackBarbell = rackBarbellToHide;
        SetRenderers(currentRackBarbell, false);

        barbellActive = true;
        activationTime = Time.time;

        if (showBarbellDelay <= 0f)
            SetRenderers(barbellSquat, true);
    }

    public void HideBarbell()
    {
        barbellActive = false;

        SetRenderers(barbellSquat, false);

        if (currentRackBarbell != null)
        {
            SetRenderers(currentRackBarbell, true);
            currentRackBarbell.SetActive(true);
            currentRackBarbell = null;
        }
    }

    #endregion

    #region Helpers

    private void UpdateBarbellTransform()
    {
        if (backBone == null) return;

        barbellSquat.transform.position = backBone.position + barbellOffset;
        barbellSquat.transform.rotation =
            backBone.rotation * Quaternion.Euler(barbellRotationOffset);
    }

    private void SetRenderers(GameObject obj, bool state)
    {
        if (obj == null) return;

        foreach (var r in obj.GetComponentsInChildren<MeshRenderer>(true))
            r.enabled = state;
    }

    private bool IsRendererVisible(GameObject obj)
    {
        if (obj == null) return false;

        var r = obj.GetComponentInChildren<MeshRenderer>();
        return r != null && r.enabled;
    }

    private void FindBackBone()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            string n = t.name.ToLower();
            if (n.Contains("spine") || n.Contains("chest"))
            {
                backBone = t;
                return;
            }
        }
    }

    #endregion
}
