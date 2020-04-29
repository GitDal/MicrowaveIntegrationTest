using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    class Step2_Light_Output
    {
        private ILight _lmt;
        private IOutput _output;
        private StringWriter textWriter;

        [SetUp]
        public void SetUp()
        {
                _output = new Output();
                _lmt = new Light(_output);

                textWriter = new StringWriter();

                Console.SetOut(textWriter);
        }

        [TearDown]
        public void TearDown()
        {
            textWriter.Close();
        }

        [Test]
        public void TurnOn_IsOnIsFalse_OutputIsCalledCorrectly()
        {
            // Act:
            _lmt.TurnOn();

            // Assert:
            Assert.That(textWriter.ToString(), Contains.Substring("on"));
        }
        [Test]
        public void TurnOn_IsOnIsTrue_NothingHappened()
        {
            // Arrange:
            _lmt.TurnOn(); // IsOn = true
            ClearTextWriter(textWriter); // Clear all output

            // Act:
            _lmt.TurnOn();

            // Assert:
            Assert.That(textWriter.ToString(), Is.EqualTo(string.Empty)); // Nothing happened (still empty console-output)
        }

        [Test]
        public void TurnOff_IsOnIsFalse_NothingHappened()
        {
            // Act:
            _lmt.TurnOff();

            // Assert:
            Assert.That(textWriter.ToString(), Is.EqualTo(string.Empty));
        }
        [Test]
        public void TurnOff_IsOnIsTrue_OutputIsCalledCorrectly()
        {
            // Arrange:
            _lmt.TurnOn(); // IsOn = true
            ClearTextWriter(textWriter); // Clear all output

            // Act:
            _lmt.TurnOff();

            // Assert:
            Assert.That(textWriter.ToString(), Contains.Substring("off")); // Nothing happened (still empty console-output)
        }


        // Utility method
        private void ClearTextWriter(StringWriter tr)
        {
            StringBuilder sb = tr.GetStringBuilder();
            sb.Remove(0, sb.Length);
        }
    }
}
