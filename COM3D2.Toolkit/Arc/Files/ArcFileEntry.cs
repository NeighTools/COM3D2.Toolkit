using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using COM3D2.Toolkit.Utils;

namespace COM3D2.Toolkit.Arc.Files
{
	public class ArcFileEntry : ArcEntry
	{
		public long Offset { get; protected set; }

		public uint CompressedSize { get; protected set; }

		public uint UncompressedSize { get; protected set; }

		public bool Deflated { get; protected set; }

		internal static ArcFileEntry Read(BinaryReader reader, long offset, WarcArc arc, ArcEntry parent = null, IEnumerable<ArcEntry> children = null)
		{
			ArcFileEntry file = new ArcFileEntry
			{
				Archive = arc,
				Parent = parent,
				Children = children
			};

			reader.BaseStream.Position = offset;
			file.Deflated = reader.ReadUInt32() == 1;
			reader.ReadUInt32(); // Junk Data?
			file.UncompressedSize = reader.ReadUInt32();
			file.CompressedSize = reader.ReadUInt32();
			file.Offset = reader.BaseStream.Position;

			return file;
		}

		public Stream GetDataStream()
		{
			if (!Deflated)
				return new PartialStream(Archive.StreamGen(), Offset, CompressedSize);
			else
			{
				using (var baseStream = new PartialStream(Archive.StreamGen(), Offset + 2, CompressedSize - 2))
				using (var deflatedStream = new DeflateStream(baseStream, CompressionMode.Decompress, false))
				{
					MemoryStream mem = new MemoryStream();
					deflatedStream.CopyTo(mem);
					mem.Position = 0;
					return mem;
				}
			}
		}
	}
}
