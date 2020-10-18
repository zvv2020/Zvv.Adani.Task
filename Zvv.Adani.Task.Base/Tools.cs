using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zvv.Adani.Task.Base
{
    /// <summary>
    /// Provides various tools.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Saves a <see cref="List{SumFileInfo}"/> object as XML file.
        /// </summary>
        /// <param name="directory">A directory which the file will be saved to.</param>
        /// <param name="collection">The collection which will be convert to XML.</param>
        /// <param name="fileName">A name of the XML file. If null, the name will be "report.xml".</param>
        public static void SaveAsXmlReport(string directory, List<SumFileInfo> collection, string fileName=null)
        {
            if (directory==null)
                throw new ArgumentNullException(nameof(directory));
            else if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var serializer = new XmlSerializer(typeof(List<SumFileInfo>));
            var resultFileName = fileName ?? "report.xml";
            using var fileStream = new FileStream(resultFileName, FileMode.Create);
            serializer.Serialize(fileStream, collection);
        }
    }
}
