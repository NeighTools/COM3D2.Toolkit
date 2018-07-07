using System;
using System.IO;

namespace COM3D2.Toolkit
{
	public class PartialStream : Stream
	{
		public Stream BaseStream { get; protected set; }

		public long BaseOffset { get; protected set; }


		public override bool CanRead => BaseStream.CanRead;

		public override bool CanSeek => BaseStream.CanSeek;

		public override bool CanWrite => BaseStream.CanWrite;

		public override long Length { get; }
		
		public override long Position
		{
			get => BaseStream.Position - BaseOffset;
			set
			{
				if (!CanSeek)
					throw new NotSupportedException("BaseStream is not seekable.");

				if (value < 0 || value > Length)
					throw new NotSupportedException("Seeking beyond limits is not supported in partial streams.");

				BaseStream.Position = BaseOffset + value;
			}
		}


		public PartialStream(Stream baseStream, long offset = 0, long length = 0)
		{
			BaseStream = baseStream;
			BaseOffset = offset;
			Length = length;

			Position = 0;
			BaseStream.Position = offset;
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
				default:
					Position += offset;
					break;
				case SeekOrigin.End:
					Position = Length + offset;
					break;
			}

			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("Partial streams cannot have their lengths set.");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (Position + count > Length)
				count = (int)(Length - Position);

			BaseStream.Read(buffer, offset, count);

			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (Position + count > Length)
				count = (int)(Length - Position);

			BaseStream.Write(buffer, offset, count);
		}
	}
}
