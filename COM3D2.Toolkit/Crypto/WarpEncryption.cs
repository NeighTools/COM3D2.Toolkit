using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace COM3D2.Toolkit.Crypto
{
    public static class WarpEncryption
    {
        private static readonly Encoding ShiftJisEncoding = Encoding.GetEncoding(932);

        public static byte[] GenerateIV(byte[] ivSeed)
        {
            uint[] seed = {0x075BCD15, 0x159A55E5, 0x1F123BB5, BitConverter.ToUInt32(ivSeed, 1) ^ 0xBFBFBFBF};

            for (var i = 0; i < 4; i++)
            {
                var n = seed[0] ^ (seed[0] << 11);
                seed[1] = seed[2];
                seed[2] = seed[3];
                seed[3] = n ^ seed[3] ^ ((n ^ (seed[3] >> 11)) >> 8);
            }

            var output = new byte[16];

            Buffer.BlockCopy(seed, 0, output, 0, 16);

            return output;
        }

        public static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key)
        {
            var ivSeed = new byte[5];
            Array.Copy(encryptedBytes, encryptedBytes.Length - 5, ivSeed, 0, 5);

            var iv = GenerateIV(ivSeed);

            using (var rijndael = new RijndaelManaged())
            {
                rijndael.Padding = PaddingMode.None;

                using (var decryptor = rijndael.CreateDecryptor(key, iv))
                    using (var mem = new MemoryStream(encryptedBytes, 0, encryptedBytes.Length - 5))
                        using (var stream = new CryptoStream(mem, decryptor, CryptoStreamMode.Read))
                        {
                            //if (blocksize == 0)
                            //don't care about blocksize?

                            var output = new byte[encryptedBytes.Length - 5];

                            stream.Read(output, 0, output.Length);

                            return output;
                        }
            }
        }

        public static string DecryptString(byte[] encryptedBytes, byte[] key)
        {
            return ShiftJisEncoding.GetString(DecryptBytes(encryptedBytes, key)).TrimEnd('\0');
        }
    }
}