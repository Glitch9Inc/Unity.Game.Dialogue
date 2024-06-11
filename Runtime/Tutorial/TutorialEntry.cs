using System;
using UnityEngine;

namespace Glitch9.Routina.Tutorial
{
    public class TutorialEntry
    {
        public string key;
        public string objectName;
        public Action action;
        public GameObject target;
        public bool vibrate;  
    }
}