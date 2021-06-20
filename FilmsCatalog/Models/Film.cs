using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FilmsCatalog.Models
{
	public class Film
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[DisplayName("Название")]
		public String Name { get; set; }

		[Required]
		[DisplayName("Описание")]
		public String Description { get; set; }

		[DisplayName("Год выпуска")]
		public DateTime ReleaseYear { get; set; }

		[Required]
		[DisplayName("Режиссёр")]
		public String Director { get; set; }

		[Required]
		public String UserSenderId { get; set; }

		public User UserSender { get; set; }

		public String PosterName { get; set; }

		public String PosterPath { get; set; }
	}
}
