using DotnetParallelRequests.Dto;
using Newtonsoft.Json;

namespace DotnetParallelRequests.Client;

public class GitHubClient
{
  private HttpClient httpClient;
  private LoggerClient loggerClient;
  private string user;

  public GitHubClient(string user)
  {
    this.httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("User-Agent", "C# Test");
    this.loggerClient = new LoggerClient();
    this.user = user;
  }

  public async Task<IEnumerable<CommitBaseDto>> FetchCommits(string repo) 
  {
    loggerClient.LogTask(string.Format("Se interogheaza {0}/{1}...", user, repo));
    var url = string.Format("https://api.github.com/repos/{0}/{1}/commits", this.user, repo);
    var response = await httpClient.GetAsync(url).ConfigureAwait(false);
    var responseJsonContent = await response.Content.ReadAsStringAsync();
    loggerClient.LogTask(string.Format("Se deserializeaza {0}/{1}...", user, repo));

    var commits = JsonConvert.DeserializeObject<IEnumerable<CommitBaseDto>>(responseJsonContent);
    return commits ?? Enumerable.Empty<CommitBaseDto>();
  }
}