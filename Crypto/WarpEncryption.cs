using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace COM3D2.Toolkit.Crypto
{
	public class WarpEncryption
	{
		public static byte[] GenerateIV(byte[] iv_seed)
		{
			uint[] seed =
			{
				0x075BCD15,
				0x159A55E5,
				0x1F123BB5,
				BitConverter.ToUInt32(iv_seed, 1) ^ 0xBFBFBFBF
			};

			for (int i = 0; i < 4; i++)
			{
				uint n = seed[0] ^ (seed[0] << 11);
				seed[0] = seed[1];
				seed[1] = seed[2];
				seed[2] = seed[3];
				seed[3] = n ^ seed[3] ^ (n ^ (seed[3] >> 11)) >> 8;
			}
            
			byte[] output = new byte[16];

			Buffer.BlockCopy(seed, 0, output, 0, 16);

			return output;
		}

		public static readonly byte[] Key = { 0x57, 0x79, 0xB9, 0xEC, 0x53, 0xD8, 0x48, 0x9F, 0xA9, 0x13, 0x00, 0xC5, 0x03, 0xB3, 0x56, 0x96 };

		public readonly byte[] IV;

		protected byte[] IVSeed;

		protected int BlockSize;

		public WarpEncryption(byte[] header)
		{
			IVSeed = new byte[5];

			Array.Copy(header, header.Length - 5, IVSeed, 0, 5);

			IV = GenerateIV(IVSeed);

			BlockSize = IVSeed[0] ^ IVSeed[1];
		}

		public byte[] DecryptBytes(byte[] input)
		{
			using (var rijndael = new RijndaelManaged())
			 {
				rijndael.Padding = PaddingMode.None;

				using (ICryptoTransform decryptor = rijndael.CreateDecryptor(Key, IV))
				using (var mem = new MemoryStream(input))
				using (var stream = new CryptoStream(mem, decryptor, CryptoStreamMode.Read))
				{
					//if (blocksize == 0)
					//don't care about blocksize?

					byte[] output = new byte[input.Length];

					{
						stream.Read(output, 0, input.Length);
					}

					return output;
				}
			}
		}

		private Encoding sjisEncoding = Encoding.GetEncoding(932);

		public string DecryptString(byte[] input)
		{
			return sjisEncoding.GetString(DecryptBytes(input)).TrimEnd('\0');
		}
	}
}
