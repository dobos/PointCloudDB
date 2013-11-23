using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Elte.PointCloudDB.Schema;

namespace Elte.PointCloudDB.CodeGen
{
    public class TupleHelper<T> : ITupleHelper
        where T : struct
    {
        public delegate void ColumnParserDelegate(ref T data, string value);

        private SchemaObjectCollection<Column> columns;
        private ColumnParserDelegate[] columnParsers;

        public SchemaObjectCollection<Column> Columns
        {
            get { return columns; }
        }

        public ColumnParserDelegate[] ColumnParsers
        {
            get { return columnParsers; }
        }

        public TupleHelper()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columns = new SchemaObjectCollection<Column>();
        }

        public Type GetTupleType()
        {
            return typeof(T);
        }

        void ITupleHelper.SetColumns(SchemaObjectCollection<Column> columns)
        {
            this.columns.AddRange(columns);
            this.columnParsers = new ColumnParserDelegate[columns.Count];
        }

        void ITupleHelper.SetParseColumnValueDelegate(int i, Delegate parser)
        {
            this.columnParsers[i] = (ColumnParserDelegate)parser;
        }
    }
}
