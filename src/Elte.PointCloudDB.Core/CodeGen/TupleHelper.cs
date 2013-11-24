using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.CodeGen
{
    /// <summary>
    /// Implements a strongly typed generic tuple helper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This class implements functions to manipulate dynamically generated
    /// tuple structs.
    /// Column parsers are generated when the tuple struct is created, so
    /// parsing can be done via direct delegate calls instead of complex
    /// dispatch logic.
    /// </remarks>
    public class TupleHelper<T> : TupleHelperBase
        where T : struct
    {       
        /// <summary>
        /// Holds parser delegates for each column.
        /// </summary>
        private ColumnParserDelegate<T>[] columnParsers;

        /// <summary>
        /// Gets the column parsers.
        /// </summary>
        public ColumnParserDelegate<T>[] ColumnParsers
        {
            get { return columnParsers; }
        }

        public TupleHelper()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Initializes private variables.
        /// </summary>
        private void InitializeMembers()
        {
        }

        /// <summary>
        /// Returns the type of the tuple struct.
        /// </summary>
        /// <returns></returns>
        public override Type GetTupleType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Initializes the columns.
        /// </summary>
        /// <param name="columns"></param>
        public override void SetColumns(SchemaObjectCollection<Column> columns)
        {
            this.Columns.AddRange(columns);
            this.columnParsers = new ColumnParserDelegate<T>[columns.Count];
        }

        /// <summary>
        /// Initializes a column parser delegate.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="parser"></param>
        public override void SetParseColumnValueDelegate(int i, Delegate parser)
        {
            this.columnParsers[i] = (ColumnParserDelegate<T>)parser;
        }

        /// <summary>
        /// Creates a tuple block with the given block size.
        /// </summary>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        /// <remarks>
        /// The tuple block will contain an array of tuple structs.
        /// </remarks>
        public override TupleChunkBase CreateBlock(int blockSize)
        {
            return new TupleChunk<T>(blockSize, this);
        }

        /// <summary>
        /// Parses a set of strings and stores the values in the
        /// appropriate fields of the tuple struct.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="values"></param>
        /// <remarks>
        /// The function uses pre-generated delegates instead of slow
        /// dispatch logic. Parsing is used with bulk insert.
        /// </remarks>
        public void Parse(ref T data, string[] values)
        {
            for (int i = 0; i < columnParsers.Length; i++)
            {
                columnParsers[i](ref data, values[i]);
            }
        }
    }
}
