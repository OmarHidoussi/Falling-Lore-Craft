using UnityEngine;

public class BorderlessWindow : MonoBehaviour
{
    void Start()
    {
        // Set the window to borderless
        SetBorderless();
    }

    void SetBorderless()
    {
        Screen.fullScreen = false;
        Screen.SetResolution(1024, 600, false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
