using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.CodeGen
{
    public class TupleHelper<T> : TupleHelperBase
        where T : struct
    {       
        private ColumnParserDelegate<T>[] columnParsers;

        public ColumnParserDelegate<T>[] ColumnParsers
        {
            get { return columnParsers; }
        }

        public TupleHelper()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public override Type GetTupleType()
        {
            return typeof(T);
        }

        public override void SetColumns(SchemaObjectCollection<Column> columns)
        {
            this.Columns.AddRange(columns);
            this.columnParsers = new ColumnParserDelegate<T>[columns.Count];
        }

        public override void SetParseColumnValueDelegate(int i, Delegate parser)
        {
            this.columnParsers[i] = (ColumnParserDelegate<T>)parser;
        }

        public override TupleBlockBase CreateBlock(int blockSize)
        {
            return new TupleBlock<T>(blockSize, this);
        }

        public void Parse(ref T data, string[] values)
        {
            for (int i = 0; i < columnParsers.Length; i++)
            {
                columnParsers[i](ref data, values[i]);
            }
        }
    }
}
