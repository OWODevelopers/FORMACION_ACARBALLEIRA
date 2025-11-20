using UnityEngine;

namespace OWOGame
{
    using System;

    public static class EventManager
    {
        public static event Action<string> OnIpReceived;

        public static void InvokeIpEvent(string ip)
        {
            OnIpReceived?.Invoke(ip);
        }
    }

    public class IpButton : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button button;

        [SerializeField]
        private TMPro.TextMeshProUGUI buttonText;

        private void Awake()
        {
            button.onClick.AddListener(NotifyIpSelected);
        }
        private void OnDestroy()
        {
            button.onClick.RemoveListener(NotifyIpSelected);
        }

        public void SetIp(string ip)
        {
            buttonText.text = ip;
        }

        private void NotifyIpSelected()
        {
            EventManager.InvokeIpEvent(buttonText.text);
        }
    }
}

