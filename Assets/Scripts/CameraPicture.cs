using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering;

public class CameraPicture : MonoBehaviour
{
    public Material depthBlitMaterial;
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
        string directory = Application.dataPath + "/Backgrounds/";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // --- RGB capture ---
RenderTexture rtColor = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        rtColor.Create();

        cam.targetTexture = rtColor;
        bool wasActive = cam.gameObject.activeSelf;
        cam.gameObject.SetActive(true);
        cam.Render();
        cam.gameObject.SetActive(wasActive);
        cam.targetTexture = null;

        RenderTexture.active = rtColor;
        Texture2D colorImage = new Texture2D(rtColor.width, rtColor.height, TextureFormat.RGB24, false);
        colorImage.ReadPixels(new Rect(0, 0, rtColor.width, rtColor.height), 0, 0);
        colorImage.Apply();
        RenderTexture.active = null;
        Destroy(rtColor);

        File.WriteAllBytes(directory + FileCounter + "_" + camName + "_rgb.png",
            colorImage.EncodeToPNG());
        Destroy(colorImage);

        // --- Depth capture ---
RenderTexture rtDepth = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
rtDepth.depthStencilFormat = GraphicsFormat.D32_SFloat;
rtDepth.Create();

RenderTexture rtDepthColor = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
rtDepthColor.Create();

cam.targetTexture = rtDepth;
cam.gameObject.SetActive(true);
cam.Render();
cam.gameObject.SetActive(wasActive);
cam.targetTexture = null;

// Use the shader material instead of plain Blit
Graphics.Blit(rtDepth, rtDepthColor, depthBlitMaterial);
Destroy(rtDepth);

RenderTexture.active = rtDepthColor;
Texture2D depthImage = new Texture2D(rtDepthColor.width, rtDepthColor.height, TextureFormat.RGB24, false);
depthImage.ReadPixels(new Rect(0, 0, rtDepthColor.width, rtDepthColor.height), 0, 0);
depthImage.Apply();
RenderTexture.active = null;
Destroy(rtDepthColor);

File.WriteAllBytes(directory + FileCounter + "_" + camName + "_depth.png",
    depthImage.EncodeToPNG());
Destroy(depthImage);

        File.WriteAllBytes(directory + FileCounter + "_" + camName + "_depth.png",
            depthImage.EncodeToPNG());
        Destroy(depthImage);
    }
}