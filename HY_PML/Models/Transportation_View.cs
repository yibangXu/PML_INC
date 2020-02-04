using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HY_PML.Models
{
	[Table("Transportation_View")]
	public partial class Transportation_View
	{
		[Key]
		public string TransportationNo { set; get; }

		public string LadingNo { set; get; }

		public DateTime? Time { set; get; }

		public string Status { set; get; }

		public string Stat { set; get; }

		public string FromGo { set; get; }

		public int Pcs { set; get; }

		public string TransportNo { set; get; }

		public string By { set; get; }

		[NotMapped]
		public int Index { set; get; }

	}
}