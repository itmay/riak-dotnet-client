namespace RiakClient.Config
{
    public sealed class RiakAuthenticationConfiguration : IRiakAuthenticationConfiguration
	{
		public string Username { get; set; }

		public string Password { get; set; }

		public string ClientCertificateFile { get; set; }

		public string ClientCertificateSubject { get; set; }

		public string CertificateAuthorityFile { get; set; }

		public bool CheckCertificateRevocation { get; set; }
	}
}
