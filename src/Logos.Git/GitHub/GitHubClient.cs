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
	public sealed class GitHubClient
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GitHubClient"/>.
		/// </summary>
		/// <param name="apiRootUrl">The URL for the root of the GitHub API, e.g., <code>https://api.github.com/</code> or <code>http://git.example.com/api/v3/</code>.</param>
		public GitHubClient(Uri apiRootUrl)
			: this(apiRootUrl, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GitHubClient"/>.
		/// </summary>
		/// <param name="apiRootUrl">The URL for the root of the GitHub API, e.g., <code>https://api.github.com/</code> or <code>http://git.example.com/api/v3/</code>.</param>
		/// <param name="userName">The GitHub username.</param>
		/// <param name="password">The GitHub password.</param>
		public GitHubClient(Uri apiRootUrl, string userName, string password)
		{
			if (apiRootUrl == null)
				throw new ArgumentNullException("apiRootUrl");

			m_apiRootUrl = apiRootUrl;
			m_userName = userName;
			m_password = password;
		}

		/// <summary>
		/// Set to true to use the cached gitdata API. This should be used for tools which frequently poll (every ~5 seconds).
		/// </summary>
		public bool UseGitDataApi { get; set; }

		/// <summary>
		/// Queries the GitHub API for the most recent commit for a given user, repository, and branch.
		/// </summary>
		/// <param name="user">The GitHub user/owner the repository is associated with.</param>
		/// <param name="repo">The GitHub repository name.</param>
		/// <param name="branch">The branch to query (ie 'master').</param>
		/// <returns>The SHA-1 hash string for the most recent commit.</returns>
		public string GetLatestCommitId(string user, string repo, string branch)
		{
			if (UseGitDataApi)
			{
				Uri url = new Uri(@"http://gitdata/commits/latest/git/{0}/{1}/{2}".FormatInvariant(user, repo, branch));
				string commitId = GetString(url);
				return !string.IsNullOrWhiteSpace(commitId) ? commitId.Trim() : null;
			}
			
			GitReference reference = Get<GitReference>(@"repos/{0}/{1}/git/refs/heads/{2}", user, repo, branch);
			return reference != null ? reference.Object.Sha : null;
		}

		/// <summary>
		/// Queries detailed information for a specific commit.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="sha">The SHA-1 hash string for the commit to query.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the queried information.</returns>
		public Commit GetCommit(string user, string repo, string sha)
		{
			return Get<Commit>(@"repos/{0}/{1}/commits/{2}", user, repo, sha);
		}

		/// <summary>
		/// Queries detailed information for a specific commit. This information is a subset of the data returned from <see cref="GetCommit"/>.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="sha">The SHA-1 hash string for the commit to query.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the queried information.</returns>
		public GitCommit GetGitCommit(string user, string repo, string sha)
		{
			return Get<GitCommit>(@"repos/{0}/{1}/git/commits/{2}", user, repo, sha);
		}

		/// <summary>
		/// Queries comparison information for two commits.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="firstSha">The SHA-1 hash string of the first commit to compare.</param>
		/// <param name="secondSha">The SHA-1 hash string of the second commit to compare.</param>
		/// <returns>Detailed comparison information.</returns>
		public CommitComparison CompareCommits(string user, string repo, string firstSha, string secondSha)
		{
			return Get<CommitComparison>(@"repos/{0}/{1}/compare/{2}...{3}", user, repo, firstSha, secondSha);
		}

		/// <summary>
		/// Creates a new Git "blob" (binary data) and adds it to the repository.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="blob">A <see cref="GitBlob"/> object describing the data to create.</param>
		/// <returns>A <see cref="GitBlob"/> object containing the newly created data.</returns>
		public GitBlob CreateBlob(string user, string repo, GitBlob blob)
		{
			string json = JsonUtility.ToJson(blob);
			Uri url = new Uri(m_apiRootUrl, @"repos/{0}/{1}/git/blobs".FormatInvariant(user, repo));

			var request = PostJson(url, json);
			return Get<GitBlob>(url, request);
		}

		/// <summary>
		/// Returns the <see cref="GitBlob"/> for the specified <see cref="GitTreeItem"/>.
		/// </summary>
		/// <param name="item">The tree item.</param>
		/// <returns>A <see cref="GitBlob"/> object with the contents of that item.</returns>
		public GitBlob GetBlob(GitTreeItem item)
		{
			return Get<GitBlob>(item.Url);
		}

		/// <summary>
		/// Creates a new commit to the repository.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="commit">A <see cref="GitCreateCommit"/> object containing the data to commit.</param>
		/// <returns>A <see cref="GitCommit"/> object containing the newly committed data.</returns>
		public GitCommit CreateCommit(string user, string repo, GitCreateCommit commit)
		{
			string json = JsonUtility.ToJson(commit);
			Uri url = new Uri(m_apiRootUrl, @"repos/{0}/{1}/git/commits".FormatInvariant(user, repo));

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
		public GitTree CreateTree(string user, string repo, GitCreateTree tree)
		{
			string json = JsonUtility.ToJson(tree);
			Uri url = new Uri(m_apiRootUrl, @"repos/{0}/{1}/git/trees".FormatInvariant(user, repo));

			var request = PostJson(url, json);
			return Get<GitTree>(url, request);
		}

		/// <summary>
		/// Returns the <see cref="GitTree"/> for a specified <see cref="GitCommit"/>.
		/// </summary>
		/// <param name="commit">The commit.</param>
		/// <returns>Information about the tree for that commit.</returns>
		public GitTree GetTree(GitCommit commit)
		{
			return Get<GitTree>(commit.Tree.Url);
		}

		/// <summary>
		/// Updates a reference with new data.
		/// </summary>
		/// <param name="user">The repository owner.</param>
		/// <param name="repo">The repository name.</param>
		/// <param name="name">The branch name.</param>
		/// <param name="update">A <see cref="GitUpdateReference"/> object containing the data to update the reference with.</param>
		/// <returns>A <see cref="GitReference"/> object containing the updated reference data.</returns>
		public GitReference UpdateReference(string user, string repo, string name, GitUpdateReference update)
		{
			string json = JsonUtility.ToJson(update);
			Uri url = new Uri(m_apiRootUrl, @"repos/{0}/{1}/git/refs/heads/{2}".FormatInvariant(user, repo, name));

			var request = PostJson(url, json, "PATCH");
			GitReference reference = Get<GitReference>(url, request);

			// now that we've pushed a new branch pointer, force GitData to update its cache (there may be an
			// unacceptably long delay between updating the reference and GitHub notifying GitData via the webhook)
			if (UseGitDataApi)
				GetString(new Uri(@"http://gitdata/commits/latest/git/{0}/{1}/{2}?refreshCache=true".FormatInvariant(user, repo, name)));

			return reference;
		}

		private T Get<T>(string urlPattern, params object[] args)
		{
			return Get<T>(new Uri(m_apiRootUrl, urlPattern.FormatInvariant(args)));
		}

		private T Get<T>(Uri uri)
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
					return JsonUtility.FromJsonTextReader<T>(reader);
			}
			catch (FormatException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
			}
			catch (InvalidDataException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
			}
			catch (JsonSerializationException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
			}
			catch (WebException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
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

		private string GetString(Uri uri)
		{
			HttpWebRequest request = CreateWebRequest(uri);
			try
			{
				using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					return reader.ReadToEnd();
			}
			catch (InvalidDataException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
			}
			catch (WebException ex)
			{
				Log.Error("{0} for {1} {2}: {3}", ex.GetType().Name, request.Method, uri.AbsoluteUri, ex.Message);
				ex.DisposeResponse();
			}

			return null;
		}

		private HttpWebRequest PostJson(Uri url, string json, string method = "POST")
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

		private void AddCredentials(WebRequest request)
		{
			// send the basic authorization info immediately (request.Credentials will wait to be challenged by the server)
			string authInfo = m_userName + ":" + m_password;
			authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
			request.Headers["Authorization"] = "Basic " + authInfo;
		}

		static readonly Logger Log = LogManager.GetLogger("GitHubClient");

		readonly Uri m_apiRootUrl;
		readonly string m_userName;
		readonly string m_password;
	}
}
