using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using GameDatabase.Storage;
using Ionic.Zlib;
using System.Text;

namespace AstralHorizon
{
    public class Decompiler : MonoBehaviour
    {
        private void Start()
        {
            Decompile(@"E:\UnityProjects\LastHope\Astral-Horizon\Mods\End_Of_Paradox_Reloaded.V1.5.5.5.ehmod", @"E:\Test");
        }
        private void Decompile(string modPath, string decompileTo)
        {
            bool containsHeader = false;
            using var file = EncryptionRemover.RemoveEncryption(modPath, ref containsHeader);
            var zlibStream = new ZlibStream(file, CompressionMode.Decompress);
            var content = zlibStream;
            if (containsHeader)
            {
                content.ReadInt32();
                content.ReadString();
                content.ReadString();
                content.ReadInt32();
                content.ReadInt32();
            }
            else
            {
                content.ReadString();
                content.ReadString();
            }        
            //Debug.LogError("11111111111111111111111111111111: " + Name);
            var itemCount = 0;
            Directory.CreateDirectory(decompileTo + @"\Jsons");
            Directory.CreateDirectory(decompileTo + @"\Sprites");
            Directory.CreateDirectory(decompileTo + @"\localization");
            Directory.CreateDirectory(decompileTo + @"\WavAudioClip");
            Directory.CreateDirectory(decompileTo + @"\OggAudioClip");
            while (true)
            {
                
                var type = content.ReadByte();
                if (type == -1) // end of file
                {
                    break;
                }
                else if (type == 1) // json
                {
                    
                    var fileContent = content.ReadString();
                    CreateJsonFile(fileContent,itemCount.ToString() + ".json",decompileTo + @"\Jsons");
                    itemCount++;
                }
                else if (type == 2) // image
                {
                    var nameImg = content.ReadString();
                    var image = content.ReadByteArray();
                    //Debug.Log(image.Length);
                    CreateFile(image, nameImg + ".png",  decompileTo + @"\Sprites");
                    
                }
                else if (type == 3) // localization
                {
                    var nameLocalization = content.ReadString();
                    var text = content.ReadString();
                    CreateLocalization(text, nameLocalization + ".xml",decompileTo + @"\localization");
                    
                }
                else if (type == 4) // wav audioClip
                {
                    var nameWav = content.ReadString();
                    var audioClip = content.ReadByteArray();
                    CreateFile(audioClip, nameWav + ".wav", decompileTo + @"\WavAudioClip");
                    
                }
                else if (type == 5) // ogg audioClip
                {
                    var nameOgg = content.ReadString();
                    var audioClip = content.ReadByteArray();
                    CreateFile(audioClip, nameOgg + ".ogg", decompileTo + @"\OggAudioClip");
                    
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ");
        }
    private void CreateFile(byte[] img,string name, string path)
    {
        FileStream debug = File.Create(path + @"\" + name );
        debug.Write(img);
        debug.Flush();
    }
    private void CreateJsonFile(string content,string name, string path)
    {
                FileStream debug = File.Create(path + @"\" + name );
        debug.Write(Encoding.Default.GetBytes(content));
        debug.Flush();
    }
    private void CreateLocalization(string content, string name, string path)
    {
        FileStream debug = File.Create(path + @"\" + name );
        debug.Write(Encoding.Default.GetBytes(content));
        debug.Flush();
    }
    }
}
