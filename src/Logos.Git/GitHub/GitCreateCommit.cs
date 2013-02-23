namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data passed into the GitHub API to create a new commit.
	/// </summary>
	public sealed class GitCreateCommit
	{
		/// <summary>
		/// String of the commit message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Array of the SHAs of the commits that were the parents of this commit. If omitted or empty, the commit will be written as a root commit. For a single parent, an array of one SHA should be provided, for a merge commit, an array of more than one should be provided.
		/// </summary>
		public string[] Parents { get; set; }

		/// <summary>
		/// String of the SHA of the tree object this commit points to.
		/// </summary>
		public string Tree { get; set; }
	}
}
