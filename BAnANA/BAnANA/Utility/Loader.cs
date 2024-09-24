using System.Reflection;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using static BAnANA.Utility.MethodManager;

namespace BAnANA.Utility
{
    [BepInPlugin(guid, title, version)]
    public class BAnANALoader : BaseUnityPlugin
    {
        public const string guid = "org.banana.ai";
        public const string title = "BAnANA";
        public const string version = "1.0.0";
        public static BAnANALoader instance;
        public static GameObject BAnANAObject = null;
        BAnANALoader() { new Harmony(guid).PatchAll(Assembly.GetExecutingAssembly()); instance = this; }

        public void Start()
        {
            Debug.LogWarning("BAnANAObject is creating, if BAnANA is not responding to you, that means this failed (or BAnANA sucks)! Check the BAnANA.Log file in your Gorilla Tag folder for more information.");
            if (!GameObject.Find("BAnANAObject"))
            {
                BAnANAObject = new GameObject("BAnANAObject");
                BAnANAObject.AddComponent<VoiceManager>();
                BAnANAObject.AddComponent<CoroutineHelper>();
                BAnANAObject.AddComponent<NotifificationLibBAnANA>();
                DontDestroyOnLoad(BAnANAObject);
                Debug.Log("BAnANAObject has been created and packed with all the classes successfully!");
            }
        }
    }
}
