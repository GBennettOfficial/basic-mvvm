using BasicMvvm.Tests.Examples;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMvvm.Tests
{
    public class SharedFuncsTests
    {
        [Fact]
        public void GetPropertyName_ReturnsCorrectName()
        {
            string propertyName = SharedFuncs.GetPropertyNameFromFunc<LoginVm, string>(x => x.Username);
            Assert.Equal("Username", propertyName);
        }

        [Fact]
        public void GetSelfName_ReturnsEmptyString()
        {
            string propertyName = SharedFuncs.GetPropertyNameFromFunc<LoginVm, LoginVm>(x => x);
            Assert.Equal("", propertyName);
        }
    }
}
