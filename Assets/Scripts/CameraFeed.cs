using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CameraFeed : MonoBehaviour
{
    [Header("Resolution")]
    public int width = 1920;
    public int height = 1080;

    [Header("Depth")]
    public Material depthBlitMaterial;

    [Header("UI Display (optional)")]
    public RawImage rgbDisplay;
    public RawImage depthDisplay;

    public RenderTexture RGBTexture { get; private set; }
    public RenderTexture DepthTexture { get; private set; }

    private Camera cam;
    private RenderTexture depthRaw;

    void Awake()
    {
        cam = GetComponent<Camera>();

        RGBTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        RGBTexture.Create();

        DepthTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        DepthTexture.Create();

        depthRaw = new RenderTexture(width, height, 24, RenderTextureFormat.Depth);
        depthRaw.depthStencilFormat = GraphicsFormat.D32_SFloat;
        depthRaw.Create();

        // Point camera at the RGB texture by default so it never renders to screen
        cam.targetTexture = RGBTexture;

        if (rgbDisplay != null) rgbDisplay.texture = RGBTexture;
        if (depthDisplay != null) depthDisplay.texture = DepthTexture;
    }

    void LateUpdate()
    {
        RenderFeeds();
    }

    public void RenderFeeds()
    {
        // RGB pass
        cam.targetTexture = RGBTexture;
        cam.Render();

        // Depth pass
        cam.targetTexture = depthRaw;
        cam.Render();

        // Leave targetTexture on RGBTexture so Unity's auto-render goes there too
        cam.targetTexture = RGBTexture;

        // Pass camera planes to the shader for manual depth linearization
        if (depthBlitMaterial != null)
        {
            depthBlitMaterial.SetFloat("_NearClip", cam.nearClipPlane);
            depthBlitMaterial.SetFloat("_FarClip", cam.farClipPlane);
            Graphics.Blit(depthRaw, DepthTexture, depthBlitMaterial);
        }
        else
        {
            Graphics.Blit(depthRaw, DepthTexture);
        }
    }

    void OnDestroy()
    {
        if (RGBTexture != null) { RGBTexture.Release(); Destroy(RGBTexture); }
        if (DepthTexture != null) { DepthTexture.Release(); Destroy(DepthTexture); }
        if (depthRaw != null) { depthRaw.Release(); Destroy(depthRaw); }
    }
}
