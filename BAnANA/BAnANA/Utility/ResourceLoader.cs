using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace BAnANA.Utility
{
    public class ResourceLoader
    {
        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }

        public static async Task PlayThroughPhoton(string resourcePath)
        {
            Stream stream = ResourceLoader.GetEmbeddedResourceStream(resourcePath);
            using (stream)
            {
                if (stream == null)
                {
                    Debug.LogError("Failed to load resource " + resourcePath);
                    return;
                }
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                string tempFilePath = Path.Combine(Application.temporaryCachePath, "tempAudio.wav");
                File.WriteAllBytes(tempFilePath, array);
                UnityWebRequest requesttt = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.WAV);
                using (requesttt)
                {
                    UnityWebRequestAsyncOperation operation = requesttt.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }
                    if (requesttt.isNetworkError || requesttt.isHttpError)
                    {
                        Debug.Log(requesttt.error);
                    }
                    else
                    {
                        AudioClip content = DownloadHandlerAudioClip.GetContent(requesttt);
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                        GorillaTagger.Instance.myRecorder.AudioClip = content;
                        GorillaTagger.Instance.myRecorder.LoopAudioClip = false;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
                        await Task.Delay((int)(content.length * 1000f) + 4000);
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
                        GorillaTagger.Instance.myRecorder.AudioClip = null;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
                    }
                }
                File.Delete(tempFilePath);
            }
        }

    }
}