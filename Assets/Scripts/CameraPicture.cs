using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering;

public class CameraPicture : MonoBehaviour
{
    public int FileCounter = 0;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;

    private void LateUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(CaptureAll());
        }
    }

    IEnumerator CaptureAll()
    {
        yield return new WaitForEndOfFrame();

        SaveCam(cam1, "Cam1");
        SaveCam(cam2, "Cam2");
        SaveCam(cam3, "Cam3");

        FileCounter++;
        Debug.Log("Captured frame: " + FileCounter);
    }

    void SaveCam(Camera cam, string camName)
    {
        // Create a fresh RT each capture so we don't need the camera active
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        rt.depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;
        rt.Create();

        cam.targetTexture = rt;

        // Temporarily enable just long enough to render one frame
        bool wasActive = cam.gameObject.activeSelf;
        cam.gameObject.SetActive(true);
        cam.Render();
        cam.gameObject.SetActive(wasActive);

        cam.targetTexture = null;

        RenderTexture.active = rt;
        Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        image.Apply();
        RenderTexture.active = null;

        Destroy(rt);

        string directory = Application.dataPath + "/Backgrounds/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
        File.WriteAllBytes(directory + FileCounter + "_" + camName + ".png", bytes);
    }
}