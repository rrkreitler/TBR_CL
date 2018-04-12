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

        // Test for Url arg.
        [Test]
        public void InvalidUrlForBadUrlArg()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] {"", "startDate", "endDate" };
                string expectedMsg = "**Syntax Error - Invalid URL" + Environment.NewLine;

                // Act
                bool? expectedResult = sut.Validate(args);

                Assert.Multiple(() =>
                {
                    Assert.That(sw.ToString().Equals(expectedMsg));
                    Assert.That(expectedResult.Equals(false));
                });
            }
        }

        // Start and end date tests
        [Test]
        public void InvalidDateTimeMsgForBadStartDateArg()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "Url", "badDateString", "endDate" };
                string expectedMsg = "**Syntax Error - Invalid start/end date or time" + Environment.NewLine;

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
        public void InvalidDateTimeMsgForBadStartTimeArg()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "Url", "1/1/2017", "/ST", "badTimeString", "2/1/2017" };
                string expectedMsg = "**Syntax Error - Invalid start/end date or time" + Environment.NewLine;

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
        public void InvalidDateTimeMsgForBadEndDateArg()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "Url", "1/1/2017", "badEndDateString" };
                string expectedMsg = "**Syntax Error - Invalid start/end date or time" + Environment.NewLine;

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
        public void InvalidDateTimeMsgForBadendTimeArg()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "Url", "1/1/2017", "/ET", "badTimeString", "2/1/2017" };
                string expectedMsg = "**Syntax Error - Invalid start/end date or time" + Environment.NewLine;

                // Act
                bool? expectedResult = sut.Validate(args);

                Assert.Multiple(() =>
                {
                    Assert.That(sw.ToString().Equals(expectedMsg));
                    Assert.That(expectedResult.Equals(false));
                });
            }
        }

        // Unknown args
        [Test]
        public void UnknownValueForTooManyArgsWithoutSwithces()
        {
            // Arrange
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string[] args = new string[] { "Url", "1/1/2017", "badString", "2/1/2017" };
                string expectedMsg = "**Syntax Error - Unknown value" + Environment.NewLine;

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