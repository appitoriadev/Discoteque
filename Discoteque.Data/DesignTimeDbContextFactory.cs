using Discoteque.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiscotequeContext>
{
    public DiscotequeContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DiscotequeContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=discoteque;Username=discotequeUsr;Password=localDk");

        return new DiscotequeContext(optionsBuilder.Options);
    }
}
