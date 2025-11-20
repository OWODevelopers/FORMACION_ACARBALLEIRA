using System;
using UnityEngine;

namespace OWOGame
{
    public class OWOSkinService : MonoBehaviour
    {
        public static OWOSkinService Instance { get; private set; }

        private OWOSkin owoSkin;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool CanFeel()
        {
            return owoSkin != null && owoSkin.CanFeel();
        }

        public void SetSkin(OWOSkin skin)
        {
            owoSkin = skin;
        }

        public void Feel(Sensation sensation, int Priority = 0)
        {
            if (!CanFeel())
                return;
            owoSkin?.Feel(sensation, Priority);
        }

        public void Feel(String key, int Priority = 0, int intensity = 0)
        {
            if (!CanFeel())
                return;
            owoSkin?.Feel(key, Priority, intensity);
        }

        public void FeelWithMuscles(Sensation sensation, String muscleKey = "Right Arm", int Priority = 0, int intensity = 0)
        {
            if (!CanFeel())
                return;
            owoSkin?.FeelWithMuscles(sensation, muscleKey, Priority, intensity);
        }

        public void FeelWithMuscles(String key, String muscleKey = "Right Arm", int Priority = 0, int intensity = 0)
        {
            if (!CanFeel())
                return;
            owoSkin?.FeelWithMuscles(key, muscleKey, Priority, intensity);
        }

        public void StopAllFeedback()
        {
            owoSkin?.StopAllHapticFeedback();
        }
    }
}