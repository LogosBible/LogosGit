using System;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a commit tree returned from GitHub API.
	/// </summary>
	public sealed class GitCommitTree
	{
		/// <summary>
		/// URL to this commit tree.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// SHA-1 hash of this commit tree.
		/// </summary>
		public string Sha { get; set; }
	}
}
