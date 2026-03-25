using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public AudioSource testSFX1;
    public AudioSource testSFX2;
    public AudioClip testClip1;
    public AudioClip testClip2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
     void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            testSFX1.PlayOneShot(testClip1, 1f);
            Debug.Log("Play Sound");
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            testSFX2.PlayOneShot(testClip2, 1f);
            Debug.Log("Play Sound");
        }
    }
}
