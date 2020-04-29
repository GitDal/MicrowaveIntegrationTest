using System;
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
    public class Step4_CookController_Display_PowerTube
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

        [TestCase(1,5)]
        [TestCase(50, 5)]
        [TestCase(100, 5)]
        [TestCase(350, 5)]
        [TestCase(650, 5)]
        [TestCase(699, 5)]
        [TestCase(700, 5)]
        public void StartCooking_CorrectPower_OutputReceivesFromPowerTube(int power, int time)
        {
            _tlm.StartCooking(power, time);

            _output.Received().OutputLine(Arg.Is<string>(str => str == $"PowerTube works with {power}"));
        }

        [Test]
        public void StartCooking_CorrectTimeAndOneTimeTick_OutputReceivesFromDisplay()
        {
            _timer.TimeRemaining.Returns(65); //One minute and 5 seconds

            _tlm.StartCooking(50, 66);

            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine($"Display shows: {1:D2}:{5:D2}");
        }

        [Test]
        public void TimeTick_CorrectTimeAndOneTimeTick_OutputReceivesFromDisplay()
        {
            _timer.TimeRemaining.Returns(65); //One minute and 5 seconds

            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine($"Display shows: {1:D2}:{5:D2}");
        }

        [Test]
        public void StartCooking_ZeroTimeOneTimeTick_OutputDoesNotReceiveFromDisplay()
        {
            _timer.TimeRemaining.Returns(0); //0 seconds

            _tlm.StartCooking(50, 0);

            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            _output.DidNotReceive().OutputLine($"Display shows: {1:D2}:{5:D2}");
        }

        [Test]
        public void StartCooking_TimerExpired_OutputReceivesFromPowerTube()
        {
            _tlm.StartCooking(50, 120);

            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine($"PowerTube turned off");
        }

        [Test]
        public void NoStartCooking_TimerExpired_OutputDoesNotReceiveFromPowerTube()
        {
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.DidNotReceive().OutputLine($"PowerTube turned off");
        }

        [TestCase(0, 5)]
        [TestCase(-1, 5)]
        [TestCase(-100, 5)]
        [TestCase(701, 5)]
        [TestCase(750, 5)]
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

        [TestCase(50, 1)]
        [TestCase(50, 2)]
        [TestCase(50, 3)]

        public void StartCooking_TimeOneSecondOrLonger_DisplayReceivesNumberOfCallsFromCookController(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time*1000 + 500);

            _display.Received(time).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestCase(50, 1)]
        [TestCase(50, 2)]
        [TestCase(50, 3)]

        public void StartCooking_TimeOneSecondOrLonger_PowerTubeTurnOffOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time*1000 + 500);

            _powerTube.Received(1).TurnOff();
        }


        [TestCase(50, 1)]
        [TestCase(50, 2)]
        [TestCase(50, 3)]

        public void StartCooking_TimeOneSecondOrLonger_UICookingIsDoneOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time*1000 + 500);

            _ui.Received(1).CookingIsDone();
        }


        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -5)]
        [TestCase(50, -10)]

        public void StartCooking_UnderOneSecond_DisplayReceivesOneCallFromCookController(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -5)]
        [TestCase(50, -10)]

        public void StartCooking_UnderOneSecond_PowerTubeTurnedOffOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _powerTube.Received(1).TurnOff();
        }

        [TestCase(50, 0)]
        [TestCase(50, -1)]
        [TestCase(50, -5)]
        [TestCase(50, -10)]

        public void StartCooking_UnderOneSecond_UICookingIsDoneOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(1500);

            _ui.Received(1).CookingIsDone();
        }

        [Test]
        public void StopCooking_AfterTwoSeconds_DisplayReceivedTwoCall()
        {
            _tlm.StartCooking(50, 3);
            
            Thread.Sleep(2100);

            _tlm.Stop();

            _display.Received(2).ShowTime(Arg.Any<int>(),Arg.Any<int>());
        }

        [Test]
        public void StopCooking_AfterTwoSeconds_PowerTubeTurnedOff()
        {
            _tlm.StartCooking(50, 3);

            Thread.Sleep(2100);

            _tlm.Stop();

            _powerTube.Received(1).TurnOff();
        }
    }
}