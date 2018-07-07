using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using COM3D2.Toolkit.Arc;
using COM3D2.Toolkit.Native;

namespace COM3D2.Toolkit.Tests.Crypto
{
	[TestClass]
	public class WarpEncryptionTests
	{
		private readonly byte[] testHeader =
			{0xBF, 0x14, 0x81, 0xA2, 0x99, 0x3C, 0x41, 0x37, 0xF0, 0x82, 0xD5, 0xA5, 0xE9, 0xF3, 0x1F, 0x07, 0x7F, 0xE1, 0x31, 0x5D, 0xFA, 0xD7, 0xE8, 0x34, 0x08, 0xBA, 0x38, 0xEB, 0x2E, 0xA3, 0x3A, 0x7C, 0x3A, 0x35, 0x22, 0xDC, 0xFF};


		private readonly byte[] dfjd = {0xE4, 0xD4, 0xF1, 0x5E, 0x2C, 0x53, 0x9F, 0xAC, 0xA7, 0xA3, 0x8F, 0x1E, 0x45};

		[TestMethod]
		public void GenerateIVTest()
		{
			//byte[] testData = {0x00, 0x40, 0x63, 0x9D, 0x8A};

			byte[] ivSeed = new byte[5];

			Array.Copy(testHeader, testHeader.Length - 5, ivSeed, 0, 5);

			byte[] output = WarpEncryption.GenerateIV(ivSeed);

			string o = BitConverter.ToString(output);
		}

        [TestMethod]
	    public void GenerateEncKeyTest()
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(File.OpenRead("model_dlc005.arc")))
                bytes = br.ReadBytes(2048);

            byte[] key = WarpEncryption.ComputeWarcKey(bytes);

            string o = BitConverter.ToString(key);
        }

        [TestMethod]
	    public void DecryptWarpToWarcTest()
        {
            using (Stream s = WarpArc.DecryptWarp(File.OpenRead("model_dlc005_2.arc"), File.OpenRead))
                using (Stream sw = File.Create("model_dlc005_2_decrypted.arc"))
                {
                    var buffer = new byte[32768];
                    int len;
                    while((len = s.Read(buffer, 0, buffer.Length)) > 0)
                        sw.Write(buffer, 0, len);
                }
        }

        [TestMethod]
	    public void OpenWarpTest()
	    {
            WarpArc warp = new WarpArc("model_dlc005_2.arc");

            Console.Write($"Got {warp.Entries.Count()} entries");
	    }

		[TestMethod]
		public void DecryptBytesTest()
		{
			//WarpArc arc = WarpArc.Read(File.OpenRead(@"M:\COM3D2\GameData_20\model_denkigai2015s003_2.arc"));

			//var enc = new WarpEncryption(testHeader);

			//var s = enc.DecryptString(dfjd);

			//byte[] filenameBytes = new byte[testHeader.Length - 5];

			//Array.Copy(testHeader, 0, filenameBytes, 0, testHeader.Length - 5);

			//string output = Encoding.ASCII.GetString(enc.DecryptBytes(filenameBytes)).TrimEnd('\0');
		}
	}
}