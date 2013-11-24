using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Elte.PointCloudDB.CodeGen;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.Storage
{
    /// <summary>
    /// Implements base functionality of reading chunks of tuples from an external file.
    /// </summary>
    public abstract class BulkFileReaderBase : IDisposable
    {
        private string path;
        private int bufferSize;
        private int blockSize;
        private Stream inputStream;
        private TupleHelperBase tupleHelper;

        private SchemaObjectCollection<Column> columns;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        protected Stream InputStream
        {
            get { return inputStream; }
        }

        protected TupleHelperBase TupleHelper
        {
            get { return tupleHelper; }
        }

        public SchemaObjectCollection<Column> Columns
        {
            get { return columns; }
        }

        protected BulkFileReaderBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.bufferSize = Constants.DefaultBulkReadBufferSize;
            this.blockSize = Streams.Constants.DefaultChunkSize;
            this.columns = new SchemaObjectCollection<Column>();
        }

        public virtual void Dispose()
        {
            Close();
        }

        public virtual void Open()
        {
            if (inputStream != null)
            {
                throw new InvalidOperationException();
            }

            inputStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, true);

            tupleHelper = TupleFactory.Instance.GetTupleHelper(columns);
        }

        public virtual void Close()
        {
            tupleHelper = null;

            if (inputStream != null)
            {
                inputStream.Close();
                inputStream.Dispose();
                inputStream = null;
            }
        }

        public IEnumerable<TupleChunkBase> ReadChunks()
        {
            while (true)
            {
                var tc = OnReadNextChunk();

                if (tc == null)
                {
                    yield break;
                }
                else
                {
                    yield return tc;
                }
            }
        }

        protected abstract TupleChunkBase OnReadNextChunk();

    }
}
