using HedgeHog.MongoDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongo {
  class Program {
    static void Main(string[] args) {
      using(var db = new ForexDbContext()) {
        var trades = db.Trades.ToArray();
        var openDate = DateTime.Parse("01/04/2018 9:35:48");
        db.Trades.Add(new Trade("DImok", 1, 2, 10, 20, 100, openDate, openDate.AddSeconds(60 * 10 + 19),48));
        db.SaveChanges();
        foreach(var o in db.Trades) {
          Console.WriteLine(new { o.Symbol, o.OpenPrice, o.ClosePrice, o.TimeOpen, o.TimeClose,o.PL } + "");
        }
        Console.ReadKey();
        return;
        db.ToDos.Add(new ToDo { What = "Debug", When = DateTime.UtcNow });
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
          toDos.First(x => x.What == "Debug").What = "Test " + DateTime.Now.Second;
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

        //var tms = db.TradingMacroSettings.ToArray();
        //db.TradingMacroSettings.Add(new TradingMacroSettings { Pair = "Dimok", PairIndex = 1 });
        //db.SaveChanges();
        //foreach(var o in tms) {
        //  Console.WriteLine(new { o.Pair, o.PairIndex } + "");
        //}
      }
    }
  }
}
