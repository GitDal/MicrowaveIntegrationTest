using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step1_Display_Output
    {
        private IDisplay _tlm;
        private IOutput _output;
        private StringWriter textWriter;
        private StringReader textReader;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _tlm = new Display(_output);

            textWriter = new StringWriter();
           
            Console.SetOut(textWriter);
        }

        [TearDown]
        public void Teardown()
        {
            textWriter.Close();
        }

        [TestCase(-1, -1)]
        [TestCase(0, 0)]
        [TestCase(1,1)]
        public void ShowTime_GivenMinutesAndSeconds_OutputIsCalledCorrectly(int mins, int secs)
        {
            // Act:
            _tlm.ShowTime(mins, secs);

            // Assert:
            //Assert.That(textWriter.ToString(),Is.EqualTo($"Display shows: {mins:D2}:{secs:D2}") );

            Assert.Multiple((() =>
            {
                Assert.That(textWriter.ToString(), Contains.Substring(mins.ToString("D2")));
                Assert.That(textWriter.ToString(), Contains.Substring(secs.ToString("D2")));
            }));
        }

        [TestCase(1)]
        public void ShowPower_GivenPower_OutputIsCalledCorrectly(int power)
        {
            // Act:
            _tlm.ShowPower(power);

            Assert.That(textWriter.ToString(), Contains.Substring(power.ToString()));
        }

    }
}
