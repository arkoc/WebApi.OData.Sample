using System.Data.Entity;

namespace WebApi.OData.Sample.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("AppDbConnection")
        {
        }
    }
}