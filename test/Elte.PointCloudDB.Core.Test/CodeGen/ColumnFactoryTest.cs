using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elte.PointCloudDB.Schema;

namespace Elte.PointCloudDB.CodeGen
{
    [TestClass]
    public class ColumnFactoryTest
    {
        [TestMethod]
        public void GetColumnHelperTest()
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

            var helper = ColumnFactory.Instance.GetColumnHelper(columns, chunkSize);

            var t = helper.GetColumnsStructType();

            Assert.AreEqual("__columnChunks_ui8_f4_f4_f4_i8_i4", t.Name);

            var fs = t.GetFields();
            Assert.AreEqual(6, fs.Length);
            Assert.AreEqual("__column_0", fs[0].Name);
        }
    }
}
