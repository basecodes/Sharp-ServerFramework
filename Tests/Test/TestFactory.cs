using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Ssc.SscFactory;
using System.Linq;

namespace Test {

    public class User {
        public string Name { get; set; }
    }
    [TestFixture]
    class TestFactory {

        [Test]
        public void TestClone() {
            var user = new User() {
                Name = "ABC123"
            };

            var clone = user.DeepCopyByExpressionTree();
            Assert.That(clone.Name, Is.EqualTo(user.Name));
        }

        [Test]
        public void TestObjectActivator() {
            var obj = ObjectFactory.GetActivator<User>(typeof(User).GetConstructors().First())();
            Assert.That(obj.GetType(), Is.EqualTo(typeof(User)));
        }
    }
}
