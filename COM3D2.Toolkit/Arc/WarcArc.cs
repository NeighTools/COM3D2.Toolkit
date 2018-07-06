using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CM3D2.Toolkit;
using CM3D2.Toolkit.Arc.Native;
using COM3D2.Toolkit.Arc.Files;

namespace COM3D2.Toolkit.Arc
{
	public class WarcArc
	{
		protected static readonly byte[] WarcHeader =
		{
			0x77, 0x61, 0x72, 0x63, // warc
			0xFF, 0xAA, 0x45, 0xF1, // ?
			0xE8, 0x03, 0x00, 0x00, // 1000
			0x04, 0x00, 0x00, 0x00, // 4
			0x02, 0x00, 0x00, 0x00, // 2
		};

		internal Func<Stream> StreamGen { get; set; }

		protected long FooterOffset { get; set; }

		protected long EndOfHeaderOffset { get; set; }
		
		public string Name { get; protected set; }

		public IEnumerable<ArcEntry> Entries { get; protected set; }



		public WarcArc(string filename) : this(() => File.OpenRead(filename))
		{
			
		}

		public WarcArc(byte[] data) : this(() => new MemoryStream(data))
		{
			
		}

		public WarcArc(Func<Stream> streamGen)
		{
			StreamGen = streamGen;

			var reader = new BinaryReader(streamGen());

			if (!reader.ReadBytes(20).ContentEqual(WarcHeader))
				throw new InvalidDataException("This is not a Warc archive.");

			LoadInternal(reader);
		}

		protected void LoadInternal(BinaryReader reader)
		{
			var stream = reader.BaseStream;

			FooterOffset = reader.ReadInt64();
			EndOfHeaderOffset = stream.Position;
			stream.Position += FooterOffset;

			byte[] utf8HashData = null;
            byte[] utf16HashData = null;
            byte[] utf16NameData = null;

            while (utf8HashData == null || utf16HashData == null || utf16NameData == null)
            {
                int blockType = reader.ReadInt32();
                long blockSize = reader.ReadInt64();
                switch (blockType)
                {
                    case 0:
                        if (utf16HashData != null)
                            throw new ArcException("Duplicate UTF16 Data Block");

                        utf16HashData = reader.ReadBytes((int)blockSize);
                        break;
                    case 1:
                        if (utf8HashData != null)
	                        throw new ArcException("Duplicate UTF8 Data Block");

                        utf8HashData = reader.ReadBytes((int)blockSize);
                        break;
                    case 3:
                        if (utf16NameData != null)
	                        throw new ArcException("Duplicate Name Data Block");
						
	                    var file = ArcFileEntry.Read(reader, reader.BaseStream.Position, this);

                        utf16NameData = file.GetDataStream().ToBytes();
                        break;
                    default:
                    {
	                    throw new ArcException("Unknown Footer Data Block");
                    }
                }
            }

            var utf8Footer = FileHashTable.Read(new MemoryStream(utf8HashData));
            var utf16Footer = FileHashTable.Read(new MemoryStream(utf16HashData));
            var nameLookup = NameTable.Read(new MemoryStream(utf16NameData));

			var utf8Flat = Extensions.Flatten(utf8Footer, table => table.SubdirEntries, table => table.FileEntries)
				.ToArray();
			var utf16Flat = Extensions.Flatten(utf16Footer, table => table.SubdirEntries, table => table.FileEntries)
				.ToArray();

			if (utf16Flat.Any(e16 => utf8Flat.First(e => e.Offset == e16.Offset).Hash != DataHasher.BaseHasher.GetHashUTF8(nameLookup[e16.Hash])))
				throw new ArcException("File Checksum Mismatch");


			var rootName = nameLookup[utf16Footer.ID];
			rootName = rootName.Substring(rootName.LastIndexOf(Path.DirectorySeparatorChar) + 1);
			Name = rootName;


			Queue<FileHashTable> tables = new Queue<FileHashTable>();
			tables.Enqueue(utf16Footer);

			List<ArcEntry> entries = new List<ArcEntry>();

			Entries = entries;

			while (tables.Count != 0)
			{
				FileHashTable table = tables.Dequeue();

				entries.AddRange(table.FileEntries.Select(x =>
				{
					var file = ArcFileEntry.Read(reader, x.Offset, this);
					file.Name = nameLookup[x.Hash];
					return (ArcEntry)file;
				}));
			}
		}
	}
}
