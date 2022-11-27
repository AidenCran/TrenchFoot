using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class HelperMonoBehaviour : MonoBehaviour
    {
        #region Singleton

        public static HelperMonoBehaviour Instance { get; private set; }

        private void Singleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        void Awake() => Singleton();

        public void InvokeActionOnComplete(Action onComplete, float delayFloat)
        {
            var delay = Helper.GetWait(delayFloat);
            
            StartCoroutine(EnterExitDelay());
            IEnumerator EnterExitDelay()
            {
                yield return delay;
                onComplete?.Invoke();
            }
        }
    }
}