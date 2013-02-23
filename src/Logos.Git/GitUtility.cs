using System.Linq;
using LibGit2Sharp;
using Logos.Git.GitHub;
using Logos.Utility;
using Logos.Utility.Logging;

namespace Logos.Git
{
	/// <summary>
	/// A collection of utility methods for interfacing with Git.
	/// </summary>
	public class GitUtility
	{
		/// <summary>
		/// Checks if the local repository is clean and if at least one remote branch tip 
		/// is set to the latest known commit of a tracked remote repository.
		/// Note that this method can still return true if the local repository head is
		/// behind an up-to-date remote-tracking branch.
		/// </summary>
		/// <param name="localRepositoryPath">A path to the base directory of a repository on the local machine.</param>
		/// <param name="remoteOwner">The remote repository owner name.</param>
		/// <param name="remoteRepository">The remote repository name.</param>
		/// <param name="remoteBranch">The remote branch name of the repository.</param>
		/// <returns>True if the local repository is in sync, false if not.</returns>
		public static bool IsLocalRepositorySynchronizedToRemote(string localRepositoryPath, string remoteOwner, string remoteRepository, string remoteBranch)
		{
			string latestCommitId = GitHubClient.GetLatestCommitId(remoteOwner, remoteRepository, remoteBranch);

			// HACK: it appears that LibGit2Sharp (or perhaps LibGit2) has trouble with submodules
			//   where the .git file points to the parent repo's modules folder. Force the working
			//   directory to be the specified path.
			using (Repository repo = new Repository(localRepositoryPath, new RepositoryOptions {WorkingDirectoryPath = localRepositoryPath}))
			{
				RepositoryStatus status = repo.Index.RetrieveStatus();

				bool inSync = true;
				if (status.IsDirty)
				{
					bool isDirty = true;

					// HACK: handle an untracked .git file, in the case of a submodule (see above)
					if (status.CountIsExactly(1))
					{
						StatusEntry entry = status.Single();
						if (entry.State == FileStatus.Untracked && entry.FilePath == ".git")
							isDirty = false;
					}

					if (isDirty)
					{
						Log.Error("Git repository is dirty!");
						foreach (StatusEntry entry in status)
							Log.Debug("{0}: {1}", entry.State, entry.FilePath);
						inSync = false;
					}
				}
				else
				{
					Log.Info("Git repository is clean.");
				}

				var remoteBranches = repo.Branches.Where(x => x.IsRemote).ToList();
				if (!remoteBranches.Any(x => x.Tip.Sha == repo.Head.Tip.Sha))
					Log.Warn("Repository HEAD ({0}) does not match tip of any branch; local repository is behind.", repo.Head.Tip.Sha);

				if (!remoteBranches.Any(x => x.Tip.Sha == latestCommitId))
				{
					Log.Error("No remote branch tip is set to latest remote commit '{0}'.", latestCommitId);
					inSync = false;
				}

				return inSync;
			}
		}

		static readonly Logger Log = LogManager.GetLogger("GitUtility");
	}
}
