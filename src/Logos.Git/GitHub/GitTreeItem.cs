using System;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing an item in a Git tree returned from GitHub API.
	/// </summary>
	public sealed class GitTreeItem
	{
		/// <summary>
		/// String of blob, tree, commit.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// URL pointing to this data.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// Number of bytes of the file referenced in the tree.
		/// </summary>
		public int? Size { get; set; }

		/// <summary>
		/// String of SHA1 checksum ID of the object in the tree.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// String of the file referenced in the tree.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// String of the file mode - one of 100644 for file (blob), 100755 for executable (blob), 040000 for subdirectory (tree), 160000 for submodule (commit) or 120000 for a blob that specifies the path of a symlink.
		/// </summary>
		public string Mode { get; set; }
	}
}
