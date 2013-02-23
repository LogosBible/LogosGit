namespace Logos.Git.GitHub
{
	/// <summary>
	/// Returns only the SHA of a Git commit returned from GitHub API.
	/// </summary>
	public sealed class GitHubCommit
	{
		/// <summary>
		/// The SHA-1 hash for this commit.
		/// </summary>
		public string Sha { get; set; }
	}
}
