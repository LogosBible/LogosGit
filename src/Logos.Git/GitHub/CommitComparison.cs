using Newtonsoft.Json;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git commit comparison returned from GitHub API.
	/// </summary>
	public sealed class CommitComparison
	{
		/// <summary>
		/// Number of total commits.
		/// </summary>
		[JsonProperty("total_commits")]
		public int TotalCommits { get; set; }

		/// <summary>
		/// Details about the commits.
		/// </summary>
		public Commit[] Commits { get; set; }
	}
}
