﻿using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GameDatabase.Storage
{
    public class FolderDatabaseStorage : IDataStorage
    {
        public FolderDatabaseStorage(string path)
        {
            var info = new DirectoryInfo(path);
            Name = info.Name;
            Id = info.Name;

            var modInfo = info.GetFiles(SignatureFileName, SearchOption.AllDirectories).FirstOrDefault();
            if (modInfo != null)
            {
                var data = File.ReadAllText(modInfo.FullName).Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2 && data[1].IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                {
                    Name = data[0];
                    Id = data[1];
                }
            }
            Debug.LogError(path);
            _path = path;

            if (TryFindDatabaseVersion(out var version))
                Version = version;
            else
                Version = new Version(1, 0);
        }

        private bool TryFindDatabaseVersion(out Version version)
        {
            var serializer = new NewtonJsonSerializer();
            var info = new DirectoryInfo(_path);
            foreach (var fileInfo in info.GetFiles("*.json", SearchOption.AllDirectories))
            {
                var data = File.ReadAllText(fileInfo.FullName);
                var settings = serializer.FromJson<Serializable.DatabaseSettingsSerializable>(data);
                if (settings.ItemType != Enums.ItemType.DatabaseSettings) continue;
                version = new Version(settings.DatabaseVersion, settings.DatabaseVersionMinor);
                return true;
            }

            version = new();
            return false;
        }

        public void LoadContent(IContentLoader loader)
        {
            var info = new DirectoryInfo(_path);
            var itemCount = 0;
            foreach (var fileInfo in info.GetFiles("*", SearchOption.AllDirectories))
            {
                var file = fileInfo.FullName;
                if (fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || 
                    fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || 
                    fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    loader.LoadImage(fileInfo.Name, new LazyImageFileLoader(file));
                }
                else if (fileInfo.Extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
                {
                    loader.LoadAudioClip(Path.GetFileNameWithoutExtension(file), new LazyAudioFileLoader(file, LazyAudioFileLoader.Format.Wav));
                }
                else if (fileInfo.Extension.Equals(".ogg", StringComparison.OrdinalIgnoreCase))
                {
                    loader.LoadAudioClip(Path.GetFileNameWithoutExtension(file), new LazyAudioFileLoader(file, LazyAudioFileLoader.Format.Ogg));
                }
                else if (fileInfo.Extension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    var xmlData = File.ReadAllText(file);
                    loader.LoadLocalization(Path.GetFileNameWithoutExtension(file), xmlData);
                }
                else if (fileInfo.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                {
                    var data = File.ReadAllText(file);
                    loader.LoadJson(file, data);
                    itemCount++;
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ", _path);
        }

        public string Name { get; }
        public string Id { get; }
        public bool IsEditable => true;
        public Version Version { get; }

        public void UpdateItem(string name, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (!name.StartsWith(_path))
                name = Path.Combine(_path, name);
            
            try
            {
                File.WriteAllText(name, content);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private readonly string _path;
        private const string SignatureFileName = "id";
    }

    public class LazyImageFileLoader : Model.IImageData
    {
        private readonly string _filename;
        private Model.ImageData _imageData;

        public Sprite Sprite
        {
            get
            {
                if (_imageData == null)
                {
                    try 
                    {
                        var rawData = File.ReadAllBytes(_filename);
                        _imageData = new(rawData);
                    }
                    catch (Exception e) 
                    {
                        Debug.LogException(e);
                        _imageData = Model.ImageData.Empty;
                    }
                }

                return _imageData.Sprite;
            }
        }

        public LazyImageFileLoader(string fliename)
        {
            _filename = fliename;
        }
    }

    public class LazyAudioFileLoader : Model.IAudioClipData
    {
        public enum Format
        {
            Wav,
            Ogg,
        }

        private readonly string _filename;
        private Model.IAudioClipData _audioClipData;
        private readonly Format _format;

        public AudioClip AudioClip
        {
            get
            {
                if (_audioClipData == null)
                {
                    Debug.Log($"Loading audio clip: {_filename}");

                    try
                    {
                        var rawData = File.ReadAllBytes(_filename);

                        switch (_format)
                        {
                            case Format.Wav:
                                _audioClipData = new Model.AudioClipData(rawData);
                                break;
                            case Format.Ogg:
                                _audioClipData = Model.OggAudioClip.Create(rawData);
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        _audioClipData = Model.AudioClipData.Empty;
                    }
                }

                return _audioClipData.AudioClip;
            }
        }

        public LazyAudioFileLoader(string fliename, Format format)
        {
            _filename = fliename;
            _format = format;
        }
    }
}
