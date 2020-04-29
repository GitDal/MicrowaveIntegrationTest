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
    public class Step6_UserInterface_Light_CookController
    {
        private IUserInterface _tlm;
        private ILight _light;
        private ICookController _cookController;
        private IPowerTube _powerTube;
        private IDisplay _display;

        // Mocks
        private IDoor _door;
        private IButton _powerButton, _timeButton, _startCancelButton;
        private ITimer _timer;
        private IOutput _output;

        [SetUp]
        public void Setup()
        {
            // Set up mocks
            _door = Substitute.For<IDoor>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _timer = Substitute.For<ITimer>();
            _output = Substitute.For<IOutput>();

            _display = new Display(_output);
            _light = new Light(_output);
            _powerTube = new PowerTube(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _tlm = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }

        [Test]
        public void OnPowerPressed_StateREADY_OutputIsCalledCorrectly()
        {
            // myState = State.READY (default)

            //_tlm.OnPowerPressed();

            Assert.True(true);
        }


    }
}
