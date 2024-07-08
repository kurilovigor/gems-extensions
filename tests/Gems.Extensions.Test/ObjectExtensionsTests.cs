namespace Gems.Extensions.Test
{
    public class ObjectExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNullObject()
        {
            var obj = null as object;
            var dict = obj.ToDictionary();

            Assert.That(dict, Is.Not.Null);
            Assert.That(dict.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestEmptyObject()
        {
            var obj = new object();
            var dict = obj.ToDictionary();

            Assert.That(dict, Is.Not.Null);
            Assert.That(dict.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestObject()
        {
            var obj = new 
            {
                Num = 12345,
                Str = "Hello",
                List = new List<string> { "ListItem" },
                Null = null as object,
            };
            var dict = obj.ToDictionary();

            Assert.That(dict, Is.Not.Null);
            Assert.That(dict.Count, Is.EqualTo(4));
            Assert.That(dict["Num"], Is.EqualTo(12345));
            Assert.That(dict["Str"], Is.EqualTo("Hello"));
            Assert.That((dict["List"] as List<string>)?.FirstOrDefault(), Is.EqualTo("ListItem"));
            Assert.That(dict["Null"], Is.Null);
        }
    }
}