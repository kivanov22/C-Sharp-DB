namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    public class FootballBettingContext : DbContext
    {
        //initilieze for testing purpose
        public FootballBettingContext()
        {

        }

        //for judge / for outer connections
        public FootballBettingContext(DbContextOptions options)
            :base(options)
        {
                
        }

        //To configure connection to your server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        //To configure database relations (DDL)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
