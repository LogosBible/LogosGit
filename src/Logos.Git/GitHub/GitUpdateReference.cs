namespace Logos.Git.GitHub
{
	/// <summary>
	/// Used to PATCH a reference.
	/// </summary>
	public sealed class GitUpdateReference
	{
		/// <summary>
		/// String of the SHA1 value to set this reference to
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// Boolean indicating whether to force the update or to make sure the update is a fast-forward update. The default is false, so leaving this out or setting it to false will make sure you’re not overwriting work.
		/// </summary>
		public bool Force { get; set; }
	}
}
