using System.Threading;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Unit
{
    [TestFixture]
    public class TimerTest
    {
        private Timer uut;

        [SetUp]
        public void Setup()
        {
            uut = new Timer();
        }

        [Test]
        public void Start_TimerTick_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            uut.Start(2);

            // wait for a tick, but no longer
            Assert.That(pause.WaitOne(1100));
        }

        [Test]
        public void Start_TimerTick_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();
            uut.Start(2);

            // wait shorter than a tick, shouldn't come
            Assert.That(!pause.WaitOne(900));
        }

        [Test]
        public void Start_TimerExpires_ShortEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);

            // wait for expiration, but not much longer, should come
            Assert.That(pause.WaitOne(2100));
        }

        [Test]
        public void Start_TimerExpires_LongEnough()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();
            uut.Start(2);

            // wait shorter than expiration, shouldn't come
            Assert.That(!pause.WaitOne(1900));
        }

        [Test]
        public void Start_TimerTick_CorrectNumber()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => notifications++;

            uut.Start(2);

            // wait longer than expiration
            Assert.That(pause.WaitOne(2100));

            Assert.That(notifications, Is.EqualTo(2));
        }

        [Test]
        public void Stop_NotStarted_NoThrow()
        {
            Assert.That( () => uut.Stop(), Throws.Nothing);
        }

        [Test]
        public void Stop_Started_NoTickTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.TimerTick += (sender, args) => pause.Set();

            uut.Start(2);
            uut.Stop();

            Assert.That(!pause.WaitOne(1100));
        }

        [Test]
        public void Stop_Started_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);

            uut.Expired += (sender, args) => pause.Set();

            uut.Start(2);
            uut.Stop();

            Assert.That(!pause.WaitOne(2100));
        }

        [Test]
        public void Stop_StartedOneTick_NoExpiredTriggered()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            int notifications = 0;

            uut.Expired += (sender, args) => pause.Set();
            uut.TimerTick += (sender, args) => uut.Stop();

            uut.Start(2);

            Assert.That(!pause.WaitOne(2100));
        }
    }
}