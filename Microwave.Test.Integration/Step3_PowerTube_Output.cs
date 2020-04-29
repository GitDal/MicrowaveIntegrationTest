using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microwave.Test.Integration.UtilityMethods;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step3_PowerTube_Output
    {
        private IPowerTube _tlm;
        private IOutput _output;
        private StringWriter _textWriter;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _tlm = new PowerTube(_output);

            _textWriter = new StringWriter();
            Console.SetOut(_textWriter);
        }

        [TearDown]
        public void Teardown()
        {
            _textWriter.Close();
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void TurnOn_IsOnIsFalse_PowerIsBetween1And100_OutputIsCalledCorrectly(int power)
        {
            // Act:
            _tlm.TurnOn(power);

            // Assert:
            Assert.That(_textWriter.ToString(), Contains.Substring(power.ToString()));
        }

        #region TurnOnExceptionTests

        [TestCase(0)]
        [TestCase(101)]
        public void TurnOn_IsOnIsFalse_PowerIsOutOfRange_ThrowsArgumentOutOfRangeExceptionAndOutputNotCalled(int power)
        {
            // Act + Assert:
            Assert.Multiple((() =>
            {
                Assert.That(() => _tlm.TurnOn(power), Throws.InstanceOf(typeof(ArgumentOutOfRangeException)));
                Assert.That(_textWriter.ToString(), Is.EqualTo(string.Empty)); // No output on console
            }));
        }

        [Test]
        public void TurnOn_IsOnIsTrue_PowerIsInRange_ThrowsApplicationExceptionAndOutputNotCalled()
        {
            // Arrange:
            _tlm.TurnOn(50); // --> IsOn = true;
            TextWriterHelper.ClearTextWriter(_textWriter);

            // Act + Assert:
            Assert.Multiple((() =>
            {
                Assert.That(() => _tlm.TurnOn(50), Throws.InstanceOf(typeof(ApplicationException)));
                Assert.That(_textWriter.ToString(), Is.EqualTo(string.Empty)); // No output on console
            }));
        }

        #endregion
        
        [Test]
        public void TurnOff_IsOnIsFalse_OutputNotCalled()
        {
            // Act:
            _tlm.TurnOff();

            // Assert:
            Assert.That(_textWriter.ToString(), Is.Empty);
        }

        [Test]
        public void TurnOff_IsOnIsTrue_OutputCalledCorrectly()
        {
            // Arrange:
            _tlm.TurnOn(50); //--> IsOn = true;
            TextWriterHelper.ClearTextWriter(_textWriter);

            // Act:
            _tlm.TurnOff();

            // Assert:
            Assert.That(_textWriter.ToString(), Is.Not.Empty);
        }
    }
}
