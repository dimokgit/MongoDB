using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Blueshift.EntityFrameworkCore.MongoDB.Annotations;
using System;

/// <summary>
/// https://www.myget.org/feed/efcore-mongodb/package/nuget/Blueshift.EntityFrameworkCore.MongoDB
/// </summary>
namespace HedgeHog.MongoDB {
  [MongoDatabase("forex")]
  public class ForexDbContext :DbContext {
    private string _connectionString;
    private MongoUrl _mongoUrl;

    public DbSet<Animal> Animals { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<Offer> Offer { get; set; }

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
    public BsonDateTime When { get; set; }
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
}