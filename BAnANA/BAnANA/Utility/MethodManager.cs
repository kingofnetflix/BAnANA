using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BAnANA.Utility
{
    public static class MethodManager
    {
        private static Dictionary<Action, Coroutine> activeCoroutines = new Dictionary<Action, Coroutine>();

        public static void Toggle(Action method, float interval)
        {
            if (activeCoroutines.ContainsKey(method))
            {
                TurnOff(method);
            }
            else
            {
                TurnOn(method, interval);
            }
        }

        public static void TurnOn(Action method, float interval)
        {
            if (!activeCoroutines.ContainsKey(method))
            {
                Coroutine coroutine = CoroutineHelper.Instance.StartCoroutine(ToggleMethod(method, interval));
                activeCoroutines[method] = coroutine;
            }
        }

        public static void TurnOff(Action method)
        {
            if (activeCoroutines.ContainsKey(method))
            {
                CoroutineHelper.Instance.StopCoroutine(activeCoroutines[method]);
                activeCoroutines.Remove(method);
            }
        }

        private static IEnumerator ToggleMethod(Action method, float interval)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval > 0 ? interval : 0.001f); // Ensure a minimal wait time
                method.Invoke();
            }
        }

        public class CoroutineHelper : MonoBehaviour
        {
            public static CoroutineHelper Instance;

            private void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
