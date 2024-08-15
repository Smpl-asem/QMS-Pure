using Microsoft.EntityFrameworkCore;

public class Context : DbContext
{
    public DbSet<Users> Users_tbl { get; set; }
    public DbSet<smsUser> sms_tbl { get; set; }
    public DbSet<UserLog> userLogs_tbl { get; set; }
    public DbSet<smsToken> smsTokens { get; set; }
    public DbSet<Category> Categories_tbl { get; set; }
    public DbSet<UserCats> UserCats_tbl { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseSqlServer("server=.\\SQL2019;database=IliaDabirkhane;trusted_connection=true;MultipleActiveResultSets=True;TrustServerCertificate=True");
        optionsBuilder.UseSqlServer("server=.\\SQL2019;database=qms;trusted_connection=true;MultipleActiveResultSets=True;TrustServerCertificate=True");
        // optionsBuilder.UseSqlServer("server=DEVELOPER2;database=IliaDabirkhaneMVC;user ID=sa;password=12345@Iran;MultipleActiveResultSets=True;TrustServerCertificate=True");
        // optionsBuilder.UseSqlServer("data source=.;initial catalog = OmidApp;integrated security=true;MultipleActiveResultSets=True;TrustServerCertificate=True");
        // optionsBuilder.UseSqlServer("data source=.;initial catalog = IliaDabir22;integrated security=true;MultipleActiveResultSets=True;TrustServerCertificate=True");
    }
}