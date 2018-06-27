using System.IO;
using COM3D2.Toolkit.Crypto;

namespace COM3D2.Toolkit.Arc
{
    public class WarpArc
    {
        private static readonly byte[] WarpNameKey =
                {0x57, 0x79, 0xB9, 0xEC, 0x53, 0xD8, 0x48, 0x9F, 0xA9, 0x13, 0x00, 0xC5, 0x03, 0xB3, 0x56, 0x96};

        public string PatchedArc { get; protected set; }

        public static WarpArc Read(Stream stream)
        {
            var arc = new WarpArc();

            using (var reader = new BinaryReader(stream))
            {
                stream.Position = 8;

                var encryptedHeaderLength = reader.ReadInt32();
                var encryptedWarpName = reader.ReadBytes(encryptedHeaderLength);
                arc.PatchedArc = WarpEncryption.DecryptString(encryptedWarpName, WarpNameKey);
            }

            return arc;
        }
    }
}