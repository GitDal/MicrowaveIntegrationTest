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
            ((CookController)_cookController).UI = _tlm; // Property injection
        }

        /* STATES: { READY, SETPOWER, SETTIME, COOKING, DOOROPEN } */

        [Test]
        public void OnPowerPressed_StateIsREADY_OutputIsCalledCorrectly()
        {
            // myState = State.READY (default)
            int powerLevel = 50; //default

            // Act:
            _tlm.OnPowerPressed(new object(), new EventArgs());

            // Assert:
            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.Contains(powerLevel.ToString()) 
                && str.Contains("W"))
            );
        }

        //(powerLevel >= 700 ? 50 : powerLevel + 50)
        [TestCase(1, 100)]
        [TestCase(13, 700)]
        [TestCase(14, 50)]
        public void OnPowerPressed_StateIsSETPOWER_OutputShowsCorrectPower(int timesPressed, int expectedPowerLevel)
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> myState = State.SETPOWER

            for(int i = 0; i < timesPressed-1; i++)
                _tlm.OnPowerPressed(new object(), new EventArgs()); // Press button timesPressed-times

            _output.ClearReceivedCalls(); //Clear received calls (power is still correct)

            // Act:
            _tlm.OnPowerPressed(new object(), new EventArgs());

            // Assert:
            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.Contains(expectedPowerLevel.ToString()))
            );
        }

        [Test]
        public void OnTimePressed_StateIsSETPOWER_OutputShowsCorrectTime()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> myState = State.SETPOWER
            int expectedTimeInMins = 1; //default

            // Act:
            _tlm.OnTimePressed(new object(), new EventArgs());

            // Assert:
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
                str.Contains(expectedTimeInMins.ToString("D2")) 
                && str.Contains(0.ToString("D2")))
            );
        }

        [TestCase(1, 2)]
        [TestCase(10, 11)]
        [TestCase(1000, 1001)]
        public void OnTimePressed_StateIsSETTIMEAndTimeIsOne_OutputShowsCorrectTime(int timePressed, int expectedTimeInMins)
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME

            for (int i = 0; i < timePressed-1; i++)
                _tlm.OnTimePressed(new object(), new EventArgs());

            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnTimePressed(new object(), new EventArgs()); //Newest output

            // Assert:
            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains(expectedTimeInMins.ToString("D2"))
                && str.Contains(0.ToString("D2")))
            );
        }

        [Test]
        public void OnStartCancelPressed_StateIsSETPOWER_OutputShowsDisplayIsCleared()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.READY

            // Assert:
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("cleared"))
            );
        }

        [Test]
        public void OnStartCancelPressed_StateIsSETTIME_OutputShowsLightIsOnAndPowerTubeIsOn()
        {
            int powerLevel = 50; //default

            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.COOKING

            // Assert:
            Assert.Multiple((() =>
            {
                _output.Received().OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("on")));

                _output.Received().OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains(powerLevel.ToString()))
                );
            }));
            
        }

        [Test]
        public void OnStartCancelPressed_StateIsCOOKING_OutputShowsPowerTubeOffLightOffAndDisplayClear()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.COOKING
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.READY

            // Assert:
            Assert.Multiple((() =>
            {
                _output.Received().OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("powertube")
                    && str.ToLower().Contains("off"))
                );

                _output.Received().OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("light") && 
                    str.ToLower().Contains("off"))
                );

                _output.Received().OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display") &&
                    str.ToLower().Contains("clear"))
                );
            }));
        }

        [Test]
        public void OnDoorOpened_StateIsREADY_OutputShowsLightOn()
        {
            // Act:
            _tlm.OnDoorOpened(new object(), new EventArgs()); //--> state = state.DOOROPEN

            // Assert:
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
                str.ToLower().Contains("light") 
                && str.ToLower().Contains("on"))
            );
        }

        [Test]
        public void OnDoorOpened_StateIsSETPOWER_OutputShowsLightOnAndDisplayClear()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnDoorOpened(new object(), new EventArgs()); //--> state = state.DOOROPEN

            // Assert:
            Assert.Multiple((() =>
            {
                _output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("light")
                    && str.ToLower().Contains("on"))
                );

                _output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display")
                    && str.ToLower().Contains("clear"))
                );
            }));
        }

        [Test]
        public void OnDoorOpened_StateIsSETTIME_OutputShowsLightOnAndDisplayClear()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnDoorOpened(new object(), new EventArgs()); //--> state = state.DOOROPEN

            // Assert:
            Assert.Multiple((() =>
            {
                _output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("light")
                    && str.ToLower().Contains("on"))
                );

                _output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display")
                    && str.ToLower().Contains("clear"))
                );
            }));
        }

        [Test]
        public void OnDoorOpened_StateIsCOOKING_OutputShowsPowerTubeOff()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.COOKING
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnDoorOpened(new object(), new EventArgs()); //--> state = State.READY

            // Assert:
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube")
                && str.ToLower().Contains("off")));
        }

        [Test]
        public void OnDoorClosed_StateIsDOOROPEN_OutputShowsLightIsOff()
        {
            // Arrange:
            _tlm.OnDoorOpened(new object(), new EventArgs()); //--> state = State.DOOROPEN
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.OnDoorClosed(new object(), new EventArgs()); //--> state = state.DOORCLOSED

            // Assert:
            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light")
                && str.ToLower().Contains("off"))
            );
        }

        [Test]
        public void CookingIsDone_StateIsCOOKING_OutputShowsDisplayClearAndLightOff()
        {
            // Arrange:
            _tlm.OnPowerPressed(new object(), new EventArgs()); //--> state = State.SETPOWER
            _tlm.OnTimePressed(new object(), new EventArgs()); //--> state = State.SETTIME
            _tlm.OnStartCancelPressed(new object(), new EventArgs()); //--> state = State.COOKING
            _output.ClearReceivedCalls(); // Clear output

            // Act:
            _tlm.CookingIsDone();

            // Assert:
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display")
                && str.ToLower().Contains("clear")));
        }

    }
}
