using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace RavenDbEmbeddedBug
{
    class Foo
    {
        public Guid Id { get; private set; }
        public DateTime? ExpirationTime { get; set; }

        public Foo()
        {
            Id = Guid.NewGuid();
            ExpirationTime = null;
        }
    }

    [TestFixture]
    public class RavenTests
    {
        private EmbeddableDocumentStore _documentStore;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _documentStore = new EmbeddableDocumentStore
                                 {
                                     RunInMemory = true,
                                     Conventions = { DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites }
                                 };
            _documentStore.Initialize();

            using (var session = _documentStore.OpenSession())
            {
                session.Store(new Foo());
                session.Store(new Foo());
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _documentStore.Dispose();
        }

        [Test]
        public void Should_give_documents_where_ExpirationDate_is_null()
        {
            using (var session = _documentStore.OpenSession())
            {
                var bar = session.Query<Foo>().Where(foo => foo.ExpirationTime == null).ToList();
                Assert.That(bar.Count, Is.EqualTo(2));
            }
        }
    }
}
