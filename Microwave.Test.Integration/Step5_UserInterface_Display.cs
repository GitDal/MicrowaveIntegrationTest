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
    class Step5_UserInterface_Display
    {
        private IOutput _output;
        private ICookController _cookController;
        private IDoor _door;
        private IButton _powerButton, _timeButton, _startCancelButton;
        private ILight _light;
        private IDisplay _display;
        private IUserInterface _tlm;

        [SetUp]
        public void SetUp()
        {
            // Stubs, mocks, fakes.
            _output = Substitute.For<IOutput>();
            _light = Substitute.For<ILight>();
            _cookController = Substitute.For<ICookController>();
            _door = Substitute.For<IDoor>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();


            // Included.
            _display = new Display(_output);
            _tlm = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }

        [Test]
        public void ready_2PowerButton_PowerIs100()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _display.Received(1).ShowPower(Arg.Is<int>(100));
        }



    }

}
