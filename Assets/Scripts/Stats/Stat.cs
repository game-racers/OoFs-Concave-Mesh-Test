using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Stats
{
    [System.Serializable]
    public class Stat 
    {
        [SerializeField]
        private int baseValue = 5;

        private List<int> modifiers = new List<int>();

        public int GetValue()
        {
            int value = baseValue;
            for (int i = 0; i < modifiers.Count; i++)
            {
                value += modifiers[i];
            }
            return value;
        }

        public void SetValue(int value)
        {
            baseValue = value;
        }

        public void AddModifier (int mod)
        {
            if (mod != 0)
                modifiers.Add(mod);
        }

        public void RemoveModifier (int mod)
        {
            if (mod != 0)
                modifiers.Remove(mod);
        }
    }
}
