using System;
using System.Collections.Generic;
using System.Linq;
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
        private IDisplay _sut;
        private IOutput _output;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _sut = new Display(_output);
        }

        [Test]
        public void ShowTime_OutputCalledCorrectly()
        {

        }
    }
}
