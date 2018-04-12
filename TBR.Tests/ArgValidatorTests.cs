using System;
using System.IO;
using NUnit.Framework;
using TBR;


namespace Tests
{
    [TestFixture]
    public class ArgValidatorTests
    {
        private ArgValidator sut;

        [SetUp]
        public void SetUp()
        {
            sut = new ArgValidator();
        }

        // Tests for minimum number of args.
        [Test]
        public void InvalidCommandForNoArgs()
        {
            // Using defaults, the min number of args required is 3.
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[0];
                string expectedMsg = "**Syntax Error - invalid command" + Environment.NewLine;

                // Act
                bool? expectedResult = sut.Validate(args);

                Assert.Multiple(() =>
                {
                    Assert.That(sw.ToString().Equals(expectedMsg));
                    Assert.That(expectedResult.Equals(false));
                });
            }
        }

        [Test]
        public void InvalidCommandForOneArgs()
        {
            // Using defaults, the min number of args required is 3.
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args =new string[] {"test"};
                string expectedMsg = "**Syntax Error - invalid command" + Environment.NewLine;

                // Act
                bool? expectedResult = sut.Validate(args);

                Assert.Multiple(() =>
                {
                    Assert.That(sw.ToString().Equals(expectedMsg));
                    Assert.That(expectedResult.Equals(false));
                });
            }
        }

        [Test]
        public void InvalidCommandForTwoArgs()
        {
            // Using defaults, the min number of args required is 3.
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "test", "test" };
                string expectedMsg = "**Syntax Error - invalid command" + Environment.NewLine;

                // Act
                bool? expectedResult = sut.Validate(args);

                Assert.Multiple(() =>
                {
                    Assert.That(sw.ToString().Equals(expectedMsg));
                    Assert.That(expectedResult.Equals(false));
                });
            }
        }

        
    }
}