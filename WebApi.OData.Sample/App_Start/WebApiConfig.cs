using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace WebApi.OData.Sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // OData Configuration
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            // Add your Container Name
            builder.ContainerName = "SampleContainer";

            /* Add your entity sets that you want to expose via OData, like this:
                builder.EntitySet<User>("Users");
                builder.EntitySet<Employee>("Employees");
                builder.EntitySet<Order>("Orders");
            */
      

            config.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: null,
                    model: builder.GetEdmModel()
                );
        }
    }
}
