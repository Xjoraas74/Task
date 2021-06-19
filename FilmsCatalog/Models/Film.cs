using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FilmsCatalog.Models
{
	public class Film
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public String Name { get; set; }

		[Required]
		public String Description { get; set; }

		public DateTime ReleaseYear { get; set; }

		[Required]
		public String Director { get; set; }

		[Required]
		public String UserSenderId { get; set; }

		public User UserSender { get; set; }

		public String PosterName { get; set; }

		public String PosterPath { get; set; }
	}
}
