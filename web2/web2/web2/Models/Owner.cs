using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;

namespace web2.Models
{
	public class Owner
	{
		public string FirstName = string.Empty;
		public string LastName = string.Empty;
		public string UserID = string.Empty;
		public string Email = string.Empty;
		public Image UserImage;

		public bool exists{
			get {
				return true;
			}
		}
	}
}