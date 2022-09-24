using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DTOs.Categories
{
    public class ImportCategoriesDto
    {
        [JsonProperty("name")]
        [Required]
        [MinLength(3)]
        [MaxLength(15)]
        public string Name { get; set; }
    }
}
