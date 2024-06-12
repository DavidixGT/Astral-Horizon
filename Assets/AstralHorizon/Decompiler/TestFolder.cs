using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace AstralHorizon
{
    public class TestFolder : MonoBehaviour
    {
        void Start()
        {
            var dir = @"E:\Test\log";  // folder location
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "log.txt"), "blah blah, text");
        }
    }
}
