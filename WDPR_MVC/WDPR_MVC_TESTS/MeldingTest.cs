using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using WDPR_MVC.Data;
using System.Linq;

namespace WDPR_MVC_TESTS
{
    public class MeldingTest : BaseTest
    {
        [Fact]
        public void Test1()
        {
            var FoundUser = GetInMemoryDBMetData().Users.Any(u => u.Email == "test1@email.com");
            Assert.True(FoundUser);
        }
    }
}
