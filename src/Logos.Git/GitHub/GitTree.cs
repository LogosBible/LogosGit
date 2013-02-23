using System;
using Newtonsoft.Json;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git tree returned from GitHub API.
	/// </summary>
	public sealed class GitTree
	{
		/// <summary>
		/// URL pointing to this tree data.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// String of the SHA1 of the tree.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// Items contained in the tree.
		/// </summary>
		[JsonProperty("tree")]
		public GitTreeItem[] Items { get; set; }
	}
}
