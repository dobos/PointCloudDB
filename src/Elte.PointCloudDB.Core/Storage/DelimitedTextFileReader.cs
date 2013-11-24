using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.Storage
{
    public class DelimitedTextFileReader : BulkFileReaderBase, IDisposable
    {
        private char columnSeparator;

        private StreamReader inputReader;

        public char ColumnSeparator
        {
            get { return columnSeparator; }
            set { columnSeparator = value; }
        }

        public DelimitedTextFileReader()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columnSeparator = ',';
        }

        public override void Dispose()
        {
            Close();

            base.Dispose();
        }

        public override void Open()
        {
            base.Open();

            if (inputReader != null)
            {
                throw new InvalidOperationException();
            }

            inputReader = new StreamReader(InputStream);
        }

        public override void Close()
        {
            if (inputReader != null)
            {
                inputReader.Close();
                inputReader.Dispose();
                inputReader = null;
            }

            base.Close();
        }

        protected override TupleChunkBase OnReadNextChunk()
        {
            TupleChunkBase chunk = null;
            var cc = Columns.Count;

            while (true)
            {
                var line = inputReader.ReadLine();

                // If end of file reached return block
                if (line == null) break;

                // Create a new block
                if (chunk == null)
                {
                    chunk = TupleHelper.CreateBlock(BlockSize);
                }

                // Parse current line
                var parts = line.Split(columnSeparator);
                if (parts.Length != cc)
                {
                    throw new Exception();  // TODO
                }

                // Append to the block
                chunk.AppendTuple(parts);

                // If the block is full, return it
                if (chunk.IsFull) break;
            }

            return chunk;
        }
    }
}
