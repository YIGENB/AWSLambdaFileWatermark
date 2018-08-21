using System;
using Xunit;
using AWSLambdaFileWatermark;

namespace XUnitTestProject1
{
    public class UnitTestLambda
    {
        [Fact]
        public void TestLambda()
        {
            var function = new TestFunction();

            Assert.NotEmpty("HELLO WORLD");
        }
    }
}
