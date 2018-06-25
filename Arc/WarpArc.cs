using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using COM3D2.Toolkit.Crypto;

namespace COM3D2.Toolkit.Arc
{
	public class WarpArc
	{
		protected WarpEncryption Encryption;

		public string PatchedArc { get; protected set; }

		public static WarpArc Read(Stream stream)
		{
			WarpArc arc = new WarpArc();

			using (BinaryReader reader = new BinaryReader(stream))
			{
				stream.Position = 8;

				int encryptedHeaderLength = reader.ReadInt32();
				byte[] encryptedHeader = reader.ReadBytes(encryptedHeaderLength);

				arc.Encryption = new WarpEncryption(encryptedHeader);

				arc.PatchedArc = arc.Encryption.DecryptString(encryptedHeader.Take(encryptedHeaderLength - 5).ToArray());
			}

			return arc;
		}
	}
}
