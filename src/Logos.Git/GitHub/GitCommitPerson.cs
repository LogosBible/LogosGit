namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a person who authored or committed returned from GitHub API.
	/// </summary>
	public sealed class GitCommitPerson
	{
		/// <summary>
		/// Timestamp of when this commit was committed.
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// String of the email of the author/committer of the commit.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// String of the name of the author/committer of the commit.
		/// </summary>
		public string Name { get; set; }
	}
}
