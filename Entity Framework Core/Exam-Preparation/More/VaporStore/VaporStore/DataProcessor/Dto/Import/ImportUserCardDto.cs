﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUserCardDto
    {
        [Required]
        [RegularExpression(@"^[A-Z][a-z]{2,} [A-Z][a-z]{2,}$")]
        public string FullName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Range(3,103)]
        public int Age { get; set; }

        public ICollection<ImportCardDto> Cards { get; set; }
    }
}
