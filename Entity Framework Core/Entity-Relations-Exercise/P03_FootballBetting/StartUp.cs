namespace P03_FootballBetting
{
    using System;
    using P03_FootballBetting.Data;
    public class StartUp
    {
        static void Main(string[] args)
        {
            //without migrations approach
            //FootballBettingContext dbContext = new FootballBettingContext();

            //dbContext.Database.EnsureCreated();

            //Console.WriteLine("DB created success !");

            //dbContext.Database.EnsureDeleted();
        }
    }
}
