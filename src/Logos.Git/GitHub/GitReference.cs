using System;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git reference returned from GitHub API.
	/// </summary>
	public sealed class GitReference
	{
		/// <summary>
		/// String of the name of the fully qualified reference (ie: refs/heads/master).
		/// </summary>
		public string Ref { get; set; }

		/// <summary>
		/// The formatted URL to this data.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// The Git object associated with this reference.
		/// </summary>
		public GitObject Object { get; set; }
	}
}
