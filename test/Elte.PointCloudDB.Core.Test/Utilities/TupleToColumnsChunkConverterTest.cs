using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Utilities
{
    [TestClass]
    public class TupleToColumnsChunkConverterTest
    {
        [TestMethod]
        public void TestConvertChunk()
        {
            var columns = new SchemaObjectCollection<Column>()
            {
                new Schema.Column("Col1", Schema.DataType.UInt64),
                new Schema.Column("Col2", Schema.DataType.Single),
                new Schema.Column("Col3", Schema.DataType.Single),
                new Schema.Column("Col4", Schema.DataType.Single),
                new Schema.Column("Col5", Schema.DataType.Int64),
                new Schema.Column("Col6", Schema.DataType.Int32)
            };
            var chunkSize = Streams.Constants.DefaultChunkSize;
            var converter = new TupleToColumnsChunkConverter(columns, chunkSize);
            var tupleChunk = GetTupleChunk(columns, chunkSize);

            var storageOfColumnsChunk = converter.ConvertChunk(tupleChunk);
            Assert.AreEqual(storageOfColumnsChunk.ChunkSize, chunkSize);
            
            var columnChunks = storageOfColumnsChunk.ColumnChunks;
            // TODO further investigations
        }

        // TODO this method should be replaced with mocking because this test is not unit test in this way but integration test.
        private TupleChunkBase GetTupleChunk(SchemaObjectCollection<Column> columns, int chunkSize)
        {
            var helper = TupleFactory.Instance.GetTupleHelper(columns);
            var tupleType = helper.GetTupleType();
            
            // Create generic type for the tuple chunk and intantiate chunk class
            var tupleChunkType = typeof(TupleChunk<>).MakeGenericType(tupleType);
            var result = (TupleChunkBase)Activator.CreateInstance(tupleChunkType, new object[] { chunkSize, helper });
            return result;
        }
    }
}
