public interface IDesignTimeDbContextFactory<DiscotequeContext>{

    DiscotequeContext CreateDbContext(string[] args);
}