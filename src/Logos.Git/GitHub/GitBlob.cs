using System;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Data describing a Git blob, which is passed into and returned from GitHub API.
	/// </summary>
	public sealed class GitBlob
	{
		/// <summary>
		/// The blob encoding, either "utf-8" or "base64".
		/// </summary>
		public string Encoding { get; set; }

		/// <summary>
		/// Content of the blob.
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// URL path to this blob.
		/// </summary>
		public Uri Url { get; set; }

		/// <summary>
		/// Size of the blob, in bytes.
		/// </summary>
		public int? Size { get; set; }

		/// <summary>
		/// SHA-1 hash for this blob.
		/// </summary>
		public string Sha { get; set; }

		/// <summary>
		/// Encodes the string and returns the Content based on the Encoding property.
		/// </summary>
		/// <returns>The blob content as an encoded string.</returns>
		public string GetContent()
		{
			switch (Encoding)
			{
			case "base64":
				return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Content));

			case "utf-8":
				return Content;

			default:
				throw new InvalidOperationException(string.Format("'encoding' type '{0}' is not supported.", Encoding));
			}
		}
	}
}
