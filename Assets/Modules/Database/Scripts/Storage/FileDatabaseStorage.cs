﻿using System;
using System.IO;
using Ionic.Zlib;
using UnityEngine;

namespace GameDatabase.Storage
{
    public class FileDatabaseStorage : IDataStorage
    {
        public FileDatabaseStorage(string filename)
        {
            var data = File.ReadAllBytes(filename);

            if (!Security.Encryption.TryDecrypt(data))
                throw new Exception("FileDatabaseStorage: invalid file format");

            _content = ZlibStream.UncompressBuffer(data);

            var index = 0;
            Name = Utils.Serialization.DeserializeString(_content, ref index);
            Id = Utils.Serialization.DeserializeString(_content, ref index);

            SchemaVersion = 1;
            if (_content[index] == 0)
            {
                index++;
                SchemaVersion = Utils.Serialization.DeserializeInt(_content, ref index);
            }

            _startIndex = index;
        }

        public string Name { get; }
        public string Id { get; }
        public int SchemaVersion { get; }
        public bool IsEditable => false;

        public void UpdateItem(string id, string content)
        {
            throw new InvalidOperationException("FileDatabaseStorage.UpdateItem is not supported");
        }

        public void LoadContent(IContentLoader loader)
        {
            var index = _startIndex;
            var itemCount = 0;
            while (_content[index] != 0)
            {
                var type = _content[index++];

                if (type == 1) // json
                {
                    var fileContent = Utils.Serialization.DeserializeString(_content, ref index);
                    loader.LoadJson(string.Empty, fileContent);
                    itemCount++;
                }
                else if (type == 2) // image
                {
                    var key = Utils.Serialization.DeserializeString(_content, ref index);
                    var image = Utils.Serialization.DeserializeByteArray(_content, ref index);
                    loader.LoadImage(key, new LazyImageDataLoader(image));
                }
                else if (type == 3) // localization
                {
                    var key = Utils.Serialization.DeserializeString(_content, ref index);
                    var text = Utils.Serialization.DeserializeString(_content, ref index);
                    loader.LoadLocalization(key, text);
                }
                else if (type == 4) // wav audioClip
                {
                    var key = Utils.Serialization.DeserializeString(_content, ref index);
                    var audioClip = Utils.Serialization.DeserializeByteArray(_content, ref index);
                    loader.LoadAudioClip(key, new LazyAudioDataLoader(audioClip));
                }
            }

            if (itemCount == 0)
                throw new FileNotFoundException("Invalid database - ", Name);
        }

        private readonly byte[] _content;
        private readonly int _startIndex;
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
        private byte[] _rawData;
        private Model.AudioClipData _audioClipData;

        public AudioClip AudioClip
        {
            get
            {
                if (_audioClipData == null)
                {
                    _audioClipData = new(_rawData);
                    _rawData = null;
                }

                return _audioClipData.AudioClip;
            }
        }

        public LazyAudioDataLoader(byte[] rawData)
        {
            _rawData = rawData;
        }
    }
}
