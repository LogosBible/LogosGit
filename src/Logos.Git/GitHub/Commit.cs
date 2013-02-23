using Newtonsoft.Json;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git commit returned from GitHub API.
	/// </summary>
	public sealed class Commit
	{
		/// <summary>
		/// The SHA-1 for this commit.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// Details describing the commit.
		/// </summary>
		[JsonProperty("commit")]
		public GitCommit GitCommit { get; set; }

		/// <summary>
		/// Details about the files contained in this commit.
		/// </summary>
		public CommitFile[] Files { get; set; }
	}
}
