using System.Configuration;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Mongo.WebApi.App_Start;
using Compusight.MoveDesk.UserManagementApi.Configuration;
using System.Web.OData.Extensions;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData.Batch;
using Microsoft.Restier.Publishers.OData;

[assembly: OwinStartup(typeof(Mongo.WebApi.Startup))]

namespace Mongo.WebApi {
  /// <summary>
  /// Represents the entry point into an application.
  /// </summary>
  public class Startup {
    /// <summary>
    /// Specifies how the ASP.NET application will respond to individual HTTP request.
    /// </summary>
    /// <param name="app">Instance of <see cref="IAppBuilder"/>.</param>
    public void Configuration(IAppBuilder app) {
      CorsConfig.ConfigureCors(ConfigurationManager.AppSettings["cors"]);
      app.UseCors(CorsConfig.Options);

      var configuration = new HttpConfiguration();

      AutofacConfig.Configure(configuration);
      app.UseAutofacMiddleware(AutofacConfig.Container);

      FormatterConfig.Configure(configuration);
      RouteConfig.Configure(configuration);
      ServiceConfig.Configure(configuration);
      SwaggerConfig.Configure(configuration);

      app.UseWebApi(configuration);
    }
  }
}