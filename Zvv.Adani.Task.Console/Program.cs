using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Zvv.Adani.Task.Base;
using static System.Console;

namespace Zvv.Adani.Task.Console
{
    class Program
    {
        static long _outputFilesCount;

        static void Main()
        {
            //input data
            Write("Type a directory name: ");
            string directoryName = null;
            while (string.IsNullOrWhiteSpace(directoryName))
                directoryName = ReadLine();

            //calculating
            WriteLine("Please wait...");
            _outputFilesCount = -1;
            DirectoryProcessor directoryProcessor = null;
            List<SumFileInfo> resultList = null;
            try
            {
                directoryProcessor = new DirectoryProcessor(directoryName);
                directoryProcessor.FileProcessed += DirectoryProcessor_FileProcessed;
                directoryProcessor.FilesCountReceived += DirectoryProcessor_FilesCountReceived;
                resultList = directoryProcessor.GetSums();
            }
            catch (Exception e)
            {
                WriteLine($"Error while processing the directory: {e.Message}");
            }
            finally
            {
                if (directoryProcessor != null)
                {
                    directoryProcessor.FilesCountReceived -= DirectoryProcessor_FilesCountReceived;
                    directoryProcessor.FileProcessed -= DirectoryProcessor_FileProcessed;
                }
            }

            //saving as XML
            if (resultList != null)
            {
                WriteLine("Saving to XML file...");
                try
                {
                    Tools.SaveAsXmlReport(directoryName, resultList);
                    WriteLine("Operation is completed.");
                }
                catch (Exception e)
                {
                    WriteLine($"Error while saving results to XML file: {e.Message}");
                }
            }
        }

        private static void DirectoryProcessor_FilesCountReceived(DirectoryProcessor sender, long result) =>
            _outputFilesCount = result;

        private static void DirectoryProcessor_FileProcessed(DirectoryProcessor sender, DirectoryProcessor.FileProcessedEventArgs e)
        {
            if (e.Succeed)
                WriteLine($"{e.FilesProcessed}/{(_outputFilesCount != -1 ? _outputFilesCount.ToString() : "-")} {e.Result}");
        }
    }
}
