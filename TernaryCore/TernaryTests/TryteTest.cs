using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using TernaryCore;

namespace TernaryTests
{
    [TestFixture]
    public class TryteTest
    {
        [Test]
        public void TryteFromInt()
        {
            int init = 123;
            Tryte tryte = new Tryte(init);
            Assert.AreEqual((int)tryte, init);
        }
    }
}
