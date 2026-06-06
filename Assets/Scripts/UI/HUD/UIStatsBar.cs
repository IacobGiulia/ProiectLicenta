using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIStatsBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public TextMeshProUGUI valueText;

    public float lerpSpeed = 5f;

    private float targetValue;

    void Start()
    {
        if (slider != null)
        {
            slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            targetValue = slider.value;
        }

        if (fillImage != null)
            ChangeColor(targetValue);
    }

    void Update()
    {
        if (slider != null)
        {
            slider.value = Mathf.MoveTowards(slider.value, targetValue, lerpSpeed * Time.deltaTime);
            if (fillImage != null)
                ChangeColor(slider.value);

            if (valueText != null)
            {
                float percent = (slider.value / slider.maxValue) * 100f;
                valueText.text = percent.ToString("0") + "%";
            }
        }
    }

    public void SetValue(float value)
    {
        targetValue = Mathf.Clamp(value, slider.minValue, slider.maxValue);
    }

    void ChangeColor(float value)
    {
        if (value > slider.maxValue * 0.6f)
        {
            fillImage.color = Color.green;
        }
        else if (value > slider.maxValue * 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}
