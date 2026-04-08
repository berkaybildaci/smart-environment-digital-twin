using UnityEngine;

public class AudioDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float threshold = 0.01f;
    [Tooltip("Enable only on the GameObject with the active AudioListener")]
    public bool useOnAudioFilterRead = false;

    [Header("Capture")]
    public CameraPicture cameraPicture;

    private float currentRMS = 0f;
    private bool wasAboveThreshold = false;
    private AudioSource[] cachedSources;

    void Start()
    {
        if (!useOnAudioFilterRead)
        {
            cachedSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        }
    }

    void Update()
    {
        float level = useOnAudioFilterRead ? currentRMS : EstimatePerceivedVolume();
        bool isAboveThreshold = level > threshold;

        if (isAboveThreshold && !wasAboveThreshold)
        {
            Debug.Log($"[{gameObject.name}] Sound detected! Level: {level:F4}");

            if (cameraPicture != null)
            {
                cameraPicture.TriggerCapture();
                Debug.Log($"[{gameObject.name}] Triggered RGB + Depth capture from all cameras.");
            }
        }

        wasAboveThreshold = isAboveThreshold;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!useOnAudioFilterRead) return;

        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i] * data[i];
        }
        currentRMS = Mathf.Sqrt(sum / data.Length);
    }

    float EstimatePerceivedVolume()
    {
        float totalVolume = 0f;
        for (int i = 0; i < cachedSources.Length; i++)
        {
            AudioSource source = cachedSources[i];
            if (source == null || !source.isPlaying) continue;

            float distance = Vector3.Distance(transform.position, source.transform.position);
            float attenuation = CalculateAttenuation(source, distance);
            totalVolume += source.volume * attenuation;
        }
        return totalVolume;
    }

    float CalculateAttenuation(AudioSource source, float distance)
    {
        if (distance <= source.minDistance) return 1f;
        if (distance >= source.maxDistance) return 0f;

        // Logarithmic rolloff: inverse distance, normalized between min and max
        return source.minDistance / distance;
    }
}
