using System;
using Xunit;
using AWSLambdaFileWatermark;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void TestPDFWatermark()
        {
            var keyName = "test.pdf";


            var res = Common.ConverPDFByS3Download(keyName, "»ªÏÄº½¿ÕË®Ó¡²âÊÔ");

            Assert.NotNull(res);
        }

        [Fact]
        public void TestWritingFile()
        {
            var keyName = "temp/111.xlsx";


            new AWSS3().WritingAnObjectAsync("111.xlsx", keyName).Wait();

            Assert.NotNull("www");
        }

        [Fact]
        public void TestWritingStream()
        {
            var keyName = "test.pdf";
            var keyName_new = "testwriting.pdf";


            var res = Common.ConverPDFByStream(keyName, "»ªÏÄº½¿ÕË®Ó¡²âÊÔ");

            new AWSS3().WritingAnObjectAsync(keyName_new, res).Wait();

            Assert.NotNull(res);
        }
    }
}
