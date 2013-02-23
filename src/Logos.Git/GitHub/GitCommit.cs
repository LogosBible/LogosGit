namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git commit returned from GitHub API.
	/// </summary>
	public sealed class GitCommit
	{
		/// <summary>
		/// The commit message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The commit SHA-1 hash.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// Details of the tree object this commit points to.
		/// </summary>
		public GitCommitTree Tree { get; set; }

		/// <summary>
		/// Details regarding the commit author.
		/// </summary>
		public GitCommitPerson Author { get; set; }

		/// <summary>
		/// Details regarding the person who committed this to the repository.
		/// </summary>
		public GitCommitPerson Committer { get; set; }
	}
}
