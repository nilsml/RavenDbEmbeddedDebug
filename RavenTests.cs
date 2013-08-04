using System;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Linq;

namespace RavenDBTests
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




        [Test]
        public void Should_give_documents_where_ExpirationDate_is_null_or_expirationdate_greater_than_today()
        {
            using (var documentStore = new EmbeddableDocumentStore
                                           {
                                               RunInMemory = true,
                                               Conventions =
                                                   {
                                                       DefaultQueryingConsistency =
                                                           ConsistencyOptions.QueryYourWrites
                                                   }
                                           })
            {
                documentStore.Initialize();

                using (var session = documentStore.OpenSession())
                {
                    session.Store(new Foo());
                    session.Store(new Foo());
                    session.SaveChanges();
                }
                using (var session = documentStore.OpenSession())
                {
                    var bar =
                        session.Query<Foo>()
                               .Where(foo => foo.ExpirationTime == null || foo.ExpirationTime > DateTime.Now)
                               .ToList();
                    Assert.That(bar.Count, Is.EqualTo(2));
                }
            }
        }
    }
}
