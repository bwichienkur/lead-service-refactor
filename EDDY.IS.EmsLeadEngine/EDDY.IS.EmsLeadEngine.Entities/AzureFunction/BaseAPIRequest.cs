using System;

namespace EDDY.IS.EmsLeadEngine.Entities.AzureFunction
{
	public class BaseAPIRequest
	{
		public string Email { get; set; }
		public string Phone { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }
		public Constants.ConversionAPIEvent Event { get; set; }
		public DateTime EventDate { get; set; }
		public string DatabaseServer { get; set; }
		public string SourceUrl { get; set; }
		public string ClientUserAgent { get; set; }
		public string ActionSource { get; set; }
		public string IPAddress { get; set; }
	}
}
