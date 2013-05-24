namespace Logos.Git.GitHub
{
	/// <summary>
	/// Used to POST a new reference.
	/// </summary>
	public sealed class GitCreateReference
	{
		/// <summary>
		/// String of the name of the fully qualified reference (ie: <c>refs/heads/master</c>). If it doesn’t start with ‘refs’ and have at least two slashes, it will be rejected.
		/// </summary>
		public string Ref { get; set; }

		/// <summary>
		/// String of the SHA1 value to set this reference to
		/// </summary>
		public string Sha { get; set; }
	}
}
