using Discoteque.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiscotequeContext>
{
    public DiscotequeContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DiscotequeContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DiscotequeDatabase") 
            ?? "Host=localhost;Database=discoteque;Username=discotequeUsr;Password=localDk";
            
        optionsBuilder.UseNpgsql(connectionString);

        return new DiscotequeContext(optionsBuilder.Options);
    }
}
