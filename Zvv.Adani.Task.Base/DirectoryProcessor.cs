using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Zvv.Adani.Task.Base
{
    /// <summary>
    /// Provides tools for getting bytes sum for a directory.
    /// </summary>
    public class DirectoryProcessor
    {
        /// <summary>
        /// Selected directory.
        /// </summary>
        public DirectoryInfo Directory { get; set; }

        /// <summary>
        /// A <see cref="CancellationTokenSource"/> object for asynchronous tasks management.
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Creates an object of the class.
        /// </summary>
        /// <param name="directoryName">Selected directory name.</param>
        public DirectoryProcessor(string directoryName)
        {
            if (directoryName != null)
                Directory = new DirectoryInfo(directoryName);
            else
                throw new ArgumentNullException(nameof(directoryName));
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Find sums of bytes of files in the current directory.
        /// </summary>
        /// <returns>A list of file names and corresponded sums of their bytes.</returns>
        public List<SumFileInfo> GetSums()
        {
            var files = Directory.GetFiles("*.*", new EnumerationOptions { RecurseSubdirectories = true });
            FilesCountReceived?.Invoke(this, files.LongLength);
            var methodResult = new ConcurrentBag<SumFileInfo>();
            long filesProcessed = 0;

            var tasks = files.Select(file => System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                bool fileIsProcessed = false;
                try
                {
                    var fileResult = new SumFileInfo { FileName = file.FullName, SumOfBytes = FileProcessor.GetSumOfFileBytes(file, _cancellationTokenSource.Token) };
                    fileIsProcessed = true;
                    methodResult.Add(fileResult);
                    FileProcessed?.Invoke(this, new FileProcessedEventArgs(true, fileResult, ++filesProcessed));
                }
                catch(OperationCanceledException) { }
                catch(Exception e) when (!fileIsProcessed)
                {
                    FileProcessed?.Invoke(this, new FileProcessedEventArgs(false, new SumFileInfo { FileName = file.FullName }, ++filesProcessed, e.Message));
                }
                catch { }
            })).ToArray();
            System.Threading.Tasks.Task.WaitAll(tasks);
            return methodResult.ToList();
        }

        /// <summary>
        /// Cancels the executed operation on finding sums of bytes.
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Provides a method that will process the event <see cref="FileProcessed"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The arguments.</param>
        public delegate void FileProcessedEventHandler(DirectoryProcessor sender, FileProcessedEventArgs e);
        /// <summary>
        /// Provides a method that will process the event <see cref="FileCountReceived"/>.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="result">The arguments.</param>
        public delegate void FilesCountReceivedEventHandler(DirectoryProcessor sender, long result);

        /// <summary>
        /// Occurs while working of the method <see cref="GetSums"/>, when another file has been processed.
        /// </summary>
        public event FileProcessedEventHandler FileProcessed;
        /// <summary>
        /// Occurs while working of the method <see cref="GetSums"/>, when the total count of files in the directory has been found.
        /// </summary>
        public event FilesCountReceivedEventHandler FilesCountReceived;

        /// <summary>
        /// Arguments for the event <see cref="FileProcessed"/>.
        /// </summary>
        public class FileProcessedEventArgs:EventArgs
        {
            /// <summary>
            /// Displays whether the operation on bytes summation for the file has had success.
            /// </summary>
            public bool Succeed { get; set; }
            /// <summary>
            /// The result of the operation on bytes summation.
            /// </summary>
            public SumFileInfo Result { get; set; }
            /// <summary>
            /// Displays how many files has processed on this moment. It can serve as a serial number of the result.
            /// </summary>
            public long FilesProcessed { get; set; }
            /// <summary>
            /// The error message. Fills if the operation on bytes summation has failed.
            /// </summary>
            public string ErrorMessage { get; set; }

            /// <summary>
            /// Creates an object of the class.
            /// </summary>
            /// <param name="succeed">Displays whether the operation on bytes summation for the file has had success.</param>
            /// <param name="result">The result of the operation on bytes summation.</param>
            /// <param name="filesProcessed">Displays how many files has processed on this moment. It can serve as a serial number of the result.</param>
            /// <param name="errorMessage">The error message. Fills if the operation on bytes summation has failed.</param>
            public FileProcessedEventArgs(bool succeed, SumFileInfo result, long filesProcessed, string errorMessage=null) : base()
            {
                Succeed = succeed;
                Result = result;
                FilesProcessed = filesProcessed;
                ErrorMessage = errorMessage;
            }
        }
    }
}
