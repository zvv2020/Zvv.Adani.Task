using Castle.Core.Internal;
using Castle.DynamicProxy.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Zvv.Adani.Task.Base.Tests
{
    /// <summary>
    /// Tests for <see cref="Tools"/>.
    /// </summary>
    [TestClass]
    public class ToolsUnitTests
    {
        [TestMethod]
        public void SaveAsXmlReport_CorrectCollection_Success()
        {
            var sourceList = new List<SumFileInfo>();
            var random = new Random();
            for (int i = 0; i < 10000; i++)
                sourceList.Add(new SumFileInfo { FileName = Guid.NewGuid().ToString(), SumOfBytes = (ulong)random.Next() });
            var testFileName = $"TestFile{Guid.NewGuid()}.xml"; //Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"TestFile{Guid.NewGuid()}.xml");
            if (File.Exists(testFileName))
                File.Delete(testFileName);
            Tools.SaveAsXmlReport(@"\", sourceList, testFileName);

            using (var fileStream = new FileStream(testFileName, FileMode.Open))
            {
                var resultList = (List<SumFileInfo>)new XmlSerializer(typeof(List<SumFileInfo>)).Deserialize(fileStream);
                CollectionAssert.AreEqual(sourceList, resultList);
            }
            File.Delete(testFileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataRow(true,true)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void SaveAsXmlReport_NullArguments_ThrowArgumentNullException(bool directoryIsNull, bool collectionIsNull)
        {
            var sourceList = new List<SumFileInfo>();
            if (!collectionIsNull)
            {
                var random = new Random();
                for (int i = 0; i < 10000; i++)
                    sourceList.Add(new SumFileInfo { FileName = Guid.NewGuid().ToString(), SumOfBytes = (ulong)random.Next() });
            }
            Tools.SaveAsXmlReport(directoryIsNull ? null : @"\", collectionIsNull ? null : sourceList);
        }
    }
}
