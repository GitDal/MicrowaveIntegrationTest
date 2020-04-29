using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Step4b_CookController_Timer
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

            Thread.Sleep(time * 1000 + 500);

            _display.Received(time).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [TestCase(50, 1)]
        [TestCase(50, 2)]
        [TestCase(50, 3)]

        public void StartCooking_TimeOneSecondOrLonger_PowerTubeTurnOffOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time * 1000 + 500);

            _powerTube.Received(1).TurnOff();
        }


        [TestCase(50, 1)]
        [TestCase(50, 2)]
        [TestCase(50, 3)]

        public void StartCooking_TimeOneSecondOrLonger_UICookingIsDoneOnce(int power, int time)
        {
            _tlm.StartCooking(power, time);

            Thread.Sleep(time * 1000 + 500);

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

            _display.Received(2).ShowTime(Arg.Any<int>(), Arg.Any<int>());
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
