using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.Storage
{
    public class DelimitedTextFileReader : TextFileReaderBase
    {
        private char columnSeparator;

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

        public override TupleBlockBase ReadNextBlock()
        {
            TupleBlockBase block = null;
            var cc = Columns.Count;

            while (true)
            {
                var line = InputReader.ReadLine();

                // If end of file reached return block
                if (line == null) break;

                // Create a new block
                if (block == null)
                {
                    block = TupleHelper.CreateBlock(BlockSize);
                }

                // Parse current line
                var parts = line.Split(columnSeparator);
                if (parts.Length != cc)
                {
                    throw new Exception();  // TODO
                }

                // Append to the block
                block.AppendTuple(parts);

                // If the block is full, return it
                if (!block.HasSpace) break;
            }

            return block;
        }
    }
}
