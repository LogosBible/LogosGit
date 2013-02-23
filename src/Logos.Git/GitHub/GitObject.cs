using System;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git object returned from GitHub API.
	/// </summary>
	public sealed class GitObject
	{
		/// <summary>
		/// The object type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// The SHA-1 hash for this object.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// The URL to this object.
		/// </summary>
		public Uri Url { get; set; }
	}
}
