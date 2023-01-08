using System.ComponentModel.DataAnnotations;

namespace DotnetParallelRequests.Dto;

// All Data Transfer Objects
// Used for serialization purposes

public class CommitBaseDto 
{

  public string? Sha { get; set; }
  public string? NodeId { get; set; }
  public string? Url { get; set; }
  public string? HtmlUrl { get; set; }
  public string? CommentsUrl { get; set; }

  public CommitDto? Commit { get; set; }
  public AuthorDto? Author { get; set; }
  public CommiterDto? Commiter { get; set; }
  public IEnumerable<ParentDto>? Parents { get; set; }
}

public class CommitDto 
{
  public CommitAuthorDto? Author { get; set; }
  public CommitAuthorDto? Committer { get; set; }
  public string? Message { get; set; }
  public string? Url { get; set; }
  public int CommentCount { get; set; }

}

public class CommitAuthorDto 
{
  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Date { get; set; }
}

public class AuthorDto 
{
  public string? Login { get; set; }
  public int Id { get; set; }
  public string? NodeId { get; set; }
  public string? AvatarUrl { get; set; }
  public string? GravatarUrl { get; set; }
  public string? Url { get; set; }
  public string? FollowersUrl { get; set; }
  public string? FollowingUrl { get; set; }
  public string? GistsUrl { get; set; }
  public string? StarredUrl { get; set; }
  public string? SubscriptionsUrl { get; set; }
  public string? OrganizationsUrl { get; set; }
  public string? ReposUrl { get; set; }
  public string? EventsUrl { get; set; }
  public string? ReceivedEventsUrl { get; set; }
  public string? Type { get; set; }
  public bool SiteAdmin { get; set; }
}

public class CommiterDto 
{

}

public class ParentDto 
{

}