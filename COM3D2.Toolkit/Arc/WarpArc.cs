using System.IO;
using COM3D2.Toolkit.Crypto;

namespace COM3D2.Toolkit.Arc
{
    // TODO: Make WarpArc inherit from WarcArc/BaseArc because WARP is just an encrypted WARC file
    public class WarpArc
    {
        private static readonly byte[] WarpNameKey =
                {0x57, 0x79, 0xB9, 0xEC, 0x53, 0xD8, 0x48, 0x9F, 0xA9, 0x13, 0x00, 0xC5, 0x03, 0xB3, 0x56, 0x96};

        public static MemoryStream DecryptWarp(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                stream.Position = 8;

                int encryptedHeaderLength = reader.ReadInt32();
                var encryptedWarpName = reader.ReadBytes(encryptedHeaderLength);
                string patchedArc = WarpEncryption.DecryptString(encryptedWarpName, WarpNameKey);

                // TODO: Load provide original WARC as a parameter

                byte[] warcStart;

                using (var br = new BinaryReader(File.OpenRead(patchedArc)))
                    warcStart = br.ReadBytes(2048);

                var key = WarpEncryption.ComputeWarcKey(warcStart);

                reader.ReadUInt64(); // Not used
                int encryptedWarcHeaderLength = reader.ReadInt32();
                var encryptedWarcHeader = reader.ReadBytes(encryptedWarcHeaderLength);

                var warcHeader = WarpEncryption.DecryptBytes(encryptedWarcHeader, key);

                var ms = new MemoryStream();
                ms.Write(warcHeader, 0, warcHeader.Length);

                var buffer = new byte[32768];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);

                ms.Position = 0;
                return ms;
            }
        }

        public static WarpArc Read(Stream stream)
        {
            var arc = new WarpArc();

            using (MemoryStream ms = DecryptWarp(stream))
            {
                //TODO: Try to create a WarcArc from the memory stream
            }

            return arc;
        }
    }
}