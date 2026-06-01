using System;
using UnityEngine;

namespace EMR.Medal
{

    [Serializable]
    public class MedalInfo
    {
        [SerializeField] string _name;
        [SerializeField] int _count = 1;

        public string Name => _name;
        public int Count => _count;
    }
}
