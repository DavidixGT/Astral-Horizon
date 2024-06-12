using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Ionic.Zlib;
using System.Text;
using System.Diagnostics;

public class Decrypt : MonoBehaviour
{
    
    private readonly int _versionMinor = 6;
    private readonly int _versionMajor = 1;
    private string _datapath;
    private string _name;
    private string _id;
    private const uint _header = 0xDA7ABA5E;
    private const int _version = 1;
    public const string SignatureFileName = "id";
            public string Name { get; private set; }
        public string Id { get; private set; }
       // public GameDatabase.Storage.Version Version { get; private set; }
    public string _modName;
    public bool _unpackImages;
    private void DecompileImages(string modSourcePath)
    {
        byte[] array = File.ReadAllBytes(modSourcePath);
        byte[] startSequence = new byte[]{137, 80, 78, 71};
        byte[] endSequence = new byte[]{73, 69, 78, 68, 174, 66, 96, 130};
        var index = 0;
        var imageCount = 1;
        while (true)
        {
            
            
            var start = FindSequence(index, array, startSequence);
            var end = FindSequence(index, array,endSequence);
            if (start[0] == -1 || start[0] == -1)
                break;

            var img = new byte[end[1] - start[0] + 1];
            for (int i = start[0]; i < end[1]; i++)
            {
                img[i - start[0]] = array[i];
            }
            index = end[1];
            //UnityEngine.Debug.Log(index);
            CreateImage(img, imageCount);
            imageCount++;
            //Thread.Sleep(10);
        }
    }
    private void CreateImage(byte[] img,int index)
    {
        FileStream debug = File.Create(@"E:\programs\EHeditor\debug\ModImages\Image" + index.ToString() + ".png");
                    debug.Write(img);
            debug.Flush();
    }
    private void Debugging2()
    {
        //fileE(encrypted) would have one byte more than just compressed file
        var fileC = new byte[5588];
        fileC[fileC.Length - 1] = (byte)255;
        var fileU = ZlibStream.UncompressBuffer(fileC);
        FileStream debug = File.Create("E:\\programs\\EHeditor\\debug\\debug2");
        debug.Write(fileU);
    }
    private void Debugging3()
    {
        //var bb = 0xAE;‰PNG
        foreach (var c in Encoding.UTF8.GetBytes("NG"))
        UnityEngine.Debug.Log(c);

        UnityEngine.Debug.Log("kabaki: ");
        foreach (var c in Encoding.UTF8.GetBytes("����"))
        UnityEngine.Debug.Log(c);

        //UnityEngine.Debug.Log("byte: " + BinaryToByte("00001000"));
    }
public int[] FindSequence(int start, byte[] array, byte[] sequence)
{
    int startIndex = -1;
    int endIndex = -1;

    for (int i = start; i < array.Length; i++)
    {
        //UnityEngine.Debug.Log(i + " a: " + array.Length);
        if (array[i] == sequence[0])
        {
            bool isMatch = true;

            for (int j = 1; j < sequence.Length; j++)
            {
                if (i + j >= array.Length || array[i + j] != sequence[j])
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                startIndex = i;
                endIndex = i + sequence.Length - 1;
                break;
            }
        }
    }

    return new int[] { startIndex, endIndex };
}
    private byte BinaryToByte(string binary)
    {
        int numOfBytes = binary.Length / 8;
        byte[] bytes = new byte[numOfBytes];
        for(int i = 0; i < numOfBytes; ++i)
        {
            bytes[i] = Convert.ToByte(binary.Substring(8 * i, 8), 2);
        }
        return bytes[0];
    }
    private void Decompile1(string modPath, string saveTo)
    {
        FileStream debug = File.Create(saveTo);
        var fileE = File.ReadAllBytes(modPath);


        var fileEA =  RemoveLastByte(fileE);//fileE;
        UnityEngine.Debug.Log(fileEA.Length);
        byte[] fileU = null;
        if (ContainsHeader(fileE))
            fileU = RemoveHeader(fileEA);
        else
            fileU = fileEA;
            

        UnityEngine.Debug.Log(fileU.Length);
        var fileB = new byte[fileU.Length];
        uint num2 = 0x12345678 ^ (uint)fileU.Length;
		uint num3 = 0x87654321 ^ (uint)fileU.Length;
        
        for (int i = 0; i < fileU.Length; i++)
        {
            fileB[i] = (byte)(fileU[i] ^ (byte)random(ref num2, ref num3));
        }
        //from time to time might throw an error
        var serializedData = ZlibStream.UncompressBuffer(fileB);//ZlibStream.UncompressBuffer(fileB);UncompressString(fileB);
        debug.Write(serializedData);
        debug.Close();
    }
    private byte[] RemoveHeader(byte[] fileE)
    {
        var output = new byte[fileE.Length - 4];
        for (int i = 4; i < fileE.Length; i++)
            output[i - 4] = fileE[i];

        return output;
    }
    private bool ContainsHeader(byte[] fileEA)
    {
        byte[] header = new byte[]{94, 186, 122, 218};
        int checkSum = 0;
        for (int i = 0; i < 4; i++)
            if (fileEA[i] == header[i])
                checkSum++;

        if (checkSum == 0)
            return false;
        else
            return true;
    }
    private byte[] RemoveHeader()
    {
        return null;
    }
    private byte[] RemoveLastByte(byte[] fileE)
    {
        var fileEA = new byte[fileE.Length - 1];
        for (int i = 0; i < fileEA.Length; i++)
        {    fileEA[i] = fileE[i];
        }
        return fileEA;
    }

		public static string UncompressString(byte[] compressed)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream(compressed))
			{
				Stream decompressor = new ZlibStream(memoryStream, CompressionMode.Decompress);
				result = UncompressString(compressed, decompressor);
			}
			return result;
		}


    	public static string UncompressString(byte[] compressed, Stream decompressor)
		{
			byte[] array = new byte[1024];
			Encoding utf = Encoding.UTF8;
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					int count;
					while ((count = decompressor.Read(array, 0, array.Length)) != 0)
					{
						memoryStream.Write(array, 0, count);
					}
				}
				finally
				{
					if (decompressor != null)
					{
						((IDisposable)decompressor).Dispose();
					}
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				result = new StreamReader(memoryStream, utf).ReadToEnd();
			}
			return result;
		}     
    void Start()
    {
        //1A223DEC-A768-1CFB-356C-43D69DFD6BA3
        //31628d50-3eec-45a9-84e2-6387f36d8741
        //E:\programs\EHeditor\test1\Database
        //E:\programs\EHeditor\Database

        string databasepath = @"E:\programs\EHeditor\test1\Database";
        string modPath = @"E:\programs\EHeditor\test1\ModSave\unity2";
        //Create(databasepath, modPath);


        //DecompileImages();
        //Debugging2();
        //Debugging3();
        string modSourceSavePath = @"E:\programs\EHeditor\debug\ModMiscFiles\" + _modName;
        Decompile1(@"E:\programs\EHeditor\debug\Mods\" + _modName, modSourceSavePath);
        //Thread.Sleep(1000);
        if (_unpackImages)
        DecompileImages(modSourceSavePath);
        //Decompile();
        //var data = ZlibStream.CompressBuffer()
    }

    private void Create(string databasepath, string modPath)
    {
        string name, guid;
        if (!TryReadSignature(databasepath, out var name1, out var guid1))
        {
            UnityEngine.Debug.Log("no guid");
            guid1 = Guid.NewGuid().ToString();
        }
        if (TryReadSignature(databasepath, out name, out guid))
        {
            _datapath = databasepath;
            _name = name;
            //UnityEngine.Debug.Log(guid);
            //_id = guid;
            //_id = "1A223DEC-A768-1CFB-356C-43D69DFD6BA3";
            _id = guid;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Build(File.Create(modPath));
            sw.Stop();
            TimeSpan elapsedTime = sw.Elapsed;
            UnityEngine.Debug.Log(elapsedTime.Milliseconds);
        }

    }
		public void Build(FileStream stream)
		{
			try
			{
				byte[] array = ZlibStream.CompressBuffer(this.SerializeData().ToArray<byte>().ToArray<byte>());//CompressBuffer(this.SerializeData().ToArray<byte>().ToArray<byte>());
				uint num = (uint)array.Length;
				byte b = 0;
				uint num2 = 305419896U ^ num;
				uint num3 = 2271560481U ^ num;
				int num4 = 0;
                
                UnityEngine.Debug.Log("uncompressed: " + num + " compressed: " + this.SerializeData().ToArray<byte>().Length + " compressed: " + this.SerializeData().ToArray<byte>().Length);
				while ((long)num4 < (long)((ulong)num))
				{
					b += array[num4];
					array[num4] ^= (byte)random(ref num2, ref num3);
					num4++;
				}
                var debugArray = this.SerializeData().ToArray<byte>();
                UnityEngine.Debug.Log("E length: " + array.Length);
                DebugTotalSum(debugArray, array);
                FileStream debug = File.Create("E:\\programs\\EHeditor\\debug\\log16");
                debug.Write(ZlibStream.CompressBuffer(debugArray));
                
				stream.Write(array, 0, array.Length);
                var nd1 = num2;
                var nd2 = num3;
                var nd4 = num2;
                var nd5 = num3;
				stream.WriteByte((byte)(b ^ (byte)random(ref num2, ref num3)));
                stream.Close();
                var nd3 = (byte)random(ref nd1, ref nd2);
                var k = (byte)(b ^ nd3);
                        //var file = File.ReadAllBytes(@"E:\programs\EHeditor\test1\ModSave\unity");
                //UnityEngine.Debug.Log("final byte in encrypted file unity: " + "   :  " + k + " check sum: " + b + " to the power of power: " + nd3 + " num2: " + nd4 + " num3: " + nd5);
                byte jj = 255;
                //UnityEngine.Debug.Log((byte)((byte)jj + (byte)10));
			}
			finally
			{
				stream.Close();
			}
		}
    /*public void Build(FileStream stream)
        {
            try
            {
                var header = SerializeHeader().ToArray();
                var rawdata = SerializeData().ToArray();// 1 and 2 serializers
                var data = ZlibStream.CompressBuffer(rawdata.ToArray());

                var size = (uint)data.Length;
                byte checksumm = 0;
                uint w = 0x12345678u ^ size;
                uint z = 0x87654321u ^ size;
                for (int i = 0; i < size; ++i)
                {
                    checksumm += data[i];
                    data[i] = (byte)(data[i] ^ (byte)random(ref w, ref z));
                }

                stream.Write(header, 0, header.Length);
                stream.Write(data, 0, data.Length);
                stream.WriteByte((byte)(checksumm ^ (byte)random(ref w, ref z)));
            }
            finally
            {
                stream.Close();
            }
        }*/
                private static uint random2(ref uint w, ref uint z)
		{
			z = 36969U * (z & 65535U) + (z >> 16);
			w = 18000U * (w & 65535U) + (w >> 16);

			return (z << 16) + w;
		}
        public void Decompile()
        {
            var file = File.ReadAllBytes(@"E:\programs\EHeditor\debug\log1-2");
            var uncompressed = ZlibStream.UncompressBuffer(file);
            var debug = File.Create("E:\\programs\\EHeditor\\debug\\log53");
            debug.Write(uncompressed);
        }
        private void DebugTotalSum(byte[] compressedA, byte[] encryptedA)
        {
            byte i = 0;
            byte j = 0;
            foreach (byte b in compressedA)
                i += b;

            foreach (byte b in encryptedA)
                j += b;

            //UnityEngine.Debug.Log("Compressed: " + i + " Encrypted: " + j);
            //UnityEngine.Debug.Log("CompressedLength: " + compressedA.Length + " EncryptedLength: " + encryptedA.Length);
        }
    /*public void Build(FileStream stream)
	{
		try
		{
			byte[] array = ZlibStream.CompressBuffer(SerializeData().ToArray().ToArray());
			uint num = (uint)array.Length;
			byte b = 0;
			uint w = 0x12345678u ^ num;
			uint z = 0x87654321u ^ num;
            var debug = File.Create(@"E:\programs\EHeditor\debug\log");
            var debugArray = new byte[num];
			for (int i = 0; i < num; i++)
			{
				b += array[i];
                debugArray[i] = (byte)random(ref w, ref z);
				array[i] ^= (byte)random(ref w, ref z);
                
			}
            debug.Write(debugArray);

			stream.Write(array, 0, array.Length);
			stream.WriteByte((byte)(b ^ (byte)random(ref w, ref z)));
		}
		finally
		{
			stream.Close();
		}
	}*/
    public static bool TryReadSignature(string path, out string name, out string guid)
    {
        name = string.Empty;
        guid = string.Empty;
        try
        {
            var id = new DirectoryInfo(path).GetFiles(SignatureFileName).FirstOrDefault();
            if (id == null)
                return false;
            var data = File.ReadAllLines(id.FullName);
            if (data.Length < 2)
                return false;
            name = data[0];
            ////UnityEngine.Debug.Log(name);
            guid = data[1];
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(guid))
                return false;
            if (guid.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    /*private IEnumerable<byte> SerializeHeader()
    {
        return Serialize(unchecked((int)_header));
    }*/
    /*private IEnumerable<byte> SerializeData()
    {
        foreach (var value in Serialize(_version))
            yield return value;
        foreach (var value in Serialize(_name))
            yield return value;
        foreach (var value in Serialize(_id))
            yield return value;
        foreach (var value in Serialize(_versionMajor))
            yield return value;
        foreach (var value in Serialize(_versionMinor))
            yield return value;
        foreach (var file in new DirectoryInfo(_datapath).EnumerateFiles("*", SearchOption.AllDirectories))
        {
            var ext = file.Extension.ToLower();
            if (ext == ".json")
            {
                yield return (byte)FileType.Data;
                foreach (var value in SerializeTextFile(file.FullName))
                    yield return value;
            }
            else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                yield return (byte)FileType.Image;
                foreach (var value in Serialize(file.Name))
                    yield return value;
                foreach (var value in SerializeBinaryFile(file.FullName))
                    yield return value;
            }
            else if (ext == ".wav")
            {
                yield return (byte)FileType.WaveAudio;
                foreach (var value in Serialize(Path.GetFileNameWithoutExtension(file.Name)))
                    yield return value;
                foreach (var value in SerializeBinaryFile(file.FullName))
                    yield return value;
            }
            else if (ext == ".ogg")
            {
                yield return (byte)FileType.OggAudio;
                foreach (var value in Serialize(Path.GetFileNameWithoutExtension(file.Name)))
                    yield return value;
                foreach (var value in SerializeBinaryFile(file.FullName))
                    yield return value;
            }
            else if (ext == ".xml")
            {
                yield return (byte)FileType.Localization;
                foreach (var value in Serialize(Path.GetFileNameWithoutExtension(file.Name)))
                    yield return value;
                foreach (var value in SerializeTextFile(file.FullName))
                    yield return value;
            }
            else
            {
                continue;
            }
        }

        yield return (byte)FileType.None;
    }*/
    
private IEnumerable<byte> SerializeData()
		{
			foreach (byte b in Serialize(this._name))
			{
				yield return b;
			}
			IEnumerator<byte> enumerator = null;
			foreach (byte b2 in Serialize(this._id))
			{
				yield return b2;
			}
			enumerator = null;
			foreach (FileInfo file in new DirectoryInfo(this._datapath).EnumerateFiles("*", SearchOption.AllDirectories))
			{
				string a = file.Extension.ToLower();
				if (a == ".json")
				{
					yield return 1;
					foreach (byte b3 in this.SerializeTextFile(file.FullName))
					{
						yield return b3;
					}
					enumerator = null;
				}
				else if (a == ".png" || a == ".jpg" || a == ".jpeg")
				{
					yield return 2;
					foreach (byte b4 in Serialize(file.Name))
					{
						yield return b4;
					}
					enumerator = null;
					foreach (byte b5 in this.SerializeBinaryFile(file.FullName))
					{
						yield return b5;
					}
					enumerator = null;
				}
				else if (a == ".wav")
				{
					yield return 4;
					foreach (byte b6 in Serialize(Path.GetFileNameWithoutExtension(file.Name)))
					{
						yield return b6;
					}
					enumerator = null;
					foreach (byte b7 in this.SerializeBinaryFile(file.FullName))
					{
						yield return b7;
					}
					enumerator = null;
				}
				else
				{
					if (!(a == ".xml"))
					{
						continue;
					}
					yield return 3;
					foreach (byte b8 in Serialize(Path.GetFileNameWithoutExtension(file.Name)))
					{
						yield return b8;
					}
					enumerator = null;
					foreach (byte b9 in this.SerializeTextFile(file.FullName))
					{
						yield return b9;
					}
					enumerator = null;
				}
			}
			IEnumerator<FileInfo> enumerator2 = null;
			yield return 0;
			yield break;
			yield break;
		}

		private IEnumerable<byte> SerializeBinaryFile(string name)
		{
			byte[] fileData = File.ReadAllBytes(name);
			foreach (byte b in BitConverter.GetBytes(fileData.Length))
			{
				yield return b;
			}
			byte[] array = null;
			foreach (byte b2 in fileData)
			{
				yield return b2;
			}
			array = null;
			yield break;
		}


        /*private IEnumerable<byte> SerializeBinaryFile(string name)
        {
            var fileData = File.ReadAllBytes(name);
            foreach (var value in BitConverter.GetBytes(fileData.Length))
                yield return value;
            foreach (var value in fileData)
                yield return value;
        }*/

        /*private IEnumerable<byte> SerializeTextFile(string name)
        {
            var fileData = File.ReadAllText(name);
            return Serialize(fileData);
        }*/

        /*private static IEnumerable<byte> Serialize(int data)
        {
            yield return (byte)(data);
            yield return (byte)(data >> 8);
            yield return (byte)(data >> 16);
            yield return (byte)(data >> 24);
        }*/
        /*private static IEnumerable<byte> Serialize(string data)
	{
		byte[] bytes;
		if (string.IsNullOrEmpty(data))
		{
			bytes = BitConverter.GetBytes(0);
			for (int i = 0; i < bytes.Length; i++)
			{
				yield return bytes[i];
			}
			yield break;
		}
		byte[] bytes2 = Encoding.UTF8.GetBytes(data);
		bytes = BitConverter.GetBytes(bytes2.Length);
		for (int i = 0; i < bytes.Length; i++)
		{
			yield return bytes[i];
		}
		bytes = Encoding.UTF8.GetBytes(data);
		for (int i = 0; i < bytes.Length; i++)
		{
			yield return bytes[i];
		}
	}*/

        /*private static IEnumerable<byte> Serialize(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                foreach (var value in BitConverter.GetBytes(0))
                    yield return value;
                yield break;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(data);

            foreach (var value in BitConverter.GetBytes(bytes.Length))
                yield return value;
            foreach (var value in System.Text.Encoding.UTF8.GetBytes(data))
                yield return value;
        }
        private static uint random(ref uint w, ref uint z)
        {
            z = 36969 * (z & 0xFFFF) + (z >> 16);
            w = 18000 * (w & 0xFFFF) + (w >> 16);
            return (z << 16) + w;  /* 32-bit result 
        }*/

        private IEnumerable<byte> SerializeTextFile(string name)
		{
			return Serialize(File.ReadAllText(name));
		}

		// Token: 0x06000312 RID: 786 RVA: 0x000133D9 File Offset: 0x000115D9
		private static IEnumerable<byte> Serialize(string data)
		{
			byte[] array;
			if (string.IsNullOrEmpty(data))
			{
				foreach (byte b in BitConverter.GetBytes(0))
				{
					yield return b;
				}
				array = null;
				yield break;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			foreach (byte b2 in BitConverter.GetBytes(bytes.Length))
			{
				yield return b2;
			}
			array = null;
			foreach (byte b3 in Encoding.UTF8.GetBytes(data))
			{
				yield return b3;
			}
			array = null;
			yield break;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000133E9 File Offset: 0x000115E9
		private static uint random(ref uint w, ref uint z)
		{
			z = 36969U * (z & 65535U) + (z >> 16);
			w = 18000U * (w & 65535U) + (w >> 16);

			return (z << 16) + w;
		}

        public enum FileType : byte
        {
            None = 0,
            Data = 1,
            Image = 2,
            Localization = 3,
            WaveAudio = 4,
            OggAudio = 5,
        }
}
