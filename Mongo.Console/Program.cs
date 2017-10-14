using HedgeHog.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo {
  class Program {
    static void Main(string[] args) {
      using(var db = new ForexDbContext()) {
        db.ToDos.Add(new ToDo { What = "Debug", When = DateTime.UtcNow});
        try {
          db.SaveChanges();
          db.ToDos.First(x => x.What == "Debug").What = "Test";
          db.SaveChanges();
          var toDos = db.ToDos.ToArray();
          foreach(var td in toDos) {
            Console.WriteLine($"{td.When.ToLocalTime().TimeOfDay}: {td.What}");
          }
          db.RemoveRange(toDos.Reverse().Skip(4));
          db.SaveChanges();

          toDos = db.ToDos.ToArray();
          toDos.First(x => x.What == "Debug").What = "Test "+DateTime.Now.Second;
          db.SaveChanges();
          foreach(var td in db.ToDos) {
            Console.WriteLine($"{td.When.ToLocalTime().TimeOfDay}: {td.What}");
          }

        } catch(Exception exc) {
          Console.WriteLine(exc + "");
        }

        //db.Offer.Add(new Offer { OfferID = 1 });
        //db.SaveChanges();
        var offers = db.Offer.ToArray();
        foreach(var o in offers) {
          Console.WriteLine(new { o.Pair, o.MMRLong } + "");
        }

        Console.ReadKey();
      }
    }
  }
}
