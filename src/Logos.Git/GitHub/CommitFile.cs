namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git committed file returned from GitHub API.
	/// </summary>
	public sealed class CommitFile
	{
		/// <summary>
		/// Can be "modified", "added", or "removed".
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// The filename for this file.
		/// </summary>
		public string Filename { get; set; }
	}
}
