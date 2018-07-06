using System.IO;

namespace COM3D2.Toolkit
{
	internal static class Utility
	{
		public static bool ContentEqual(this byte[] b1, byte[] b2)
		{
			if (ReferenceEquals(b1, b2))
				return true;

			if (b1 == null || b2 == null)
				return false;

			if (b1.Length != b2.Length)
				return false;

			for (int i = 0; i < b1.Length; i++)
				if (b1[i] != b2[i])
					return false;

			return true;
		}

		public static void CopyTo(this Stream input, Stream output)
		{
			var buffer = new byte[4096];
			int bytesRead;

			while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
				output.Write(buffer, 0, bytesRead);
		}

		public static byte[] ToBytes(this Stream stream)
		{
			long prevPosition = stream.Position;
			
			stream.Position = 0;

			byte[] buffer = new byte[stream.Length];

			using (MemoryStream mem = new MemoryStream(buffer, 0, buffer.Length, true, true))
			{
				stream.CopyTo(mem);
				
				stream.Position = prevPosition;

				return buffer;
			}
		}
	}
}
