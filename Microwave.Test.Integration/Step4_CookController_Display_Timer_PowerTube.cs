using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step4_CookController_Display_Timer_PowerTube
    {
        private IOutput output;
        private CookController cookController;
        private Timer timer;
        private PowerTube powerTube;
        private Display display;

        [SetUp]
        public void Setup()
        {
            output = Substitute.For<IOutput>();
            display = new Display(output);
            powerTube = new PowerTube(output);
            timer = new Timer();
            cookController = new CookController(timer, display, powerTube);
        }
    }
}
