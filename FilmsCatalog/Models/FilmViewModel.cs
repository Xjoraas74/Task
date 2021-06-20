using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace FilmsCatalog.Models
{
	public class FilmViewModel
	{
		[Required]
		[DisplayName("Название")]
		public String Name { get; set; }

		[Required]
		[DisplayName("Описание")]
		public String Description { get; set; }

		[DisplayName("Год выпуска")]
		/*[DisplayFormat(DataFormatString = "{yyyy}", ApplyFormatInEditMode = true)]*/
		public DateTime ReleaseYear { get; set; }

		[Required]
		[DisplayName("Режиссёр")]
		public String Director { get; set; }

		[DisplayName("Постер")]
		public IFormFile PosterPhoto { get; set; }
	}
}
