using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Storage;

namespace Elte.PointCloudDB.Storage
{
    [TestClass]
    public class DelimitedTextFileReaderTest
    {

        [TestMethod]
        public void GetNextBlockTest()
        {
            var br = new DelimitedTextFileReader();
            br.Path = @"..\..\..\files\test1.csv";

            br.Columns.Add(new Column("Col1", DataType.Int32));
            br.Columns.Add(new Column("Col2", DataType.Int32));
            br.Columns.Add(new Column("Col3", DataType.Int32));

            br.Open();

            var block = br.ReadNextBlock();

            br.Close();
        }

    }
}
