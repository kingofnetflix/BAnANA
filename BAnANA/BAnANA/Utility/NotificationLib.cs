using BepInEx;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;

namespace BAnANA.Utility
{
    public class NotifificationLibBAnANA : MonoBehaviour
    {
        // all of this stuff is not made by me but lars
        static GameObject MainCamera;
        static GameObject HUDObj;
        static GameObject HUDObj2;
        static Text Testtext;
        static Material AlertText = new Material(Shader.Find("GUI/Text Shader"));
        static int NotificationDecayTime = 150;
        static int NotificationDecayTimeCounter = 0;
        public static int NoticationThreshold = 5; //Amount of notifications before they stop queuing up
        static string[] Notifilines;
        static string newtext;
        public static string PreviousNotifi;
        static bool HasInitBAnANA = false;
        static Text NotifiText;
        public static bool IsEnabled = true;
        static float smth = 0f;
        public static bool sendOnce = false;
        
        public void Awake()
        {
            // Plugin startup logic
            Debug.Log($"Plugin NotificationLibBAnANA is loaded!");
            if (HasInitBAnANA == false)
            {
                Init();
                HasInitBAnANA = true;
            }
        }
        

        public static void Init()
        {
            //this is mostly copy pasted from LHAX, which was also made by me.
            //LHAX got leaked the day before this. so i might as well make this public cus people asked me to.

            //if your using other mod menus with this library then it wont work. dont make issues for it.
            MainCamera = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/Main Camera");
            HUDObj = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Cube);
            HUDObj2 = new GameObject();
            HUDObj2.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.name = "NOTIFICATIONLIB_HUD_OBJ";
            HUDObj.AddComponent<Canvas>();
            HUDObj.AddComponent<CanvasScaler>();
            HUDObj.AddComponent<GraphicRaycaster>();
            Debug.Log("step 9");
            HUDObj.GetComponent<Canvas>().enabled = true;
            Debug.Log("step 10");
            HUDObj.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            Debug.Log("step 11");
            HUDObj.GetComponent<Canvas>().worldCamera = MainCamera.GetComponent<Camera>();
            Debug.Log("step 12");
            HUDObj.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
            Debug.Log("step 13");
            HUDObj.GetComponent<RectTransform>().position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
            Debug.Log("step 14");
            HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z - 4.6f);
            HUDObj.transform.parent = HUDObj2.transform;
            HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            var Temp = HUDObj.GetComponent<RectTransform>().rotation.eulerAngles;
            Temp.y = -270f;
            HUDObj.transform.localScale = new Vector3(1f, 1f, 1f);
            HUDObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(Temp);
            GameObject TestText = new GameObject();
            TestText.transform.parent = HUDObj.transform;
            Testtext = TestText.AddComponent<Text>();
            Testtext.text = "";
            Testtext.fontSize = 10;
            Testtext.font = GameObject.Find("COC Text").GetComponent<Text>().font;
            Testtext.rectTransform.sizeDelta = new Vector2(260, 70);
            Testtext.alignment = TextAnchor.LowerCenter;
            Testtext.rectTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
            Testtext.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            Testtext.material = AlertText;
            NotifiText = Testtext;
            Debug.Log("[NotificationLibBAnANA] init ran");
        }

        public void FixedUpdate()
        {
            //This is a bad way to do this, but i do not want to rely on utila.
            HUDObj2.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z);
            HUDObj2.transform.rotation = MainCamera.transform.rotation;
            //HUDObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1.6f);
            if (Testtext.text != "") //THIS CAUSES A MEMORY LEAK!!!!! -no longer causes a memory leak
            {
                NotificationDecayTimeCounter++;
                if (NotificationDecayTimeCounter > NotificationDecayTime)
                {
                    Notifilines = null;
                    newtext = "";
                    NotificationDecayTimeCounter = 0;
                    Notifilines = Testtext.text.Split(Environment.NewLine.ToCharArray()).Skip(1).ToArray();
                    foreach (string Line in Notifilines)
                    {
                        if (Line != "")
                        {
                            newtext = newtext + Line + "\n";
                        }
                    }

                    Testtext.text = newtext;
                }
            }
            else
            {
                NotificationDecayTimeCounter = 0;
            }
        }

        public static void SendNotification(string NotificationText)
        {
            if (IsEnabled)
            {
                if (!NotificationText.Contains(Environment.NewLine))
                {
                    NotificationText = NotificationText + Environment.NewLine;
                }
                NotifiText.supportRichText = true;
                NotifiText.text = NotifiText.text + NotificationText;
                PreviousNotifi = NotificationText;
            }
        }
        public static void SendNotifOnce(string text)
        {
            if (IsEnabled)
            {
                if (sendOnce == false)
                {
                    if (!text.Contains(Environment.NewLine))
                    {
                        text += Environment.NewLine;
                    }
                    NotifiText.text = NotifiText.text + text;
                    PreviousNotifi = text;
                    sendOnce = true;
                }
            }
        }

        public static void SendWithCooldown(string YOURTEXT, float TIMER)
        {
            if (IsEnabled)
            {
                if (NotifiText == null)
                {
                    Debug.LogError("NotifiText is null (wtf?)");
                    return;
                }
                if (smth < Time.time)
                {
                    if (!YOURTEXT.Contains(Environment.NewLine))
                    {
                        YOURTEXT += Environment.NewLine;
                    }
                    NotifiText.text = NotifiText.text + YOURTEXT;
                    PreviousNotifi = YOURTEXT;
                    smth = Time.time + TIMER;
                }
            }
        }

        public static void ClearAllNotifications()
        {
            NotifiText.text = "";
        }
        public static void ClearPastNotifications(int amount)
        {
            string[] Notifilines = null;
            string newtext = "";
            Notifilines = NotifiText.text.Split(Environment.NewLine.ToCharArray()).Skip(amount).ToArray();
            foreach (string Line in Notifilines)
            {
                if (Line != "")
                {
                    newtext = newtext + Line + "\n";
                }
            }

            NotifiText.text = newtext;
        }
    }
}
