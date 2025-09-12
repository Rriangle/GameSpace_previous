using System;

namespace GameSpace.Areas.social_hub.Models.ViewModels
{
	public class MessageViewModel
	{
		public int MessageId { get; set; }
		public int SenderId { get; set; }
		public string User { get; set; } = "";
		public string Content { get; set; } = "";
		public DateTime Time { get; set; }
		public bool IsMine { get; set; }
		public bool IsRead { get; set; }  // For messages I sent: represents whether the recipient has read it
	}
}
