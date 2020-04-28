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
        private IOutput _output;
        private IUserInterface _ui;
        private ITimer _timer;
        private IDisplay _display;
        private CookController _tlm;
        private PowerTube _powerTube;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _ui = Substitute.For<IUserInterface>();
            _timer = Substitute.For<ITimer>();
            _display = Substitute.For<IDisplay>();
            _powerTube = new PowerTube(_output);
            _tlm = new CookController(_timer, _display, _powerTube, _ui);
        }

        [TestCase(1,60)]
        [TestCase(2, 60)]
        [TestCase(50, 60)]
        [TestCase(99, 60)]
        [TestCase(100, 60)]
        public void StartCooking_CorrectPower_OutputReceivesFromPowerTube(int power, int time)
        {
            _tlm.StartCooking(power, time);

            _output.Received().OutputLine(Arg.Is<string>(str => str == $"PowerTube works with {power}"));
        }

        [TestCase(0, 60)]
        [TestCase(-1, 60)]
        [TestCase(-100, 60)]
        [TestCase(101, 60)]
        [TestCase(150, 60)]
        public void StartCooking_WrongPower_ThrowArgumentOutOfRangeException(int power, int time)
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => _tlm.StartCooking(power, time));
        }

        [Test]
        public void StartCooking_CookingAlreadyStarted_ThrowApplicationException()
        {
            _tlm.StartCooking(50,60);
            Assert.Throws<System.ApplicationException>(() => _tlm.StartCooking(50,60));
        }

        [Test]
        public void Stop_CookingWasStarted_CorrectOutput()
        {
            _tlm.StartCooking(50,60);
            _tlm.Stop();
            
            _output.Received().OutputLine(Arg.Is<string>(str => str == $"PowerTube turned off"));
        }

        [Test]
        public void Stop_CookingWasNotStarted_NoOutput()
        {
            _tlm.Stop();
            _output.DidNotReceive().OutputLine(Arg.Any<string>());
        }

    }
}
