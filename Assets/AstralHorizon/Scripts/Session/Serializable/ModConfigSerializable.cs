using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AstralHorizon.Session.Serializable
{
    [Serializable]
    public class ModConfigSerializable
    {
        public ModConfigSerializable(string[] Mods) => mods = Mods;
        public string[] mods;
    }
}