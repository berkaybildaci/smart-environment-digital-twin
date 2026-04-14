using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPicture : MonoBehaviour
{
    public CameraFeed[] feeds;
    public int FileCounter = 0;

    private void LateUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(CaptureAll());
        }
    }

    public void TriggerCapture()
    {
        StartCoroutine(CaptureAll());
    }

    IEnumerator CaptureAll()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < feeds.Length; i++)
        {
            if (feeds[i] == null || feeds[i].RGBTexture == null) continue;
            SaveFeed(feeds[i], "Cam" + (i + 1));
        }

        FileCounter++;
        Debug.Log("Captured frame: " + FileCounter);
    }

    void SaveFeed(CameraFeed feed, string camName)
    {
        string directory = Application.dataPath + "/Backgrounds/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        SaveRT(feed.RGBTexture, directory + FileCounter + "_" + camName + "_rgb.png");
        SaveRT(feed.DepthTexture, directory + FileCounter + "_" + camName + "_depth.png");
    }

    void SaveRT(RenderTexture rt, string path)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        File.WriteAllBytes(path, tex.EncodeToPNG());
        Destroy(tex);
    }
}
