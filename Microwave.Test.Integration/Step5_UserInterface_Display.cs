using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    class Step5_UserInterface_Display
    {
        private IOutput _output;
        private ICookController _cookController;
        private IDoor _door;
        private IButton _button;
        private IUserInterface _uut;


        [SetUp]
        public void SetUp()
        {
            _output = new Substitute.For(IOutput);
            
        }
    }
}
