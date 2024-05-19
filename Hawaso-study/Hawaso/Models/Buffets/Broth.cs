﻿using System.ComponentModel.DataAnnotations;

namespace Hawaso.Models.Buffets
{
    /// <summary>
    /// 국물
    /// </summary>
    public class Broth
    {
        public int Id{ get; set; }
        
        [Required]
        [StringLength(25)]
        public string? Name{ get; set; }

        public bool IsVegan{ get; set; }

        public List<Noodle> Noodles { get; set; } = new List<Noodle>();
    }
}

