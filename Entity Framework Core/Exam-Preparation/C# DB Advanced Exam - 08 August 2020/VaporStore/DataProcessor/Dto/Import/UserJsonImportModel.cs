using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UserJsonImportModel
    {
        //        •	Id – integer, Primary Key
        //•	Username – text with length[3, 20] (required)
        //•	FullName – text, which has two words, consisting of Latin letters.Both start with an upper letter and are followed by lower letters.The two words are separated by a single space (ex. "John Smith") (required)
        //•	Email – text(required)
        //•	Age – integer in the range[3, 103] (required)
        //•	Cards – collection of type Card
        //        •	Number – text, which consists of 4 pairs of 4 digits, separated by spaces(ex. “1234 5678 9012 3456”) (required)
        //•	Cvc – text, which consists of 3 digits(ex. “123”) (required)
        //•	Type – enumeration of type CardType, with possible values(“Debit”, “Credit”) (required)
        //•	UserId – integer, foreign key(required)
        //•	User – the card’s user(required)
        //•	Purchases – collection of type Purchase



        [Required]
        [RegularExpression("[A-Z][a-z]{2,} [A-Z][a-z]{2,}")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3,103)]
        public int Age { get; set; }

        public IEnumerable<Card> Cards { get; set; }

    }
}
