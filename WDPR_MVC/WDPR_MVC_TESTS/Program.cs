using System;
using Microsoft.EntityFrameworkCore;
using WDPR_MVC.Data;

namespace WDPR_MVC_TESTS
{
    class Program
    {
        private string databaseName; // zonder deze property kun je geen clean context maken.

        private MyContext GetInMemoryDBMetData()
        {
            MyContext context = GetNewInMemoryDatabase(true);

            //Replace with other info
            //context.Add(new Student { Id = 1, Name = "Jan", Leeftijd = 58 });

            context.SaveChanges();
            return GetNewInMemoryDatabase(false); // gebruik een nieuw (clean) object voor de context
        }

        private MyContext GetNewInMemoryDatabase(bool NewDb)
        {
            if (NewDb) this.databaseName = Guid.NewGuid().ToString(); // unieke naam

            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(this.databaseName)
                .Options;

            return new MyContext(options);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello melding tester!");
        }
    }
}
