using UnityEngine;
using TMPro;
using System.Collections;

public class RestMessageUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text messageText;

    public float displayTime = 2f;

    private Coroutine showRoutine;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine(message));
    }

    public void ShowMessage(float timeLeft)
    {
        if (showRoutine != null)
            StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine(
            $"You must rest. Try again in {Mathf.Ceil(timeLeft)}s"));
    }

    private IEnumerator ShowRoutine(string message)
    {
        panel.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(displayTime);
        panel.SetActive(false);
        showRoutine = null;
    }
}