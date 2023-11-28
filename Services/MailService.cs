namespace CarRental.Services
{
	public class MailService : IMailService
	{
		//public void SendMail(CreateMailRequest re)
		//{
		//	MailjetClient client = new("7c422dab4ab56712284b03f409082f0c", "70e3498b7a231f5b74b9acea3d71365f");
		//	MailjetRequest request = new MailjetRequest
		//	{
		//		Resource = Send.Resource,
		//	}
		//	   .Property(Send.FromEmail, "minhson10102002@gmail.com")
		//	   .Property(Send.FromName, "San su kien")
		//	   .Property(Send.Subject, re.Title)
		//	   .Property(Send.HtmlPart, re.Content)
		//	   .Property(Send.Recipients, new JArray {
		//				new JObject {
		//					 {"Email", re.Email},
		//					 {"Name", re.Name}
		//				}
		//		   });

		//	_ = Task.Run(() => client.PostAsync(request));
		//}
	}
}