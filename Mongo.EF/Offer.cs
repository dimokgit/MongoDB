using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HedgeHog.MongoDB {
  public class Offer {
    [Key]
    public ObjectId _id { get; set; }
    public String OfferID { get; set; }
    public String Pair { get; set; }
    public int InstrumentType { get; set; }
    public Double Bid { get; set; }
    public Double Ask { get; set; }
    public Double Hi { get; set; }
    public Double Low { get; set; }
    public Double IntrS { get; set; }
    public Double IntrB { get; set; }
    public String ContractCurrency { get; set; }
    public int ContractSize { get; set; }
    public int Digits { get; set; }
    public int DefaultSortOrder { get; set; }
    public Double PipCost { get; set; }
    public Double MMRLong { get; set; }
    double _MMRShort = double.NaN;
    public double MMRShort {
      get { return double.IsNaN(_MMRShort) ? MMRLong : _MMRShort; }
      set {
        _MMRShort = value;
      }
    }
    public DateTime Time { get; set; }
    public int BidChangeDirection { get; set; }
    public int AskChangeDirection { get; set; }
    public int HiChangeDirection { get; set; }
    public int LowChangeDirection { get; set; }
    public String QuoteID { get; set; }
    public String BidID { get; set; }
    public String AskID { get; set; }
    public DateTime BidExpireDate { get; set; }
    public DateTime AskExpireDate { get; set; }
    public String BidTradable { get; set; }
    public String AskTradable { get; set; }
    public Double PointSize { get; set; }
  }

}
