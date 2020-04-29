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
    class Step7_Door_Buttons_UserInterface
    {
        private ITimer _timer;
        private IOutput _output;
        private ICookController _cookController;

        private ILight _light;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private IUserInterface _userInterface;
        private IButton _tlmPowerButton, _tlmTimeButton, _tlmStartCancelButton;
        private IDoor _tlmDoor;

        [SetUp]
        public void SetUp()
        {
            // Stubs, mocks, fakes.
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>();
      

            // Top level modules.
            _tlmPowerButton = new Button();
            _tlmTimeButton = new Button();
            _tlmStartCancelButton = new Button();
            _tlmDoor = new Door();

            // Included.
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _light = new Light(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
           
            _userInterface = new UserInterface(_tlmPowerButton, _tlmTimeButton, _tlmStartCancelButton, _tlmDoor, _display, _light,
                _cookController);
        }

        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            _tlmDoor.Open();
            _tlmDoor.Close();

            _tlmPowerButton.Press();
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 50 W"));
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {
            _tlmPowerButton.Press();
            _tlmPowerButton.Press();
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 100 W"));
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 14; i++)
            {
                _tlmPowerButton.Press();
            }
            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 700 W"));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                _tlmPowerButton.Press();
            }
            _tlmPowerButton.Press();
            _output.Received(2).OutputLine(Arg.Is<string>("Display shows: 50 W"));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            _tlmPowerButton.Press();
            _tlmStartCancelButton.Press();

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            _tlmPowerButton.Press();
            _tlmDoor.Open();

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            _tlmPowerButton.Press();
            _tlmTimeButton.Press();

            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 01:00"));
        }

        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            _tlmPowerButton.Press();
            _tlmTimeButton.Press();
            _tlmTimeButton.Press();

            _output.Received(1).OutputLine(Arg.Is<string>("Display shows: 02:00"));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            _tlmPowerButton.Press();
            _tlmTimeButton.Press();
            _tlmDoor.Open();

            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }

        [Test]
        public void Cooking_CookingIsDone_ClearDisplay()
        {
            _tlmPowerButton.Press();
            _tlmTimeButton.Press();
            _tlmStartCancelButton.Press();

            _userInterface.CookingIsDone();
            _output.Received(1).OutputLine(Arg.Is<string>("Display cleared"));
        }

        [TestCase(1, 50)]
        [TestCase(2, 100)]
        [TestCase(5,250)]
        [TestCase(14, 700)]
        public void Cooking_CookingStarted_PowerTubeOutputCorrect(int powerPresses, int powerUsed)
        {
            for (int i = 0; i < powerPresses; i++)
            {
                _tlmPowerButton.Press();
            }
            _tlmTimeButton.Press();
            _tlmStartCancelButton.Press();

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"PowerTube works with {powerUsed}")));
        }

    }
}