using System;
using System.Collections.Generic;
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
        private IOutput _output;
        private ILight _uut;

        [SetUp]
        public void SetUp()
        {
                _output = new Output();
                _uut = new Light(_output);
        }
    }
}
