using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using System;
using Blueshift.EntityFrameworkCore.MongoDB.Infrastructure;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.Serialization;
/// <summary>
/// https://www.myget.org/feed/efcore-mongodb/package/nuget/Blueshift.EntityFrameworkCore.MongoDB
/// </summary>
namespace HedgeHog.MongoDB {
  [MongoDatabase("forex")]
  public class ForexDbContext :DbContext {
    private string _connectionString;
    private MongoUrl _mongoUrl;

    //public DbSet<Animal> Animals { get; set; }
    //public DbSet<Employee> Employees { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<Offer> Offer { get; set; }
    DbSet<TraderModelPersist> TraderModelPersist { get; set; }
    //public DbSet<Trade> Trades { get; set; }

    public ForexDbContext(string connectionString):this() {
      _connectionString = connectionString;
    }
    public ForexDbContext() : this(new DbContextOptions<ForexDbContext>()) {
      if(string.IsNullOrWhiteSpace(_connectionString))
        _connectionString= "mongodb://dimok:1Aaaaaaa@ds040017.mlab.com:40017/forex";
    }

    public ForexDbContext(DbContextOptions<ForexDbContext> dbContextOptions)
        : base(dbContextOptions) {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      //_connectionString = "mongodb://dimok:1Aaaaaaa@ds040017.mlab.com:40017/forex1";
      //optionsBuilder.UseMongoDb(connectionString);

      _mongoUrl = new MongoUrl(_connectionString);
      //optionsBuilder.UseMongoDb(mongoUrl);

      MongoClientSettings settings = MongoClientSettings.FromUrl(_mongoUrl);
      //settings.SslSettings = new SslSettings
      //{
      //    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
      //};
      //optionsBuilder.UseMongoDb(settings);

      MongoClient mongoClient = new MongoClient(settings);
      optionsBuilder.UseMongoDb(mongoClient);
    }
    public override int SaveChanges() {
      try {
        return base.SaveChanges();
      }catch(Exception exc) {
        throw new Exception(new { _mongoUrl.DatabaseName } + "", exc);
      }
    }
  }

  [BsonKnownTypes(typeof(Tiger), typeof(PolarBear), typeof(Otter))]
  [BsonDiscriminator(Required = true, RootClass = true)]
  public abstract class Animal {
    [BsonId]
    public ObjectId Id { get; private set; }
    public string Name { get; set; }
    public double Age { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
  }

  [BsonDiscriminator("panthera tigris")]
  public class Tiger :Animal { }

  [BsonDiscriminator("Ursus maritimus")]
  public class PolarBear :Animal { }

  [BsonDiscriminator("Lutrinae")]
  [BsonKnownTypes(typeof(SeaOtter), typeof(EurasianOtter))]
  public abstract class Otter :Animal { }

  [BsonDiscriminator("Enhydra lutris")]
  public class SeaOtter :Otter { }

  [BsonDiscriminator("Lutra lutra")]
  public class EurasianOtter :Otter { }

  public class Employee {
    [Key]
    public ObjectId Id { get; private set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [BsonIgnore]
    public string FullName => string.IsNullOrWhiteSpace(FirstName)
        ? LastName
        : $"{LastName}, {FirstName}";

    public double Age { get; set; }
    public List<Specialty> Specialties { get; set; } = new List<Specialty>();
  }

  public class ToDo {
    [Key]
    public ObjectId Id { get; private set; }
    public string What { get; set; }
    //[BsonDateTimeOptions(Representation = BsonType.String)]
    public DateTime When { get; set; }
  }

  public enum ZooTask {
    Feeding,
    Training,
    Exercise,
    TourGuide
  }

  [ComplexType]
  public class Specialty {
    public string AnimalType { get; set; }
    public ZooTask Task { get; set; }
  }
  public class ModelBase :INotifyPropertyChanged {
    [System.ComponentModel.DataAnnotations.Key]
    public string _key { get; set; }
    #region INotifyPropertyChanged Members

    #region PropertyChanged Event
    event PropertyChangedEventHandler PropertyChangedEvent;
    public event PropertyChangedEventHandler PropertyChanged {
      add {
        if(PropertyChangedEvent == null || !PropertyChangedEvent.GetInvocationList().Contains(value))
          PropertyChangedEvent += value;
      }
      remove {
        PropertyChangedEvent -= value;
      }
    }
    #endregion

    ~ModelBase() {
      if(PropertyChangedEvent != null)
        PropertyChangedEvent.GetInvocationList().Cast<PropertyChangedEventHandler>().ToList().ForEach(d => PropertyChangedEvent -= d);
    }
    protected void RaisePropertyChanged(params Expression<Func<object>>[] propertyLamdas) {
      if(propertyLamdas == null || propertyLamdas.Length == 0) RaisePropertyChangedCore();
      else
        foreach(var pl in propertyLamdas) {
          RaisePropertyChanged(pl);
        }
    }
    protected void OnPropertyChanged(string propertyName) {
      RaisePropertyChanged(propertyName);
    }
    protected void RaisePropertyChanged(string propertyName) {
      RaisePropertyChangedCore(propertyName);
    }
    protected void RaisePropertyChangedCore(params string[] propertyNames) {
      if(PropertyChangedEvent == null) return;
      if(propertyNames.Length == 0)
        propertyNames = new[] { new StackFrame(1).GetMethod().Name.Substring(4) };
      foreach(var pn in propertyNames)
        //Application.Current.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => {
        PropertyChangedEvent(this, new PropertyChangedEventArgs(pn));
      //}));
    }
    #endregion
  }
  class TraderModelPersist :ModelBase {
    public static string CurrentDirectory() => System.Net.Dns.GetHostName().ToLower() + "::" 
      ;

    public TraderModelPersist() {
      _key = CurrentDirectory();
    }
    //public ObjectId _id { get; set; }
    private string _TradingMacroName;
    public string TradingMacroName {
      get { return _TradingMacroName; }
      set {
        if(_TradingMacroName != value) {
          _TradingMacroName = value;
          RaisePropertyChangedCore();
        }
      }
    }

    private int _IpPort;
    public int IpPort {
      get { return _IpPort; }
      set {
        if(_IpPort != value) {
          _IpPort = value;
          RaisePropertyChangedCore();
        }
      }
    }
    bool _IsInVirtualTrading;
    public bool IsInVirtualTrading {
      get => _IsInVirtualTrading;
      set {
        if(_IsInVirtualTrading != value) {
          _IsInVirtualTrading = value;
          RaisePropertyChangedCore();
        }
      }
    }

    string _IB_IPAddress = "127.0.0.1";
    public string IB_IPAddress {
      get => _IB_IPAddress;
      set {
        if(_IB_IPAddress != value) {
          _IB_IPAddress = value;
          RaisePropertyChangedCore();
        }
      }
    }
    int _IB_IPPort = 7497;
    public int IB_IPPort {
      get => _IB_IPPort;
      set {
        if(_IB_IPPort != value) {
          _IB_IPPort = value;
          RaisePropertyChangedCore();
        }
      }
    }
    int _IB_ClientId = 0;
    public int IB_ClientId {
      get => _IB_ClientId;
      set {
        if(_IB_ClientId != value) {
          _IB_ClientId = value;
          RaisePropertyChangedCore();
        }
      }
    }

    public double GrossToExitSave {
      get => GrossToExit < 1 ? GrossToExit : 0;
      set {
        if(GrossToExit != value) {
          GrossToExit = value;
        }
      }
    }
    double _GrossToExit = 0;
    [BsonIgnore]
    public double GrossToExit {
      get => _GrossToExit;
      set {
        if(_GrossToExit != value) {
          _GrossToExit = value;
          RaisePropertyChangedCore();
        }
      }
    }
    double _profitByHedgeRatioDiff = 1 / 3.0;
    public double ProfitByHedgeRatioDiff {
      get => _profitByHedgeRatioDiff;
      set {
        if(_profitByHedgeRatioDiff != value) {
          _profitByHedgeRatioDiff = value;
          RaisePropertyChangedCore();
        }
      }
    }

  }

  [Serializable]
  [DataContract]
  [BsonIgnoreExtraElements]
  public class Trade :PositionBase {
    /// <summary>
    /// Not Implemented exception
    /// </summary>
    public static Func<double> PipRateNI = () => { throw new NotImplementedException(); };
    private Trade() {
    }

    [DataMember]
    [DisplayName("BS")]
    public bool Buy { get; set; }
    [DataMember]
    [DisplayName("")]
    public bool IsBuy { get; set; }
    //[DataMember]
    //[DisplayName("")]
    //[DisplayFormat(DataFormatString = "{0}")]
    //public TradeRemark Remark { get; set; }
    double _Open;
    [DataMember]
    [DisplayName("")]
    public double Open {
      get { return _Open; }
      set {
        _Open = value;
      }
    }
    double _Close;
    [DataMember]
    [DisplayName("")]
    
    public double Close {
      get { return _Close; }
      set {
        _Close = value;
        if(BaseUnitSize == 0)
          return;
        GrossPL = CalcGrossPL(Close);
      }
    }
    public double CalcGrossPL(double close) {
      var gross = Buy ? close - Open : Open - close;
      PL = gross / PipSize;
      var offset = Pair == "USDOLLAR" ? 1 : 10.0;
      return 0;

    }
    [DataMember]
    [DisplayName("")]
    public double Limit { get; set; }
    [DisplayName("")]
    public double LimitInPips { get { return Limit == 0 ? 0 : InPips(IsBuy ? Limit - Open : Open - Limit); } }
    [DisplayName("")]
    public double LimitToCloseInPips { get { return Limit == 0 ? 0 : InPips(IsBuy ? Limit - Close : Close - Limit); } }
    #region Stop
    private double _Stop;
    [DisplayName("")]
    [DataMember]
    public double Stop {
      get { return _Stop; }
      set {
        if(_Stop != value) {
          _Stop = value;
          OnPropertyChanged("Stop");
        }
      }
    }

    #endregion
    [DisplayName("")]
    public double StopInPips { get { return Stop == 0 ? 0 : InPips(IsBuy ? Stop - Open : Open - Stop); } }
    [DisplayName("")]
    public double StopToCloseInPips { get { return Stop == 0 ? 0 : InPips(IsBuy ? Stop - Close : Close - Stop); } }
    [DataMember]
    public double PL { get; set; }

    double _GrossPL;
    [DataMember]
    [DisplayName("")]
    public double GrossPL {
      get { return _GrossPL; }
      set {
        _GrossPL = value;
      }
    }
    private DateTime _time2;
    [DataMember]
    [DisplayFormat(DataFormatString = "{0:dd HH:mm}")]
    public DateTime Time2 {
      get { return _time2; }
      set {
        if(value.Kind == DateTimeKind.Unspecified)
          throw new ArgumentException(new { Time2 = new { value.Kind } } + "");
        _time2 = value;
        Time = _time2.Kind != DateTimeKind.Local ? TimeZoneInfo.ConvertTimeFromUtc(_time2, TimeZoneInfo.Local) : _time2;
      }
    }
    public void SetTime(DateTime time) {
      Time = time;
    }
    private DateTime _time2Close;
    [DataMember]
    [DisplayFormat(DataFormatString = "{0:dd HH:mm}")]
    public DateTime Time2Close {
      get { return _time2Close; }
      set {
        if(value.Kind == DateTimeKind.Unspecified)
          throw new ArgumentException(new { Time2Close = new { value.Kind } } + "");
        _time2Close = value;
        TimeClose = _time2Close.Kind != DateTimeKind.Local ? TimeZoneInfo.ConvertTimeFromUtc(_time2Close, TimeZoneInfo.Local) : _time2Close;
      }
    }
    DateTime _TimeClose;
    [DataMember]
    [DisplayFormat(DataFormatString = "{0:dd HH:mm}")]
    [DisplayName("Time Close")]
    [BsonIgnore]
    public DateTime TimeClose {
      get { return _TimeClose; }
      set { _TimeClose = value; }
    }
    public DateTime DateClose { get { return TimeClose.Date; } }
    public int DaysSinceClose { get { return (int)Math.Floor((DateTime.Now - TimeClose).TotalDays); } }
    [DataMember]
    public int Lots {
      get => _lots;
      set {
        if(_lots == value) return;
        _lots = value;
        OnPropertyChanged(nameof(Lots));
      }
    }
    public double Position => IsBuy ? Lots : -Lots;
    public int AmountK { get { return Lots / (BaseUnitSize == 0 ? 1000 : BaseUnitSize); } }

    [DataMember]
    public string OpenOrderID { get; set; }
    [DataMember]
    public string OpenOrderReqID { get; set; }
    [DataMember]
    public string StopOrderID { get; set; }
    [DataMember]
    public string LimitOrderID { get; set; }


    double _commission = double.NaN;
    [DataMember]
    public double Commission {
      get {
        return double.IsNaN(_commission) ? (CommissionByTrade?.Invoke(this) ?? 0) : _commission;
      }
      set { _commission = value; }
    }

    [DataMember]
    public bool IsVirtual { get; set; }

    public bool IsClosed() => Kind == PositionKind.Closed;
    public void CloseTrade() {
      Kind = PositionKind.Closed;
    }

    
    public int BaseUnitSize { get; private set; }
    Func<Trade, double> _commissionByTrade;
    private int _lots;

    public Func<Trade, double> CommissionByTrade {
      get { return _commissionByTrade; }
      set { _commissionByTrade = value; }
    }
    public double NetPL => GrossPL - (CommissionByTrade == null ? Commission : CommissionByTrade(this));
    public double NetPL2 => GrossPL - (CommissionByTrade == null ? Commission : CommissionByTrade(this) * 2);
    public double CalcNetPL2(double close) => CalcGrossPL(close) - (CommissionByTrade == null ? Commission : CommissionByTrade(this) * 2);

    public double NetPLInPips { get { return InPips(NetPL); } }
    public double OpenInPips { get { return InPips(this.Open); } }
    public double CloseInPips { get { return InPips(this.Close); } }

    public bool IsParsed { get; set; }

    /// <summary>
    /// 100,10000
    /// </summary>
    public int PipValue { get { return (int)Math.Round(Math.Abs(this.PL / (this.Open - this.Close)), 0); } }

    public double InPips(double value) { return value * PipValue; }

    public Trade Clone() {
      var t = this.MemberwiseClone() as Trade;
      return t;
    }

  }

  [Serializable]
  [DataContract]
  [BsonIgnoreExtraElements]
  public abstract class PositionBase :INotifyPropertyChanged {
    [Key]
    [DataMember]
    public string Id { get; set; }
    [DataMember]
    public string Pair { get; set; }
    private DateTime _time;
    [DataMember]
    [DisplayFormat(DataFormatString = "{0:dd HH:mm}")]
    [BsonIgnore]
    public virtual DateTime Time {
      get => _time;
      set {
        _time = value;
        Id = Pair + ":" + value.ToEpochTime() + "";
      }
    }
    public enum PositionKind { Unknown, Open, Closed };
    [DataMember]
    public PositionKind Kind { get; set; }
    [DataMember]
    public string KindString { get { return Kind + ""; } }
    double _PipSize;
    public double PipSize {
      get { return _PipSize; }
      set {
        _PipSize = value;
      }
    }
    [DataMember]
    public double PipCost { get; set; }

    [DataMember]
    /// <summary>
    /// 2,4
    /// </summary>
    public double PointSize { get; set; }
    public string PointSizeFormat { get { return "n" + PointSize; } }

    private double _StopAmount;

    [DataMember]
    public double StopAmount {
      get { return _StopAmount; }
      set {
        if(_StopAmount != value) {
          _StopAmount = value;
          OnPropertyChanged("StopAmount");
        }
      }
    }

    [DataMember]
    public double LimitAmount { get; set; }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(params string[] propertyNames) {
      foreach(var pn in propertyNames)
        OnPropertyChanged(pn);
    }
    protected virtual void OnPropertyChanged(string propertyName) {
      if(PropertyChanged == null) return;
      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
  static class Mixins {
    public static long ToEpochTime(this DateTime dateTime) {
      var date = dateTime.ToUniversalTime();
      var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
      var ts = ticks / TimeSpan.TicksPerSecond;
      return ts;
    }

    /// <summary>
    /// Converts the given date value to epoch time.
    /// </summary>
    public static long ToEpochTime(this DateTimeOffset dateTime) {
      var date = dateTime.ToUniversalTime();
      var ticks = date.Ticks - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks;
      var ts = ticks / TimeSpan.TicksPerSecond;
      return ts;
    }

  }
}