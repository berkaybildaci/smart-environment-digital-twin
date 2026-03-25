using UnityEngine;
using UnityEngine.InputSystem;

public class RecordingInput : MonoBehaviour
{
    public AudioRecorder recorder; // drag your Main Camera into this slot in the Inspector

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            recorder.StartRecording();
            Debug.Log("Recording started!");
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            recorder.StopRecording("my_recording");
            Debug.Log("Recording stopped!");
        }
    }
}