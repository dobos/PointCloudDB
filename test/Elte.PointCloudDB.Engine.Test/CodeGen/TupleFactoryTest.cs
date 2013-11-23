using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elte.PointCloudDB.CodeGen
{
    [TestClass]
    public class TupleFactoryTest
    {
        [TestMethod]
        public void GetTupleNameTest()
        {
            var columns = new Schema.Column[]
            {
                new Schema.Column("Col1", Schema.DataType.UInt64),
                new Schema.Column("Col2", Schema.DataType.Single),
                new Schema.Column("Col3", Schema.DataType.Single),
                new Schema.Column("Col4", Schema.DataType.Single),
                new Schema.Column("Col5", Schema.DataType.Int64),
                new Schema.Column("Col6", Schema.DataType.Int32)
            };

            var t = TupleFactory.Instance.GetTupleStuctType(columns);

            Assert.AreEqual("__tuple_ui8_f4_f4_f4_i8_i4", t.Name);

            var fs = t.GetFields();
            Assert.AreEqual(6, fs.Length);
            Assert.AreEqual("__column_0", fs[0].Name);
        }
    }
}
