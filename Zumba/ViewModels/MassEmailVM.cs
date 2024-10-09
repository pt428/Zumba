using Zumba.Models;

namespace Zumba.ViewModels
{
	public class MassEmailVM
	{
		public string EmailSubject { get; set; }=string.Empty;
		public string EmailBody { get; set; } = string.Empty;
		public List<MassEmailRecipient> Recipients { get; set; }
	}
}
