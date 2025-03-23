using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace IL.Daiquiri.Storage
{
    // TODO: Добавить восстановление резервной копии
    public sealed class StorageManager
    {
        [UsedImplicitly]
        public StorageManager()
        {
        }

        private string BasePath => Application.persistentDataPath;

        public bool TryReadBytes(string fileName, out byte[] bytes, out Exception exception)
        {
            try
            {
                bytes = ReadBytes(fileName);
                exception = null;

                return true;
            }
            catch (Exception currentException)
            {
                bytes = null;
                exception = currentException;

                return false;
            }
        }

        private byte[] ReadBytes(string fileName)
        {
            var datFileName = GetFileNameWithExtension(fileName, ".dat");
            var bytes = ReadBytesWithHash(datFileName);

            return bytes;
        }

        private static byte[] ReadBytesWithHash(string fileName)
        {
            using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            var hash = ReadBytes(fileStream, 16);
            var bytes = ReadBytes(fileStream, (int)fileStream.Length - 16);

            using var md5 = MD5.Create();

            var valid = md5.ComputeHash(bytes).SequenceEqual(hash);

            if (!valid)
            {
                throw new InvalidOperationException();
            }

            return bytes;
        }

        private static byte[] ReadBytes(FileStream fileStream, int count)
        {
            var offset = 0;
            var bytes = new byte[count];

            while (count > 0)
            {
                var numberOfBytes = fileStream.Read(bytes, offset, count);

                if (numberOfBytes == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += numberOfBytes;
                count -= numberOfBytes;
            }

            return bytes;
        }

        public bool TryWriteBytes(string fileName, byte[] bytes, out Exception exception)
        {
            try
            {
                WriteBytes(fileName, bytes);

                exception = null;

                return true;
            }
            catch (Exception currentException)
            {
                exception = currentException;

                return false;
            }
        }

        private void WriteBytes(string fileName, byte[] bytes)
        {
            var datFileName = GetFileNameWithExtension(fileName, ".dat");
            var bakFileName = GetFileNameWithExtension(fileName, ".bak");
            var tmpFileName = GetFileNameWithExtension(fileName, ".tmp");

            WriteBytesWithHash(tmpFileName, bytes);

            if (File.Exists(datFileName))
            {
                File.Replace(tmpFileName, datFileName, bakFileName);
            }
            else
            {
                File.Move(tmpFileName, datFileName);
            }
        }

        private static void WriteBytesWithHash(string fileName, byte[] bytes)
        {
            using var md5 = MD5.Create();

            var hash = md5.ComputeHash(bytes);

            using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);

            fileStream.Write(hash, 0, hash.Length);
            fileStream.Write(bytes, 0, bytes.Length);
        }

        private string GetFileNameWithExtension(string fileName, string extension)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(Path.Combine(BasePath, fileName));
            stringBuilder.Append(extension);

            return stringBuilder.ToString();
        }
    }
}
