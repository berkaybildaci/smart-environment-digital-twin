using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : MonoBehaviour
{
    private List<float> recordedSamples = new List<float>();
    private bool isRecording = false;
    private int sampleRate;
    private int channels;

    public void StartRecording()
    {
        recordedSamples.Clear();
        sampleRate = AudioSettings.outputSampleRate;
        channels = AudioSettings.speakerMode == AudioSpeakerMode.Stereo ? 2 : 1;
        isRecording = true;
        Debug.Log("Recording started...");
    }

    public void StopRecording(string fileName)
    {
            isRecording = false;

    string folder = Path.Combine(Application.dataPath, "Recordings"); // <- THIS builds Assets/Recordings
    if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

    string filePath = Path.Combine(folder, fileName + ".wav");
    SaveAsWav(filePath);
    Debug.Log("Saved to: " + filePath);
    }

    // This is called by Unity's audio engine on the audio thread
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isRecording) return;
        recordedSamples.AddRange(data);
    }

    private void SaveAsWav(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        using (var writer = new BinaryWriter(fileStream))
        {
            int dataLength = recordedSamples.Count * 2; // 16-bit samples

            // WAV Header
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(36 + dataLength);
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16);           // Subchunk size
            writer.Write((short)1);     // PCM format
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);    // Bits per sample
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(dataLength);

            // Audio Data
            foreach (float sample in recordedSamples)
            {
                short s = (short)Mathf.Clamp(sample * 32767f, short.MinValue, short.MaxValue);
                writer.Write(s);
            }

            
        }

        
    }
}