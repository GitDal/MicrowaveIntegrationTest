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

            // Top level module.
            _tlm = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }

        // Similar tests to UserInterface unit tests.
        // This time using real a display class and asserting on the output substitute.
        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);

            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 50 W"));
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 100 W"));
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 14; i++)
            {
                _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 700 W"));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _output.Received(2).OutputLine(Arg.Is<string>("Display shows: 50 W"));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }
      
        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 01:00"));
        }
        
        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 02:00"));
        }
        
        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }
        
        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _tlm.CookingIsDone();
            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }
    }
}
