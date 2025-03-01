using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SlyTools.Common.IO
{
    public enum FileEndian
    {
        Big,
        Little
    };

    public class BinaryStream : IDisposable
    {
        private FileStream stream;
        public long Position => stream.Position;
        public long Length => stream.Length;
        public FileEndian Endian { get; set; }
        public bool IsLittleEndian => Endian == FileEndian.Little;
        public bool IsBigEndian => !IsLittleEndian;

        /// <summary>
        /// Opens a given file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="mode">The file mode as represented in fopen.</param>
        /// <param name="endian">The file endianness.</param>
        public BinaryStream(string path, string mode, FileEndian endian = FileEndian.Little)
        {
            if (mode.Contains('r'))
            {
                stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            }
            else
            {
                stream = File.Open(path, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            Endian = endian;
        }

        /// <summary>
        /// Clones a binary stream using the same stream.
        /// </summary>
        /// <param name="instance">The stream to clone.</param>
        public BinaryStream(BinaryStream instance)
        {
            if (instance.stream.CanWrite)
            {
                stream = new FileStream(instance.stream.Name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            else
            {
                stream = new FileStream(instance.stream.Name, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            Endian = instance.Endian;
        }

        /// <summary>
        /// Creates a binary stream by reading an offset of the given type from an existing file and skips to that offset.
        /// </summary>
        /// <typeparam name="T">The position type.</typeparam>
        /// <param name="stream">The binary stream to inherit from.</param>
        /// <returns>The file stream or null if the value could not be read.</returns>
        public static BinaryStream CreateFromStream<T>(BinaryStream stream, bool ignore_zero = false) where T : unmanaged, IConvertible
        {
            T value = new T();

            if (stream.Read(ref value))
            {
                long offset = Convert.ToInt64(value);

                if (!ignore_zero || offset != 0)
                {
                    return CreateFromStreamOffset(stream, offset);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a binary stream from an existing file and skips to the supplied offset.
        /// </summary>
        /// <param name="stream">The binary stream to inherit from.</param>
        /// <param name="offset">The offset to seek to.</param>
        /// <returns></returns>
        public static BinaryStream CreateFromStreamOffset(BinaryStream stream, long offset)
        {
            BinaryStream result = new BinaryStream(stream);
            
            if (!result.Skip(offset))
            {
                result.Dispose();
                return null;
            }

            return result;
        }

        /// <summary>
        /// Changes the current position of the file.
        /// </summary>
        /// <param name="offset">The position offset.</param>
        /// <param name="seekOrigin">The seek origin.</param>
        /// <returns>Whether or not the operation suceeded.</returns>
        public bool Seek(long offset, SeekOrigin seekOrigin)
        {
            if (!stream.CanSeek)
            {
                return false;
            }

            stream.Seek(offset, seekOrigin);
            return true;
        }

        /// <summary>
        /// Skips a number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool Skip(long count)
        {
            return Seek(count, SeekOrigin.Current);
        }

        /// <summary>
        /// Writes padding to the stream.
        /// </summary>
        /// <param name="count">Number of byes to write.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool Pad(int count)
        {
            if (!stream.CanWrite)
            {
                return false;
            }

            byte[] padding = new byte[count];
            stream.Write(padding);
            return true;
        }

        /// <summary>
        /// Checks if this file is in read mode and can read the supplied number of bytes.
        /// </summary>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>Whether or not the value can be read.</returns>
        public bool CanRead(long length = 1)
        {
            if (stream.CanRead)
            {
                return Length - Position >= length;
            }

            return false;
        }

        /// <summary>
        /// Reads a value from the file.
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <param name="value">The output value.</param>
        /// <returns>Whether or not the operation suceeded.</returns>
        public bool Read<T>(ref T value) where T : unmanaged
        {
            int size = Marshal.SizeOf(value);
            
            if (!CanRead(size))
            {
                return false;
            }

            byte[] buffer = new byte[size];
            stream.Read(buffer);

            // Swap bytes
            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(buffer);
            }

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            value = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return true;
        }

        /// <summary>
        /// Reads an array of bytes the file.
        /// </summary>
        /// <param name="value">The output value.</param>
        /// <returns>Whether or not the operation suceeded.</returns>
        public bool Read(ref byte[] value)
        {
            int size = value.Length;

            if (!CanRead(size))
            {
                return false;
            }

            stream.Read(value);
            return true;
        }

        /// <summary>
        /// Reads a null-terminated string from the file.
        /// </summary>
        /// <param name="value">The output string.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool ReadString(ref string value)
        {
            long size = 0;
            long position = Position;

            while (CanRead())
            {
                if (stream.ReadByte() == '\0')
                {
                    Seek(position, SeekOrigin.Begin);
                    ReadString(ref value, size);
                    return true;
                }

                ++size;
            }

            Seek(position, SeekOrigin.Begin);
            return false;
        }

        /// <summary>
        /// Reads a string from the file and trims any null-terminators that may be present.
        /// </summary>
        /// <param name="value">The output string.</param>
        /// <param name="size">The string length.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool ReadString(ref string value, long size)
        {
            if (!CanRead(size))
            {
                return false;
            }

            byte[] buffer = new byte[size];
            stream.Read(buffer);
            value = Encoding.ASCII.GetString(buffer).Trim('\0');

            return true;
        }

        /// <summary>
        /// Reads a run length encoded string from the file.
        /// </summary>
        /// <typeparam name="T">The type of the length encoding.</typeparam>
        /// <param name="value">The output string.</param>
        /// <returns>Whether or not the operation suceeded.</returns>
        public bool ReadString<T>(ref string value) where T : unmanaged, IConvertible
        {
            T size = new T();

            if (!Read(ref size))
            {
                return false;
            }

            return ReadString(ref value, Convert.ToInt64(size));
        }

        /// <summary>
        /// Writes a value to the binary stream.
        /// </summary>
        /// <typeparam name="T">The type to write.</typeparam>
        /// <param name="value">The input value.</param>
        /// <param name="alignment">The alignment of the value.</param>
        /// <returns>Whether or not the operation succeeded.</returns>
        public bool Write<T>(T value, ushort alignment = 0) where T : unmanaged
        {
            if (!stream.CanWrite)
            {
                return false;
            }

            int size = Marshal.SizeOf(value);
            byte[] bytes = new byte[size];

            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, size);
            handle.Free();

            if (BitConverter.IsLittleEndian != IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            stream.Write(bytes, 0, size);

            // TODO: This assumes you're already making an aligned write
            if (alignment != 0 && size % alignment != 0)
            {
                return Pad(alignment - (size % alignment));
            }

            return true;
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
