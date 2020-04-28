using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step3_PowerTube_Output
    {
        private PowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            powerTube = new PowerTube(new Output());
        }

        [Test]

    }
}
