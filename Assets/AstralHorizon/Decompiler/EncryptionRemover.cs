using System.IO;

namespace GameDatabase.Storage
{
public static class EncryptionRemover
{
    public static Stream RemoveEncryption(string modPath, ref bool containsHeader)
    {
        //FileStream debug = File.Create(saveTo);
        var fileE = File.ReadAllBytes(modPath);


        var fileEA =  RemoveLastByte(fileE);//fileE;
        UnityEngine.Debug.Log(fileEA.Length);
        byte[] fileU = null;
        if (ContainsHeader(fileE))
        {
            fileU = RemoveHeader(fileEA);
            containsHeader = true;
        }
        else
        {
            fileU = fileEA;
            containsHeader = false;
        }

        UnityEngine.Debug.Log(fileU.Length);
        var fileB = new byte[fileU.Length];
        uint num2 = 0x12345678 ^ (uint)fileU.Length;
		uint num3 = 0x87654321 ^ (uint)fileU.Length;
        
        for (int i = 0; i < fileU.Length; i++)
        {
            fileB[i] = (byte)(fileU[i] ^ (byte)random(ref num2, ref num3));
        }
        //from time to time might throw an error
        //var serializedData = ZlibStream.UncompressBuffer(fileB);//ZlibStream.UncompressBuffer(fileB);UncompressString(fileB);
        return new MemoryStream(fileB);
    }
    		private static uint random(ref uint w, ref uint z)
		{
			z = 36969U * (z & 65535U) + (z >> 16);
			w = 18000U * (w & 65535U) + (w >> 16);

			return (z << 16) + w;
		}
    public static byte[] RemoveHeader(byte[] fileE)
    {
        var output = new byte[fileE.Length - 4];
        for (int i = 4; i < fileE.Length; i++)
            output[i - 4] = fileE[i];

        return output;
    }
    private static bool ContainsHeader(byte[] fileEA)
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
    private static byte[] RemoveHeader()
    {
        return null;
    }
    private static byte[] RemoveLastByte(byte[] fileE)
    {
        var fileEA = new byte[fileE.Length - 1];
        //List<byte> fileEList = fileE.OfType<byte>().ToList();
        for (int i = 0; i < fileEA.Length; i++)
        {    fileEA[i] = fileE[i];
        }
        return fileEA;
    }
}
}
