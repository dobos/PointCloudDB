using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Storage
{
    abstract class TextFileReaderBase : BulkFileReaderBase
    {
        protected delegate object ColumnParser(string value);

        private ColumnParser[] columnParsers;

        protected TextFileReaderBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columnParsers = null;
        }
    }
}
