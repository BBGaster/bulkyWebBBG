﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Display(Name ="List Price for 1-50")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name = "List Price for 50+")]
        [Range(1, 1000)]
        public double ListPrice50 { get; set; }
        [Required]
        [Display(Name = "List Price for 100+")]
        [Range(1, 1000)]
        public double ListPrice100 { get; set; }

    }
}
