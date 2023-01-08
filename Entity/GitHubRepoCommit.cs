using DotnetParallelRequests.Dto;

namespace DotnetParallelRequests.Entity;

public class GitHubRepoCommit
{
  public GitHubFetchingUser? fetchingUser { get; set; }
  public IEnumerable<CommitBaseDto>? commits { get; set; }
}