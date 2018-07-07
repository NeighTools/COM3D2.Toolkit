using System;
using System.IO;
using COM3D2.Toolkit.Crypto;

namespace COM3D2.Toolkit.Arc
{
    public class WarpArc : WarcArc
    {
        private static readonly byte[] WarpNameKey =
                {0x57, 0x79, 0xB9, 0xEC, 0x53, 0xD8, 0x48, 0x9F, 0xA9, 0x13, 0x00, 0xC5, 0x03, 0xB3, 0x56, 0x96};

	    protected WarpArc()
	    {

	    }

        public static Stream DecryptWarp(Stream stream, Func<string, Stream> warcFunc)
        {
	        var reader = new BinaryReader(stream);

            stream.Position = 8;

            int encryptedHeaderLength = reader.ReadInt32();
            var encryptedWarpName = reader.ReadBytes(encryptedHeaderLength);
            string patchedArc = WarpEncryption.DecryptString(encryptedWarpName, WarpNameKey);

            byte[] warcStart;

            using (var br = new BinaryReader(warcFunc(patchedArc)))
                warcStart = br.ReadBytes(2048);

            var key = WarpEncryption.ComputeWarcKey(warcStart);

            reader.ReadUInt64(); // Not used
            int encryptedWarcHeaderLength = reader.ReadInt32();
            var encryptedWarcHeader = reader.ReadBytes(encryptedWarcHeaderLength);

            var warcHeader = WarpEncryption.DecryptBytes(encryptedWarcHeader, key);

            var ms = new MemoryStream(warcHeader);
                
			return new ChainedStream(ms, stream);
        }

	    public WarpArc(string filename) : this(() => File.OpenRead(filename), (s) => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), s)))
	    {
			
	    }

	    public WarpArc(string filename, Func<string, Stream> warcFunc) : this(() => File.OpenRead(filename), warcFunc)
	    {
			
	    }

	    public WarpArc(byte[] data, Func<string, Stream> warcFunc) : this(() => new MemoryStream(data), warcFunc)
	    {
			
	    }

	    public WarpArc(Func<Stream> streamGen, Func<string, Stream> warcFunc)
	    {
		    StreamGen = () => DecryptWarp(streamGen(), warcFunc);
			
			using (var reader = new BinaryReader(StreamGen()))
			{
				//if (!reader.ReadBytes(20).ContentEqual(WarcHeader))
				//	throw new InvalidDataException("This is not a Warc archive.");
				reader.ReadBytes(20); //ignore the first 20 bytes, it's not the same warc specification

				LoadInternal(reader);
			}
	    }
    }
}