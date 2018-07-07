using System;
using System.Collections.Generic;
using System.IO;

namespace COM3D2.Toolkit.Utils
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

	    public static IEnumerable<U> Flatten<T, U>(
	            T element,
	            Func<T, IEnumerable<T>> elementSelector,
	            Func<T, IEnumerable<U>> valueSelector)
	    {
	        foreach (U node in valueSelector(element))
	            yield return node;
	        foreach (T subElem in elementSelector(element))
	        foreach (U node in Flatten(subElem, elementSelector, valueSelector))
	            yield return node;
	    }

	    public static IEnumerable<U> Flatten<T, U>(T element, Func<T, T> elementSelector, Func<T, IEnumerable<U>> valueSelector)
	    {
	        foreach (U node in valueSelector(element))
	            yield return node;
	        foreach (U node in Flatten(elementSelector(element), elementSelector, valueSelector))
	            yield return node;
	    }

        public static uint Reverse(this uint self)
	    {
	        // It's stupid, but JIT generates 6 bytes less of ASM this way
	        // :)
	        uint result = (self & 0xFF) << 24;
	        result |= (self & 0xFF00) << 8;
	        result |= (self & 0xFF0000) >> 8;
	        result |= (self & 0xFF000000) >> 24;
	        return result;
	    }
	}
}
