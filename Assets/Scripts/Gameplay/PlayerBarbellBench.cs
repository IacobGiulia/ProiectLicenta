using UnityEngine;

public class PlayerBarbellBench : MonoBehaviour
{
    [Header("Player Barbell (in hands)")]
    public GameObject barbellBench;
    public Animator animator;

    [Header("Hand Bones")]
    public Transform leftHandBone;
    public Transform rightHandBone;

    [Header("Barbell Positioning")]
    public Vector3 barbellOffset = Vector3.zero;
    public Vector3 barbellRotationOffset = Vector3.zero;

    [Header("Rotation Mode")]
    public RotationMode rotationMode = RotationMode.AlignWithHands;

    [Header("Timing")]
    public float showBarbellDelay = 0f;
    public string benchPressStateName = "Bench Press";

    public enum RotationMode
    {
        AlignWithHands,
        FixedRotation,
        WorldAligned
    }

    private bool barbellActive;
    private float activationTime;
    private GameObject currentBenchBarbell;
    private bool hasEnteredBenchPress = false;

    #region Unity

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        SetRenderers(barbellBench, false);

        if (leftHandBone == null || rightHandBone == null)
            FindHandBones();
    }

    private void LateUpdate()
    {
        if (!barbellActive)
            return;

        float elapsed = Time.time - activationTime;

        if (!IsRendererVisible(barbellBench) && elapsed >= showBarbellDelay)
        {
            SetRenderers(barbellBench, true);
            return;
        }

        if (IsRendererVisible(barbellBench))
        {
            UpdateBarbellTransform();

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName(benchPressStateName))
            {
                hasEnteredBenchPress = true;
                return;
            }

            if (hasEnteredBenchPress && !state.IsName(benchPressStateName))
            {
                hasEnteredBenchPress = false;
                HideBarbell();
            }
        }
    }

    #endregion

    #region Public API (apelată din Interactable)

    public void ShowBarbell(GameObject benchBarbellToHide)
    {
        if (benchBarbellToHide == null)
        {
            return;
        }

        currentBenchBarbell = benchBarbellToHide;

        SetRenderers(currentBenchBarbell, false);

        barbellActive = true;
        activationTime = Time.time;

        if (showBarbellDelay <= 0f)
            SetRenderers(barbellBench, true);
    }

    public void HideBarbell()
    {
        barbellActive = false;

        SetRenderers(barbellBench, false);

        if (currentBenchBarbell != null)
        {
            SetRenderers(currentBenchBarbell, true);
            currentBenchBarbell.SetActive(true);
            currentBenchBarbell = null;
        }
    }

    #endregion

    #region Helpers

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

    private void UpdateBarbellTransform()
    {
        if (leftHandBone == null || rightHandBone == null)
            return;

        Vector3 mid = (leftHandBone.position + rightHandBone.position) / 2f;
        barbellBench.transform.position = mid + barbellOffset;

        Vector3 dir = rightHandBone.position - leftHandBone.position;
        if (dir.magnitude < 0.01f) return;

        switch (rotationMode)
        {
            case RotationMode.AlignWithHands:
                barbellBench.transform.rotation =
                    Quaternion.LookRotation(dir.normalized) *
                    Quaternion.Euler(barbellRotationOffset);
                break;

            case RotationMode.FixedRotation:
                barbellBench.transform.rotation =
                    Quaternion.Euler(barbellRotationOffset);
                break;

            case RotationMode.WorldAligned:
                barbellBench.transform.rotation =
                    Quaternion.identity *
                    Quaternion.Euler(barbellRotationOffset);
                break;
        }
    }

    private void FindHandBones()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            string n = t.name.ToLower();
            if (n.Contains("left") && n.Contains("hand")) leftHandBone = t;
            if (n.Contains("right") && n.Contains("hand")) rightHandBone = t;
        }
    }

    #endregion
}
