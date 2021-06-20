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
		[Required(ErrorMessage = "Обязательное поле")]
		[DisplayName("Название")]
		public String Name { get; set; }

		[Required(ErrorMessage = "Обязательное поле")]
		[DisplayName("Описание")]
		public String Description { get; set; }

		[DisplayName("Год выпуска")]
		public DateTime ReleaseYear { get; set; }

		[Required(ErrorMessage = "Обязательное поле")]
		[DisplayName("Режиссёр")]
		public String Director { get; set; }

		[DisplayName("Постер")]
		public IFormFile PosterPhoto { get; set; }
	}
}
