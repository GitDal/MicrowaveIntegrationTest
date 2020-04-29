using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step4_CookController_Display_Timer_PowerTube
    {
        private IOutput _output;
        private IUserInterface _ui;
        private ITimer _timer;
        private Display _display;
        private CookController _tlm;
        private PowerTube _powerTube;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _ui = Substitute.For<IUserInterface>();
            _timer = Substitute.For<ITimer>();
            _display = new Display(_output);
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

        /*
        [TestCase(50, 1)]
        [TestCase(50, 30)]
        [TestCase(50, 60)]
        [TestCase(50, 90)]
        [TestCase(50, 120)]
        [TestCase(50, 125)]
        public void StartCooking_CorrectPower_OutputReceivesFromDisplay(int power, int time)
        {
            int minutes = time / 60;
            int seconds = time - (minutes * 60);

            _tlm.StartCooking(power, time);
            _timer.Start(time);
            _output.Received().OutputLine(Arg.Is<string>(str => str == $"Display shows: {minutes:D2}:{seconds:D2}"));
        }*/



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

    [TestFixture]
    public class Step4_CookController_Timer
    {
        private IOutput _output;
        private IPowerTube _powerTube;
        private IUserInterface _ui;
        private IDisplay _display;
        private Timer _timer;
        private CookController _tlm;
        

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();
            _ui = Substitute.For<IUserInterface>();
            _display = Substitute.For<IDisplay>();
            _powerTube = Substitute.For<IPowerTube>();

            _timer = new Timer();
            _tlm = new CookController(_timer, _display, _powerTube, _ui);
        }

        [TestCase(50, 1000)]
        [TestCase(50, 2000)]
        [TestCase(50, 3000)]

        public void StartCooking_TimeOneSecondOrLonger_DisplayReceivesNumberOfCallsFromCookController(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time + 500);

            _display.Received(time/1000).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestCase(50, 1000)]
        [TestCase(50, 2000)]
        [TestCase(50, 3000)]

        public void StartCooking_TimeOneSecondOrLonger_PowerTubeTurnOffOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time + 500);

            _powerTube.Received(1).TurnOff();
        }


        [TestCase(50, 1000)]
        [TestCase(50, 2000)]
        [TestCase(50, 3000)]

        public void StartCooking_TimeOneSecondOrLonger_UICookingIsDoneOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time + 500);

            _ui.Received(1).CookingIsDone();
        }


        [TestCase(50, 999)]
        [TestCase(50, 500)]
        [TestCase(50, 1)]
        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -1000)]
        [TestCase(50, -2000)]

        public void StartCooking_UnderOneSecond_DisplayReceivesNumberOfCallsFromCookController(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestCase(50, 999)]
        [TestCase(50, 500)]
        [TestCase(50, 1)]
        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -1000)]
        [TestCase(50, -2000)]

        public void StartCooking_UnderOneSecond_PowerTubeTurnedOffOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _powerTube.Received(1).TurnOff();
        }

        [TestCase(50, 999)]
        [TestCase(50, 500)]
        [TestCase(50, 1)]
        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -1000)]
        [TestCase(50, -2000)]

        public void StartCooking_UnderOneSecond_UICookingIsDoneOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _ui.Received(1).CookingIsDone();
        }

        [Test]
        public void StopCooking_AfterTwoSeconds_DisplayReceivedTwoCall()
        {
            _tlm.StartCooking(50, 3000);
            
            Thread.Sleep(2100);

            _tlm.Stop();

            _display.Received(2).ShowTime(Arg.Any<int>(),Arg.Any<int>());
        }

        [Test]
        public void StopCooking_AfterTwoSeconds_PowerTubeTurnedOff()
        {
            _tlm.StartCooking(50, 3000);

            Thread.Sleep(2100);

            _tlm.Stop();

            _powerTube.Received(1).TurnOff();
        }



    }
}