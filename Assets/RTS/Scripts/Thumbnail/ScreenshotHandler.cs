using UnityEngine;
using System.IO;

public class ScreenshotHandler : MonoBehaviour
{
    public KeyCode screenshotKey = KeyCode.P;
    private int screenshotCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string filename = "screenshot_" + screenshotCount + ".png";
        string path = Path.Combine(Application.persistentDataPath, filename);

        // You can also save to the project root in the editor:
        // string path = Path.Combine(Application.dataPath, "..", filename);

        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Screenshot saved to: " + path);
        screenshotCount++;
    }
}