using System;
using System.IO;
using Ionic.Zlib;
using UnityEngine;
using CommonComponents.Serialization;
using AstralHorizon.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace GameDatabase.Storage
{
    public class FileDatabaseStorage : IDataStorage
    {
        private const uint _header = 0xDA7ABA5E;
        private readonly string _filename;

        public string Name { get; private set; }
        public string Id { get; private set; }
        public Version Version { get; private set; }
        public bool IsEditable => false;
        private static string _modPath;
        private static bool _containsHeader = false;

        public FileDatabaseStorage(string filename)
        {
            _filename = filename;
            _modPath = filename;//@"E:\UnityProjects\LastHope\Astral-Horizon\Mods" + "/Event-Horizon-ES-Mod 0.15.4";
            //Debug.LogError(filename + "         333333333");
            using (var file = new FileStream(_modPath, FileMode.Open, FileAccess.Read))//_fileName
            {
                ReadDataTillContent(file);
            }
        }

        public void UpdateItem(string id, string content)
        {
            throw new InvalidOperationException("FileDatabaseStorage.UpdateItem is not supported");
        }

        public void LoadContent(IContentLoader loader)
        {
            using var file = new FileStream(_modPath, FileMode.Open, FileAccess.Read);//_fileName
            var content = ReadDataTillContent(file);
            Debug.LogError("11111111111111111111111111111111: " + Name);
            var itemCount = 0;
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

                    try
                    {
                        loader.LoadJson(string.Empty, fileContent);
                        itemCount++;
                    }
                    catch (Exception e)
                    {
                        // Skip files with errors to allow loading mods created with differen database version
                        Debug.LogException(e);
                    }
                }
                else if (type == 2) // image
                {
                    var key = content.ReadString();
                    var image = content.ReadByteArray();
                    loader.LoadImage(key, new LazyImageDataLoader(image));
                }
                else if (type == 3) // localization
                {
                    var key = content.ReadString();
                    var text = content.ReadString();
                    loader.LoadLocalization(key, text);
                }
                else if (type == 4) // wav audioClip
                {
                    var key = content.ReadString();
                    var audioClip = content.ReadByteArray();
                    loader.LoadAudioClip(key, new LazyAudioDataLoader(audioClip, LazyAudioDataLoader.Format.Wav));
                }
                else if (type == 5) // ogg audioClip
                {
                    var key = content.ReadString();
                    var audioClip = content.ReadByteArray();
                    loader.LoadAudioClip(key, new LazyAudioDataLoader(audioClip, LazyAudioDataLoader.Format.Ogg));
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ", Name);
        }

        private Stream ReadDataTillContent(FileStream file)
        {
            var obsolete = !TryReadHeader(file);
            //Debug.LogError(obsolete + "444444444");
            var content = UnpackContent();
            //
            if (!_containsHeader)
                LoadHeaderDataObsolete(content);
            else
                LoadHeaderData(content);

            //Debug.Log(content2.ReadString().Length +" first string of the file");
            /*if (!_containsHeader)
                LoadHeaderDataObsolete(content2);
            else
                LoadHeaderData(content2);*/

            return content;
        }

        private static bool TryReadHeader(FileStream file)
        {
            var position = file.Position;
            var header = file.ReadUInt32();
            //Debug.Log(header + " :19919191919191");
            if (header != _header)
            {
                file.Seek(position, SeekOrigin.Begin);
                return false;
            }

            return true;
        }

        private static Stream UnpackContent(string path = null)
        {
            //UnityEngine.Debug.LogError(file.Length);
            //@"E:\UnityProjects\LastHope\Astral-Horizon\Mods" + "/Event-Horizon-ES-Mod 0.15.4"
            //var encryptedStream = new Security.EncryptedReadStream(file, (int)(file.Length - file.Position)); //
            Stream encryptedStream;
            if (path == null)
                encryptedStream = EncryptionRemover.RemoveEncryption(_modPath, ref _containsHeader);
            else
                encryptedStream = EncryptionRemover.RemoveEncryption(path, ref _containsHeader);

            var zlibStream = new ZlibStream(encryptedStream, CompressionMode.Decompress);
            return zlibStream;
        }


        private void LoadHeaderData(Stream stream)
        {
            var formatId = stream.ReadInt32();
            Name = stream.ReadString();
            Id = stream.ReadString();
            //Debug.LogError("Name: " + Name);
            var major = stream.ReadInt32();
            var minor = stream.ReadInt32();

            Version = new Version(major, minor);
        }

        private void LoadHeaderDataObsolete(Stream stream)
        {
            Name = stream.ReadString();
            Id = stream.ReadString();

            Version = new Version(1, 0);
        }
    }

    public class LazyImageDataLoader : Model.IImageData
    {
        private byte[] _rawData;
        private Model.ImageData _imageData;

        public Sprite Sprite
        {
            get
            {
                if (_imageData == null)
                {
                    _imageData = new(_rawData);
                    _rawData = null;
                }

                return _imageData.Sprite;
            }
        }

        public LazyImageDataLoader(byte[] rawData)
        {
            _rawData = rawData;
        }
    }

    public class LazyAudioDataLoader : Model.IAudioClipData
    {
        public enum Format
        {
            Wav,
            Ogg,
        }

        private readonly Format _format;
        private byte[] _rawData;
        private Model.IAudioClipData _audioClipData;

        public AudioClip AudioClip
        {
            get
            {
                if (_audioClipData == null)
                {
                    switch (_format)
                    {
                        case Format.Wav:
                            _audioClipData = new Model.AudioClipData(_rawData);
                            break;
                        case Format.Ogg:
                            _audioClipData = Model.OggAudioClip.Create(_rawData);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    _rawData = null;
                }

                return _audioClipData.AudioClip;
            }
        }

        public LazyAudioDataLoader(byte[] rawData, Format format)
        {
            _rawData = rawData;
            _format = format;
        }
    }
}
