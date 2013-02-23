using System;
using System.IO;
using System.Net;
using System.Text;
using Logos.Utility;
using Logos.Utility.Logging;
using Logos.Utility.Net;
using Newtonsoft.Json;

namespace Logos.Git.GitHub
{
	/// <summary>
	/// Interface for interacting with the GitHub API (http://developer.github.com/v3/).
	/// </summary>
	public static class GitHubClient
	{
		/// <summary>
		/// Set to true to use the cached gitdata.lrscorp.net API. This should be used for tools which frequently poll (every ~5 seconds).
		/// </summary>
		public static bool UseGitDataApi { get; set; }

		/// <summary>
		/// Sets login credentials passed in with all subsequent requests.
		/// </summary>
		/// <param name="userName">The GitHub username.</param>
		/// <param name="password">The GitHub password.</param>
		public static void SetCredentials(string userName, string password)
		{
			s_userName = userName;
			s_password = password;
		}

		/// <summary>
		/// Queries the GitHub API for the most recent commit for a given user, repository, and branch.
		/// </summary>
		/// <param name="user">The GitHub user/owner the repository is associated with.</param>
		/// <param name="repo">The GitHub repository name.</param>
		/// <param name="branch">The branch to query (ie 'master').</param>
		/// <returns>The SHA-1 hash string for the most recent commit.</returns>
		public static string GetLatestCommitId(string user, string repo, string branch)
		{
			if (UseGitDataApi)
			{
				Uri url = new Uri(@"http://gitdata.lrscorp.net/commits/latest/git/{0}/{1}/{2}".FormatInvariant(user, repo, branch));
				string commitId = GetString(url);
				return !string.IsNullOrWhiteSpace(commitId) ? commitId.Trim() : null;
			}
			
			GitReference reference = Get<GitReference>(@"http://git/api/v3/repos/{0}/{1}/git/refs/heads/{2}", user, repo, branch);
			return reference != null ? reference.Object.Sha : null;
		}

		/// <summary>
		/// Queries detailed information for a specific commit.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="sha">The SHA-1 hash string for the commit to query.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the queried information.</returns>
		public static Commit GetCommit(string user, string repo, string sha)
		{
			return Get<Commit>(@"http://git/api/v3/repos/{0}/{1}/commits/{2}", user, repo, sha);
		}

		/// <summary>
		/// Queries detailed information for a specific commit. This information is a subset of the data returned from <see cref="GetCommit"/>.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="sha">The SHA-1 hash string for the commit to query.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the queried information.</returns>
		public static GitCommit GetGitCommit(string user, string repo, string sha)
		{
			return Get<GitCommit>(@"http://git/api/v3/repos/{0}/{1}/git/commits/{2}", user, repo, sha);
		}

		/// <summary>
		/// Queries comparison information for two commits.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="firstSha">The SHA-1 hash string of the first commit to compare.</param>
		/// <param name="secondSha">The SHA-1 hash string of the second commit to compare.</param>
		/// <returns>Detailed comparison information.</returns>
		public static CommitComparison CompareCommits(string user, string repo, string firstSha, string secondSha)
		{
			return Get<CommitComparison>(@"http://git/api/v3/repos/{0}/{1}/compare/{2}...{3}", user, repo, firstSha, secondSha);
		}

		/// <summary>
		/// Creates a new Git "blob" (binary data) and adds it to the repository.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="blob">A <see cref="GitBlob"/> object describing the data to create.</param>
		/// <returns>A <see cref="GitBlob"/> object containing the newly created data.</returns>
		public static GitBlob CreateBlob(string user, string repo, GitBlob blob)
		{
			string json = JsonUtility.ToJson(blob);
			Uri url = new Uri(@"http://git/api/v3/repos/{0}/{1}/git/blobs".FormatInvariant(user, repo));

			var request = PostJson(url, json);
			return Get<GitBlob>(url, request);
		}

		/// <summary>
		/// Creates a new commit to the repository.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="commit">A <see cref="GitCreateCommit"/> object containing the data to commit.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the newly committed data.</returns>
		public static GitCommit CreateCommit(string user, string repo, GitCreateCommit commit)
		{
			string json = JsonUtility.ToJson(commit);
			Uri url = new Uri(@"http://git/api/v3/repos/{0}/{1}/git/commits".FormatInvariant(user, repo));

			var request = PostJson(url, json);
			return Get<GitCommit>(url, request);
		}

		/// <summary>
		/// Creates a Git tree.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="tree">A <see cref="GitCreateTree"/> object containing the data to create the tree.</param>
		/// <returns>A <see cref="GitTree"/> object containing the newly created tree data.</returns>
		public static GitTree CreateTree(string user, string repo, GitCreateTree tree)
		{
			string json = JsonUtility.ToJson(tree);
			Uri url = new Uri(@"http://git/api/v3/repos/{0}/{1}/git/trees".FormatInvariant(user, repo));

			var request = PostJson(url, json);
			return Get<GitTree>(url, request);
		}

		/// <summary>
		/// Updates a reference with new data.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="name">The branch name.</param>
		/// <param name="update">A <see cref="GitUpdateReference"/> object containing the data to update the reference with.</param>
		/// <returns>A <see cref="GitReference"/> object containing the updated reference data.</returns>
		public static GitReference UpdateReference(string user, string repo, string name, GitUpdateReference update)
		{
			string json = JsonUtility.ToJson(update);
			Uri url = new Uri(@"http://git/api/v3/repos/{0}/{1}/git/refs/heads/{2}".FormatInvariant(user, repo, name));

			var request = PostJson(url, json, "PATCH");
			GitReference reference = Get<GitReference>(url, request);

			// now that we've pushed a new branch pointer, force GitData to update its cache (there may be an
			// unacceptably long delay between updating the reference and GitHub notifying GitData via the webhook)
			if (UseGitDataApi)
				GetString(new Uri(@"http://gitdata.lrscorp.net/commits/latest/git/{0}/{1}/{2}?refreshCache=true".FormatInvariant(user, repo, name)));

			return reference;
		}

		private static T Get<T>(string urlPattern, params object[] args)
		{
			return Get<T>(new Uri(urlPattern.FormatInvariant(args)));
		}

		private static T Get<T>(Uri uri)
		{
			HttpWebRequest request = CreateWebRequest(uri);
			return Get<T>(uri, request);
		}

		private static T Get<T>(Uri uri, HttpWebRequest request)
		{
			try
			{
				using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
				{
					return JsonUtility.FromJsonTextReader<T>(reader);
				}
			}
			catch (FormatException)
			{
				Log.Error("Error requesting {0}.", uri.AbsoluteUri);
			}
			catch (JsonSerializationException)
			{
				Log.Error("Error requesting {0}.", uri.AbsoluteUri);
			}
			catch (WebException ex)
			{
				Log.Error("Error requesting {0}.", uri.AbsoluteUri);
				ex.DisposeResponse();
			}

			return default(T);
		}

		private static HttpWebRequest CreateWebRequest(Uri uri)
		{
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			request.UserAgent = "GitHubClient";
			return request;
		}

		private static string GetString(Uri uri)
		{
			HttpWebRequest request = CreateWebRequest(uri);
			try
			{
				using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					return reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				Log.Error("Error requesting {0}.", uri.AbsoluteUri);
				ex.DisposeResponse();
			}

			return null;
		}

		private static HttpWebRequest PostJson(Uri url, string json, string method = "POST")
		{
			HttpWebRequest request = CreateWebRequest(url);
			AddCredentials(request);
			request.Method = method;
			request.ContentType = "application/json; charset=utf-8";
			byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
			request.ContentLength = jsonBytes.Length;
			using (Stream stream = request.GetRequestStream())
				stream.Write(jsonBytes, 0, jsonBytes.Length);
			return request;
		}

		private static void AddCredentials(WebRequest request)
		{
			// send the basic authorization info immediately (request.Credentials will wait to be challenged by the server)
			string authInfo = s_userName + ":" + s_password;
			authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			request.Headers["Authorization"] = "Basic " + authInfo;
		}

		static GitHubClient()
		{
			UseGitDataApi = false;
		}

		static readonly Logger Log = LogManager.GetLogger("GitHubClient");
		static string s_userName;
		static string s_password;
	}
}
