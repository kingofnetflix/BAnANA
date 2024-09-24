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

        private static async Task PlayThroughPhoton(string resourcePath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream streaming = assembly.GetManifestResourceStream(resourcePath))
            {
                bool flag = streaming == null;
                if (flag)
                {
                    Debug.LogError("Failed to load resource " + resourcePath);
                    return;
                }
                byte[] buffer = new byte[streaming.Length];
                streaming.Read(buffer, 0, buffer.Length);
                string tempFilePath = Path.Combine(Application.temporaryCachePath, "tempAudio.mp3");
                File.WriteAllBytes(tempFilePath, buffer);
                using (UnityWebRequest unityWebRequest1 = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.MPEG))
                {
                    UnityWebRequestAsyncOperation operation = unityWebRequest1.SendWebRequest();
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }
                    if (unityWebRequest1.isNetworkError || unityWebRequest1.isHttpError)
                    {
                        Debug.Log(unityWebRequest1.error);
                    }
                    else
                    {
                        AudioClip myClip = DownloadHandlerAudioClip.GetContent(unityWebRequest1);
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.AudioClip;
                        GorillaTagger.Instance.myRecorder.AudioClip = myClip;
                        GorillaTagger.Instance.myRecorder.LoopAudioClip = false;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = true;
                        await Task.Delay((int)(myClip.length * 1000f) + 4000);
                        GorillaTagger.Instance.myRecorder.SourceType = Recorder.InputSourceType.Microphone;
                        GorillaTagger.Instance.myRecorder.AudioClip = null;
                        GorillaTagger.Instance.myRecorder.RestartRecording(true);
                        GorillaTagger.Instance.myRecorder.DebugEchoMode = false;
                        myClip = null;
                    }
                    operation = null;
                }
                UnityWebRequest unityWebRequest = null;
                File.Delete(tempFilePath);
                buffer = null;
                tempFilePath = null;
            }
            Stream stream = null;
        }

    }
}