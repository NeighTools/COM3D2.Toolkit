using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace COM3D2.Toolkit
{
    public class ChainedStream : Stream
    {
        protected List<Stream> BackingStreams;
        protected long[] BaseOffsets;

        public ChainedStream(params Stream[] streams) : this(streams.AsEnumerable()) { }

        public ChainedStream(IEnumerable<Stream> streams)
        {
            BackingStreams = new List<Stream>(streams);
            BaseOffsets = BackingStreams.Select(x => x.Position).ToArray();
        }

        public override bool CanRead => BackingStreams.All(x => x.CanRead);

        public override bool CanSeek => BackingStreams.All(x => x.CanSeek);

        public override bool CanWrite => BackingStreams.All(x => x.CanWrite);

        public override long Length => BackingStreams.Sum(x => x.Length);

        public override long Position
        {
            get => BackingStreams.Select((t, i) => t.Position - BaseOffsets[i]).Sum();
            set
            {
                long pos = value;

                for (int i = 0; i < BackingStreams.Count; i++)
                {
                    long length = BackingStreams[i].Length - BaseOffsets[i];

                    BackingStreams[i].Position = Math.Min(pos, length) + BaseOffsets[i];
                    pos -= length;

                    if (pos < 0)
                        pos = 0;
                }
            }
        }

        public override void Flush()
        {
            foreach (Stream stream in BackingStreams)
                stream.Flush();
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
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;

            while (count > 0)
            {
                Stream currentStream = GetCurrentStream();

                if (currentStream.Position == currentStream.Length) //end of last stream
                    break;

                int canRead = (int) (currentStream.Length - currentStream.Position);
                int localRead = currentStream.Read(buffer, offset, Math.Min(count, canRead));

                offset += localRead;
                read += localRead;
                count -= localRead;
            }

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected Stream GetCurrentStream()
        {
            long position = Position;

            foreach (Stream stream in BackingStreams)
            {
                if (position < stream.Length)
                    return stream;

                position -= stream.Length;
            }

            return BackingStreams.Last();
        }

        protected override void Dispose(bool disposing)
        {
            foreach (Stream stream in BackingStreams)
                stream.Dispose();

            base.Dispose(disposing);
        }
    }
}