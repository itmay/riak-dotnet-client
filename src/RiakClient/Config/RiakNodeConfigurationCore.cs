namespace RiakClient.Config
{
	/// <summary>
	/// Represents a configuration element for a Riak Node.
	/// </summary>
	public sealed class RiakNodeConfiguration : IRiakNodeConfiguration
    {
	    private static readonly Timeout DefaultTimeout = new Timeout(4000);

		//Required
	    public string Name { get; set; }

	    //Required
		public string HostAddress { get; set; }

	    public int PbcPort { get; set; } = 8087;

	    public int PoolSize { get; set; } = 30;

	    public bool ExternalLoadBalancer { get; set; } = false;

	    public bool UseTtbEncoding { get; set; } = true;

	    public Timeout NetworkReadTimeout { get; set; } = DefaultTimeout;

		public Timeout NetworkWriteTimeout { get; set; } = DefaultTimeout;

		public Timeout NetworkConnectTimeout { get; set; } = DefaultTimeout;
	}
}
