using System;
using System.IO;
using System.Text;
using COM3D2.Toolkit.Native;

namespace COM3D2.Toolkit.Arc
{
    public class WarpArc : WarcArc
    {
        private static readonly byte[] WarpNameKey =
                {0x57, 0x79, 0xB9, 0xEC, 0x53, 0xD8, 0x48, 0x9F, 0xA9, 0x13, 0x00, 0xC5, 0x03, 0xB3, 0x56, 0x96};

        public WarpArc(string filename) : this(() => File.OpenRead(filename),
                                               s => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), s))) { }

        public WarpArc(string filename, Func<string, Stream> warcFunc) : this(() => File.OpenRead(filename), warcFunc) { }

        public WarpArc(byte[] data, Func<string, Stream> warcFunc) : this(() => new MemoryStream(data), warcFunc) { }

        public WarpArc(byte[] data, byte[] decryptionSeed) : this(() => new MemoryStream(data), _ => new MemoryStream(decryptionSeed)) { }

        public WarpArc(Func<Stream> streamGen, Func<string, Stream> warcFunc)
        {
            StreamGen = () => DecryptWarp(streamGen(), warcFunc);

            using (var reader = new BinaryReader(StreamGen()))
            {
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "warc")
                    throw new ArcException("Could not decrypt WARP file.");

                LoadInternal(reader);
            }
        }

        protected WarpArc() { }

        public static Stream DecryptWarp(Stream stream, Func<string, Stream> warcFunc)
        {
            var reader = new BinaryReader(stream);

            stream.Position = 8;

            int encryptedHeaderLength = reader.ReadInt32();
            byte[] encryptedWarpName = reader.ReadBytes(encryptedHeaderLength);
            string patchedArc = WarpEncryption.DecryptString(encryptedWarpName, WarpNameKey);

            byte[] warcStart;

            using (var br = new BinaryReader(warcFunc(patchedArc)))
                warcStart = br.ReadBytes(2048);

            byte[] key = WarpEncryption.ComputeWarcKey(warcStart);

            reader.ReadUInt64(); // Not used
            int encryptedWarcHeaderLength = reader.ReadInt32();
            byte[] encryptedWarcHeader = reader.ReadBytes(encryptedWarcHeaderLength);

            byte[] warcHeader = WarpEncryption.DecryptBytes(encryptedWarcHeader, key);

            var ms = new MemoryStream(warcHeader);

            return new ChainedStream(ms, stream);
        }
    }
}