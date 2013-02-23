using Newtonsoft.Json;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data passed into the GitHub API to create a new tree.
	/// </summary>
	public sealed class GitCreateTree
	{
		/// <summary>
		/// String of the SHA1 of the tree you want to update with new data.
		/// </summary>
		[JsonProperty("base_tree")]
		public string BaseTree { get; set; }

		/// <summary>
		/// Array of Hash objects (of path, mode, type and sha) specifying a tree structure.
		/// </summary>
		public GitTreeItem[] Tree { get; set; }
	}
}
