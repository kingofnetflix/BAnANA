using UnityEngine;
using UnityEngine.Windows.Speech;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using BAnANA.Utility;
using System.IO;
using System.Media;
using System;
using Photon.Voice.Unity;

namespace BAnANA
{
    public class VoiceManager : MonoBehaviour
    {
        private string[] wakeWords = { "banana", "jarvis", "alexa", "hey google" }; // implementation of custom wake words hard coded
        private Dictionary<string, System.Action> commands;
        private KeywordRecognizer keywordRecognizer;
        private KeywordRecognizer commandRecognizer;
        private bool listeningForCommand = false;
        private Coroutine commandListeningCoroutine;
        /* below are tts lines. if you are adding your own, make sure its imported in your visual studio project
         * and it's build action is "Embedded Resource"
         * you can change it by right clicking on the file
         * and going to it's properties. i used elevenlabs, voice Serena!
        */
        private static string[] TTSResponseToWakeWAV = new string[]
        {
            "BAnANA.Resources.yes.wav",
            "BAnANA.Resources.whatsup.wav",
            "BAnANA.Resources.howhelp.wav"
        };
        private static string[] TTSResponseToOrderWAV = new string[]
         {
            "BAnANA.Resources.alright.wav",
            "BAnANA.Resources.certainly.wav",
            "BAnANA.Resources.noproblem.wav"
        };
    public void Start()
        {
            try
            {
                commands = new Dictionary<string, System.Action>
                {
                    { "disconnect me", () => PhotonNetwork.Disconnect() },
                    { "toggle iron monkey", () => MethodManager.Toggle(Mods.IronMonke, 0f) },
                    { "toggle spider monkey", () => MethodManager.Toggle(Mods.Webshooters, 0f) } //you can use my method manager here to toggle mods you create! :D
                    // add more commands and their corresponding actions here
                };
                keywordRecognizer = new KeywordRecognizer(wakeWords);
                keywordRecognizer.OnPhraseRecognized += OnWakeWordRecognized;
                keywordRecognizer.Start();
                Debug.Log("BAnANA has been initalized!");
            }
            catch (Exception ex)
            {
                Debug.LogError("BAnANA has crashed. I have fallen and can't get up! Check your BAnANA.log file in your Gorilla Tag folder for more information.");
                NotifificationLibBAnANA.SendNotification("BAnANA has crashed. Check your BAnANA.log file in your Gorilla Tag folder for more information.");
                File.WriteAllText("BAnANA.log", ex.ToString());
            }
        }

        void OnWakeWordRecognized(PhraseRecognizedEventArgs args)
        {
            if (Array.Exists(wakeWords, element => element == args.text))
            {
                Debug.Log($"wake word recognized {args.text}");
                listeningForCommand = true;
                StartListeningForCommands();
                commandListeningCoroutine = StartCoroutine(Timeout());
                TTSResponse(true);
                NotifificationLibBAnANA.SendNotification("listening..");
            }
        }

        void StartListeningForCommands()
        {
            if (commandRecognizer != null && commandRecognizer.IsRunning)
            {
                commandRecognizer.Stop();
                commandRecognizer.Dispose();
            }

            string[] commandKeywords = new string[commands.Keys.Count];
            commands.Keys.CopyTo(commandKeywords, 0);
            commandRecognizer = new KeywordRecognizer(commandKeywords);
            commandRecognizer.OnPhraseRecognized += OnCommandRecognized;
            commandRecognizer.Start();
        }

        void OnCommandRecognized(PhraseRecognizedEventArgs args)
        {
            if (listeningForCommand && commands.ContainsKey(args.text))
            {
                Debug.Log($"recognized a command: {args.text}");
                commands[args.text]?.Invoke();
                listeningForCommand = false;
                if (commandListeningCoroutine != null)
                {
                    StopCoroutine(commandListeningCoroutine);
                }
                TTSResponse(false);
                NotifificationLibBAnANA.SendNotification($"recognized {args.text}");
            }
        }

        public static void TTSResponse(bool wake = true)
        {
            // this system is kinda butt
            if (wake)
            {
                int index = UnityEngine.Random.Range(0, TTSResponseToWakeWAV.Length);
                string resourceName = TTSResponseToWakeWAV[index];

                Stream wavStream = ResourceLoader.GetEmbeddedResourceStream(resourceName);
                
                if (wavStream == null)
                {
                    Debug.LogError($"resource {resourceName} not found. wake is {wake}");
                    return;
                }
                Debug.Log(resourceName);
                using (SoundPlayer player = new SoundPlayer(wavStream))
                {
                    player.Play();
                }
            }
            if (!wake)
            {
                int index = UnityEngine.Random.Range(0, TTSResponseToOrderWAV.Length);
                string resourceName = TTSResponseToOrderWAV[index];

                Stream wavStream = ResourceLoader.GetEmbeddedResourceStream(resourceName);
                if (wavStream == null)
                {
                    Debug.LogError($"resource {resourceName} not found. wake is {wake}");
                    return;
                }
                Debug.Log(resourceName);
                using (SoundPlayer player = new SoundPlayer(wavStream))
                {
                    player.Play();
                }
            }
        }
        IEnumerator Timeout()
        {
            yield return new WaitForSeconds(6);
            if (listeningForCommand)
            {
                Debug.Log("BAnANA was listening for too long, stopping..");
                listeningForCommand = false;
                if (commandRecognizer != null && commandRecognizer.IsRunning)
                {
                    commandRecognizer.Stop();
                    commandRecognizer.Dispose();
                }
                NotifificationLibBAnANA.SendNotification("stopped listening");
            }
        }

        void OnDestroy()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
                keywordRecognizer.Dispose();
            }
            if (commandRecognizer != null && commandRecognizer.IsRunning)
            {
                commandRecognizer.Stop();
                commandRecognizer.Dispose();
            }
            if (commandListeningCoroutine != null)
            {
                StopCoroutine(commandListeningCoroutine);
            }
        }
    }
}
