using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreadmillUI : MonoBehaviour
{
    public Slider speedSlider;
    public TextMeshProUGUI statusText;
    public GameObject panel;

    public Color tooSlowColor = Color.blue;
    public Color tooFastColor = Color.red;
    public Color perfectColor = Color.green;

    public Image fillImage;
    private TreadmillPaceSystem currentTreadmill;

    void Start()
    {
        panel.SetActive(false);
    }

    public void SetTreadmill(TreadmillPaceSystem treadmill)
    {
        currentTreadmill = treadmill;
        panel.SetActive(true);
    }

    public void Hide()
    {
        currentTreadmill = null;
        panel.SetActive(false);
    }

    void Update()
    {
        if (currentTreadmill == null) return;

        float speed = currentTreadmill.currentSpeed;

        speedSlider.value = speed;

        if (speed < currentTreadmill.IdealMin)
        {
            statusText.text = "Too Slow";
            statusText.color = tooSlowColor;

            fillImage.color = tooSlowColor;
        }
        else if (speed > currentTreadmill.IdealMax)
        {
            statusText.text = "Too Fast";
            statusText.color = tooFastColor;

            fillImage.color = tooFastColor;
        }
        else
        {
            statusText.text = "Perfect Pace";
            statusText.color = perfectColor;

            fillImage.color=Color.Lerp(fillImage.color, perfectColor, Time.deltaTime * 5f); 
        }
    }
}