using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Zvv.Adani.Task.Base.Tests
{
    /// <summary>
    /// Tests for <see cref="FileProcessor"/>.
    /// </summary>
    [TestClass]
    public class FileProcessorUnitTests
    {
        [TestMethod]
        [DataRow((ulong)0)]
        [DataRow((ulong)10)]
        [DataRow((ulong)1000)]
        [DataRow((ulong)1000000)]
        public void GetSumOfFileBytes_RandomBytes_CorrectSum(ulong length)
        {
            var testFileName = $"TestFile{Guid.NewGuid()}.dat";
            CreateTestFile(testFileName, length, true, out ulong expectedSum);
            var resultSum = FileProcessor.GetSumOfFileBytes(new FileInfo(testFileName), new CancellationToken());
            File.Delete(testFileName);
            Assert.AreEqual(expectedSum, resultSum);
        }

        [TestMethod]
        [DataRow((ulong)1000000000)]
        public void GetSumOfFileBytes_CancellationToken_Cancelled(ulong length)
        {
            var testFileName = $"TestFile{Guid.NewGuid()}.dat";
            CreateTestFile(testFileName, length, false, out _);
            var cancellationTokenSource = new CancellationTokenSource();
            try
            {
                var task = System.Threading.Tasks.Task.Factory.StartNew(() => FileProcessor.GetSumOfFileBytes(new FileInfo(testFileName), cancellationTokenSource.Token));
                cancellationTokenSource.Cancel();
                task.Wait();
                Assert.Fail("Operation has not been cancelled. Try to raise the file length.");
            }
            catch(AggregateException e)
            {
                Assert.IsNotNull(e.InnerExceptions.SingleOrDefault((ie) => ie is OperationCanceledException));
            }
            File.Delete(testFileName);
        }

        private void CreateTestFile(string fileName, ulong length, bool countSum, out ulong expectedSum)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            using var fileStream = File.Create(fileName);
            expectedSum = 0;
            var random = new Random();
            byte nextByte;
            using var binaryWriter = new BinaryWriter(fileStream);
            for (ulong i = 0; i < length; i++)
            {
                nextByte = (byte)random.Next(byte.MinValue, byte.MaxValue);
                binaryWriter.Write(nextByte);
                if (countSum)
                    try
                    {
                        expectedSum = checked(expectedSum + nextByte);
                    }
                    catch (OverflowException)
                    {
                        Assert.Fail("An OverflowException was thrown.");
                    }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetSumOfFileBytes_NullFileName_ArgumentException()
        {
            FileProcessor.GetSumOfFileBytes((string)null, new CancellationToken());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSumOfFileBytes_NullFileInfo_ArgumentNullException()
        {
            FileProcessor.GetSumOfFileBytes((FileInfo)null, new CancellationToken());
        }
    }
}
