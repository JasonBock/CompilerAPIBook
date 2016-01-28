using Microsoft.Build.Framework;
using System.Diagnostics;
using MBU = Microsoft.Build.Utilities;

namespace CommentRemover.Task
{
	public class CommentRemoverTask
		: MBU.Task
	{
		public override bool Execute()
		{
			this.Log.LogMessage(
				$"Removing comments for project {this.ProjectFilePath}...");
			var stopwatch = Stopwatch.StartNew();
			WorkspaceCommentRemover.RemoveCommentsFromProjectAsync(
				this.ProjectFilePath).Wait();
			stopwatch.Stop();
			this.Log.LogMessage(
				$"Removing comments for project {this.ProjectFilePath} complete - total time: {stopwatch.Elapsed}.");
			return true;
		}

		[Required]
		public string ProjectFilePath { get; set; }
	}
}
