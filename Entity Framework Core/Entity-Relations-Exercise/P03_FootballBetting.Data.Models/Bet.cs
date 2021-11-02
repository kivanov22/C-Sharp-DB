
namespace P03_FootballBetting.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        public decimal Amount { get; set; }

        public decimal Prediction { get; set; }//?? enum or string

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }
    }
}
