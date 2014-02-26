using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    class ColumnHelper<C, T> : ColumnHelperBase
        where C : class, new()
        where T : struct
    {
        private ColumnAllocatorDelegate<C>[] allocators;
        private ColumnValuesAssigner<C, T>[] assigners;

        /// <summary>
        /// Returns the type of the storage of columns chunk struct.
        /// </summary>
        /// <returns></returns>
        public override Type GetColumnsClassType()
        {
            return typeof(C);
        }

        /// <summary>
        /// Initializes the columns.
        /// </summary>
        /// <param name="columns"></param>
        public override void SetColumns(SchemaObjectCollection<Column> columns)
        {
            this.Columns.AddRange(columns);
            this.allocators = new ColumnAllocatorDelegate<C>[columns.Count];
            this.assigners = new ColumnValuesAssigner<C, T>[columns.Count];
        }

        public override void SetColumnAllocatorDelegate(int i, Delegate allocator)
        {
            this.allocators[i] = (ColumnAllocatorDelegate<C>)allocator;
        }

        public override void SetColumnValuesAssigner(int i, Delegate assigner)
        {
            this.assigners[i] = (ColumnValuesAssigner<C, T>)assigner;
        }

        /// <summary>
        /// Creates a chunk of the storage of columns with the given chunk size.
        /// </summary>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        /// <remarks>
        /// The the storage of columns will contain arrays of columns.
        /// </remarks>
        public override ColumnsChunkBase CreateChunk(int chunkSize)
        {
            return new ColumnsChunk<C, T>(chunkSize, this);
        }

        public void Allocate(C data, int chunkSize)
        {
            for (int i = 0; i < allocators.Length; i++)
            {
                allocators[i](data, chunkSize);
            }
        }

        public void Assign(T[] tuples, C data, int chunkSize)
        {
            for (int i = 0; i < assigners.Length; i++)
            {
                assigners[i](tuples, data, chunkSize);
            }
        }
    }
}
