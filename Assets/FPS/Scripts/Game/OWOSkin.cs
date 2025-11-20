using OWOGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class OWOSkin
{
    private bool suitEnabled = false;
    private string filePath = Path.Combine(Application.dataPath, "Plugins/OWO/Sensations");
    private Dictionary<String, Sensation> sensationsMap = new Dictionary<String, Sensation>();
    private Dictionary<String, Muscle[]> muscleMap = new Dictionary<String, Muscle[]>();

    public Dictionary<string, Sensation> SensationsMap { get => sensationsMap; set => sensationsMap = value; }

    public OWOSkin()
    {
        RegisterAllSensationsFiles();
        DefineAllMuscleGroups();
    }
    ~OWOSkin()
    {
        Debug.Log("Destructor called");
        DisconnectOWO();
    }

    #region Skin Configuration

    private void RegisterAllSensationsFiles()
    {
        DirectoryInfo d = new DirectoryInfo(filePath);
        FileInfo[] Files = d.GetFiles("*.owo", SearchOption.AllDirectories);
        for (int i = 0; i < Files.Length; i++)
        {
            string filename = Files[i].Name;
            string fullName = Files[i].FullName;
            string prefix = Path.GetFileNameWithoutExtension(filename);
            if (filename == "." || filename == "..")
                continue;
            string tactFileStr = File.ReadAllText(fullName);
            try
            {
                Sensation test = Sensation.Parse(tactFileStr);
                SensationsMap.Add(prefix, test);
            }
            catch (Exception e) { UnityEngine.Debug.Log(e.Message); }
        }
    }
    private void DefineAllMuscleGroups()
    {
        Muscle[] leftArm = { Muscle.Arm_L.WithIntensity(70), Muscle.Pectoral_L.WithIntensity(90), Muscle.Dorsal_L.WithIntensity(70) };
        muscleMap.Add("Left Arm", leftArm);

        Muscle[] rightArm = { Muscle.Arm_R.WithIntensity(70), Muscle.Pectoral_R.WithIntensity(90), Muscle.Dorsal_R.WithIntensity(70) };
        muscleMap.Add("Right Arm", rightArm);

        Muscle[] bothArms = leftArm.Concat(rightArm).ToArray();
        muscleMap.Add("Both Arms", bothArms);
    }
    public async void InitializeOWO(List<string> ipList)
    {
        Debug.Log("Initializing OWO skin");

        var gameAuth = GameAuth.Create(AllBakedSensations()).WithId("211125");

        OWO.Configure(gameAuth);
        if (ipList.Count == 0) await OWO.AutoConnect();
        else
        {
            await OWO.Connect(ipList.ToArray());
        }

        if (OWO.ConnectionState == OWOGame.ConnectionState.Connected)
        {
            suitEnabled = true;
            Debug.Log("OWO suit connected.");
            OWO.Send(SensationsFactory.Create(intensityPercentage: 20, frequency: 70, durationSeconds: 1));
        }
        if (!suitEnabled) Debug.Log("OWO is not enabled?!?!");
    }
    private BakedSensation[] AllBakedSensations()
    {
        var result = new List<BakedSensation>();

        foreach (var sensation in SensationsMap.Values)
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
    public void DisconnectOWO()
    {
        Debug.Log("Disconnecting OWO skin.");
        OWO.Disconnect();
    }
    #endregion

    public void Feel(String key, int Priority = 0, int intensity = 0)
    {
        if (SensationsMap.ContainsKey(key))
        {
            Sensation toSend = SensationsMap[key];

            if (intensity != 0)
            {
                toSend = toSend.WithMuscles(Muscle.All.WithIntensity(intensity));
            }

            OWO.Send(toSend.WithPriority(Priority));
        }

        else Debug.Log("Feedback not registered: " + key);
    }

    public void Feel(Sensation sensation, int Priority = 0)
    {
        if (sensation != null)
        {
            OWO.Send(sensation.WithPriority(Priority));
        }

        else Debug.Log($"Feedback not registered: {sensation}");
    }

    public void FeelWithMuscles(String key, String muscleKey = "Right Arm", int Priority = 0, int intensity = 0)
    {
        Debug.Log($"FEEL WITH MUSCLES: {key} - {muscleKey}");

        if (!muscleMap.ContainsKey(muscleKey))
        {
            Debug.Log("MuscleGroup not registered: " + muscleKey);
            return;
        }

        if (SensationsMap.ContainsKey(key))
        {
            if (intensity != 0)
            {
                OWO.Send(SensationsMap[key].WithMuscles(muscleMap[muscleKey].WithIntensity(intensity)).WithPriority(Priority));
            }
            else
            {
                OWO.Send(SensationsMap[key].WithMuscles(muscleMap[muscleKey]).WithPriority(Priority));
            }
        }

        else Debug.Log("Feedback not registered: " + key);
    }

    public void StopAllHapticFeedback()
    {
        //StopRaining();
        OWO.Stop();
    }

    public bool CanFeel()
    {
        return suitEnabled;
    }

    #region Loops

    #region Raining

    //public void StartRaining()
    //{
    //    if (raining) return;
    //    raining = true;
    //    RainingFuncAsync();
    //}

    //public void StopRaining()
    //{
    //    if (!raining) return;
    //    raining = false;
    //}

    //public async Task RainingFuncAsync()
    //{
    //    while (raining)
    //    {
    //        if (!inMenu)
    //            Feel("Rain", 0);
    //        await Task.Delay(200);
    //    }
    //}

    #endregion

    #endregion
}
