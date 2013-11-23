using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Elte.PointCloudDB.Engine.Storage
{
    /// <summary>
    /// Implements base functionality of an operator that can read a tuple stream from an external file.
    /// </summary>
    private abstract class BulkFileReaderBase : Operators.OperatorBase
    {
        private string path;
        private int bufferSize;
        private Stream stream;

        private Schema.SchemaObjectCollection<Schema.Column> columns;

        protected Stream Stream
        {
            get { return stream; }
        }

        protected void Open()
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, true);
        }

        protected void Close()
        {
            stream.Close();
            stream.Dispose();
            stream = null;
        }
    }
}
