using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zvv.Adani.Task.Base
{
    /// <summary>
    /// Provides tools for getting bytes sum for a file.
    /// </summary>
    public static class FileProcessor
    {
        /// <summary>
        /// Returns sum of bytes of specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling the operation. If the operation is cancelled, an <see cref="OperationCanceledException"/> is thrown.</param>
        /// <returns>Sum of bytes of specified file.</returns>
        public static ulong GetSumOfFileBytes(FileInfo file, CancellationToken cancellationToken)
        {
            if (file != null)
                return GetSumOfFileBytes(file.FullName, cancellationToken);
            else
                throw new ArgumentNullException(nameof(file));
        }

        /// <summary>
        /// Returns sum of bytes of specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling the operation. If the operation is cancelled, an <see cref="OperationCanceledException"/> is thrown.</param>
        /// <returns>Sum of bytes of specified file.</returns>
        public static ulong GetSumOfFileBytes(string fileName, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
                return GetSumOfStreamBytes(new FileStream(fileName, FileMode.Open), cancellationToken);
            else
                throw new ArgumentException($"Wrong {nameof(fileName)} argument.");
        }

        /// <summary>
        /// Returns sum of bytes of specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">A cancellation token for cancelling the operation. If the operation is cancelled, an <see cref="OperationCanceledException"/> is thrown.</param>
        /// <returns>Sum of bytes of specified stream.</returns>
        private static ulong GetSumOfStreamBytes(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (stream.Position > 0)
                stream.Seek(0, SeekOrigin.Begin);

            ulong result = 0;
            using var binaryReader = new BinaryReader(stream);
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                if (!cancellationToken.IsCancellationRequested)
                    result = checked(result + binaryReader.ReadByte());
                else
                    throw new OperationCanceledException(cancellationToken);
            }
                
            return result;
        }
    }
}
