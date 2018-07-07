// --------------------------------------------------
// CM3D2.Toolkit - FileHashTable.cs
// --------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace COM3D2.Toolkit.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct FileHashTable
    {
        public long Header;
        public ulong ID;
        public int Dirs;
        public int Files;
        public int Depth;
        public int Junk;
        public FileEntry[] DirEntries;
        public FileEntry[] FileEntries;
        public ulong[] Parents;
        public FileHashTable[] SubdirEntries;

		public static FileHashTable Read(Stream s)
        {
            FileHashTable entry;
	        BinaryReader br = new BinaryReader(s);

            entry = new FileHashTable
            {
                Header = br.ReadInt64(),
                ID = br.ReadUInt64(),
                Dirs = br.ReadInt32(),
                Files = br.ReadInt32(),
                Depth = br.ReadInt32(),
                Junk = br.ReadInt32(),
            };
            entry.DirEntries = new FileEntry[entry.Dirs];
            for (var i = 0; i < entry.Dirs; i++)
            {
                entry.DirEntries[i] = new FileEntry
                {
                    Hash = br.ReadUInt64(),
                    Offset = br.ReadInt64()
                };
            }
            entry.FileEntries = new FileEntry[entry.Files];
            for (var i = 0; i < entry.Files; i++)
            {
                entry.FileEntries[i] = new FileEntry
                {
                    Hash = br.ReadUInt64(),
                    Offset = br.ReadInt64()
                };
            }
            entry.Parents = new ulong[entry.Depth];
            for (var i = 0; i < entry.Depth; i++)
            {
                entry.Parents[i] = br.ReadUInt64();
            }
            entry.SubdirEntries = new FileHashTable[entry.Dirs];
            for (var i = 0; i < entry.Dirs; i++)
            {
                entry.SubdirEntries[i] = Read(s);
            }

            return entry;
        }
    }

	internal static class NameTable
	{
		public static Dictionary<ulong, string> Read(Stream s)
		{
			Dictionary<ulong, string> table = new Dictionary<ulong, string>();
			using (BinaryReader br = new BinaryReader(s))
			{
				while (s.Position < s.Length)
				{
					ulong hash = br.ReadUInt64();
					int sz = br.ReadInt32();
					string str = Encoding.Unicode.GetString(br.ReadBytes(sz * 2));
					if (!table.ContainsKey(hash))
						table.Add(hash, str);
				}
			}
			return table;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct FileEntry
	{
		public ulong Hash;
		public long Offset;
	}
}
