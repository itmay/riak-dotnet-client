using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace RiakClient.Config
{
    public sealed class RiakClusterConfiguration : IRiakClusterConfiguration
    {
	    private static readonly Timeout DefaultNodePollTime = new Timeout(TimeSpan.FromSeconds(5));
	    private static readonly Timeout DefaultDefaultRetryWaitTime = new Timeout(200);

		private readonly List<IRiakNodeConfiguration> _nodes = new List<IRiakNodeConfiguration>();

	    public IEnumerable<IRiakNodeConfiguration> RiakNodes => _nodes;

	    public Timeout NodePollTime { get; set; } = DefaultNodePollTime;

	    public Timeout DefaultRetryWaitTime { get; set; } = DefaultDefaultRetryWaitTime;

	    public int DefaultRetryCount { get; set; } = 3;

	    public bool UseTtbEncoding { get; set; } = true;

	    public bool DisableListExceptions { get; set; } = false;

	    public IRiakAuthenticationConfiguration Authentication { get; set; }

	    public void AddNode(IRiakNodeConfiguration nodeConfiguration)
	    {
		    _nodes.Add(nodeConfiguration);
	    }

	    /// <summary>
	    /// Load a <see cref="RiakClusterConfiguration"/> from the local configuration file,
	    /// and return a new <see cref="IRiakClusterConfiguration"/>. 
	    /// </summary>
	    /// <param name="configuration">Set of configuration properties.</param>
	    /// <param name="sectionName">The section to load the configuration from.</param>
	    /// <returns>An initialized and configured <see cref="IRiakClusterConfiguration"/>.</returns>
	    public static IRiakClusterConfiguration LoadFromConfig(IConfiguration configuration, string sectionName)
	    {
		    if (configuration == null)
		    {
			    throw new ArgumentNullException(nameof(configuration));
		    }

		    return configuration.GetSection(sectionName).Get<RiakClusterConfiguration>();
	    }

	    /// <summary>
	    /// Load a <see cref="RiakClusterConfiguration"/> from a specified configuration file,
	    /// and return a new <see cref="IRiakClusterConfiguration"/>.
	    /// </summary>
	    /// <param name="sectionName">The section to load the configuration from.</param>
	    /// <param name="fileName">The file containing the configuration section.</param>
	    /// <returns>An initialized and configured <see cref="IRiakClusterConfiguration"/>.</returns>
	    public static IRiakClusterConfiguration LoadFromConfig(string sectionName, string fileName)
	    {
		    var configuration = new ConfigurationBuilder()
			    .SetBasePath(AppContext.BaseDirectory)
			    .AddJsonFile(fileName)
			    .Build();

		    return LoadFromConfig(configuration, sectionName);
	    }
	}
}
