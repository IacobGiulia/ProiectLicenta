using UnityEngine;
using UnityEditor;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        #if UNITY_EDITOR

        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
