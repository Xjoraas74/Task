using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FilmsCatalog.Models
{
	public class FilmViewModel
	{
		[Required]
		public String Name { get; set; }

		[Required]
		public String Description { get; set; }

		public DateTime ReleaseYear { get; set; }

		[Required]
		public String Director { get; set; }

		public IFormFile PosterPhoto { get; set; }
	}
}
