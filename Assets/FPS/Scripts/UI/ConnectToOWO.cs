using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OWOGame
{
    public class ConnectToOWO : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_ipButtonPrefab;

        [SerializeField]
        private Transform m_container;

        private Dictionary<String, Sensation> m_sensationsMap = new Dictionary<String, Sensation>();
        private List<string> m_alreadyAddedIps = new List<string>();
        private List<string> m_selectedIPs = new List<string>();
        private List<GameObject> m_ipButtons = new List<GameObject>();

        private Coroutine m_showCoroutine;
        private OWOSkin owoSkin;

        private void Awake()
        {
            EventManager.OnIpReceived += ConnectToIP;
            owoSkin = new OWOSkin();

        }
        private void OnDestroy()
        {
            EventManager.OnIpReceived -= ConnectToIP;
            if (OWO.ConnectionState == ConnectionState.Connected)
                OWO.Disconnect();
        }

        public void StartScanForIPs()
        {
            OWO.StartScan(); // Esto empieza a buscar cuaquier aplicación OWO que este escuchando.
        }

        public void ShowIPs()
        {
            if (OWO.ConnectionState == ConnectionState.Connected)
            {
                // Si ya estamos conectados no tiene sentido buscar más IPs. Hay que mostrar algo al usuario.
                return;
            }

            ClearExistingButtons();
            StartScanForIPs();

            if (m_showCoroutine != null) StopCoroutine(m_showCoroutine);
            m_showCoroutine = StartCoroutine(WaitAndDisplayIPs());
        }

        private IEnumerator WaitAndDisplayIPs()
        {
            while (OWO.DiscoveredApps.Length <= 0)
            {
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            foreach (string discovered in OWO.DiscoveredApps) // DiscoveredApps es el que tiene todas las IPs que encuentre el SDK.
            {
                if (!m_alreadyAddedIps.Contains(discovered))
                {
                    m_alreadyAddedIps.Add(discovered);
                }
                CreateIPButton(discovered);
            }
        }

        private void ClearExistingButtons()
        {
            foreach (GameObject button in m_ipButtons)
            {
                Destroy(button);
            }
            m_ipButtons.Clear();
            m_selectedIPs.Clear();
        }

        private void CreateIPButton(string ip)
        {
            // Aquí haced que se añade el gameobject para seleccionar la IP que queráis. 
            // Cada botón debería de avisar a el controlador de que se ha seleccionado, dudo que os queráis conectar siempre a cualquier app que aparezca.
            // En este caso se añadirían a la lista selectedIPs con el método de abajo AddToSelected()

            GameObject go = Instantiate(m_ipButtonPrefab, m_container);
            m_ipButtons.Add(go);
            IpButton ipButton = go.GetComponentInChildren<IpButton>();
            ipButton.SetIp(ip);
            AddToSelected(ip);
        }

        public void AddToSelected(string ip)
        {
            if (m_selectedIPs.Contains(ip)) return;

            m_selectedIPs.Add(ip);
        }

        public async void ConnectToIP(string ip)
        {
            List<string> ipList = new List<string> { ip };
            owoSkin.InitializeOWO(ipList);
        }

        public async void ConnectToAll()
        {
            owoSkin.InitializeOWO(m_selectedIPs);
        }

        public BakedSensation[] AllBakedSensations()
        {
            var result = new List<BakedSensation>();

            foreach (var sensation in m_sensationsMap.Values)
            {
                if (sensation is BakedSensation baked)
                {
                    Debug.Log("Registered baked sensation: " + baked.name);
                    result.Add(baked);
                }
                else
                {
                    Debug.Log("Sensation not baked? " + sensation);
                    continue;
                }
            }
            return result.ToArray();
        }
    }
}
