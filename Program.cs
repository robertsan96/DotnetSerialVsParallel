using System.Diagnostics;

using DotnetParallelRequests.Client;
using DotnetParallelRequests.Dto;
using DotnetParallelRequests.Entity;
using Newtonsoft.Json;

var logger = new LoggerClient();

var userRepos = new List<GitHubFetchingUser>() 
{
  new GitHubFetchingUser { Username="torvalds", Repository="linux" },
  new GitHubFetchingUser { Username="microsoft", Repository="dotnet" },
  new GitHubFetchingUser { Username="facebook", Repository="react-native" },
  new GitHubFetchingUser { Username="postgres", Repository="postgres" },
  new GitHubFetchingUser { Username="apple", Repository="swift" },
};

//
// Performance Test: Parallel Sequence
// 1. Fetch commits for a user & repo (request + deserialization into DTOs)
// 2. Serialize the DTO into string & write a file json file with it
//

///

var parallelCommits = new List<GitHubRepoCommit>(); // GitHub commits
var parallelRequestAndDeserializations = new Dictionary<GitHubFetchingUser, Task>(); // Tasks & Task Results for phase 1: fetching info
var parallelSerializationAndFileWrites = new List<Task>(); // Tasks for phase 2: file writing

// 1. Fetch commits for a user & repo (request + deserialization into DTOs)

logger.LogNewSequence("Executare Paralela");
logger.LogNewTaskSequence(string.Format("Se cer informatiile de pe GitHub pentru {0} utilizatori/repos...", userRepos.Count));

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

// 1.1 Request and deserialize using GitHub Client
foreach(var fetchingUser in userRepos) 
{
  _ = fetchingUser.Username ?? throw new ArgumentNullException();
  _ = fetchingUser.Repository ?? throw new ArgumentNullException();

  var ghClient = new GitHubClient(fetchingUser.Username);
  var task = ghClient.FetchCommits(fetchingUser.Repository);
  parallelRequestAndDeserializations.Add(fetchingUser, task);
}

// 1.1.1 Blocks execution until all tasks are done in parallel
await Task.WhenAll(parallelRequestAndDeserializations.Values);

// 1.2 Collect GitHub results and store them in the parallelCommits variable
foreach(var taskSet in parallelRequestAndDeserializations) 
{
  var result = ((Task<IEnumerable<CommitBaseDto>>)taskSet.Value).Result;
  var ghRepoCommit = new GitHubRepoCommit() { fetchingUser=taskSet.Key, commits=result };
  parallelCommits.Add(ghRepoCommit);
}

stopwatch.Stop();
TimeSpan elapsedParallelRestAndDeserialization = stopwatch.Elapsed;

logger.LogSequenceTime(string.Format("Timp paralel (rest api + deserializare JSON): {0}", elapsedParallelRestAndDeserialization.TotalMilliseconds));

stopwatch.Restart();
logger.LogNewTaskSequence("Se initializeaza scrierea in fisiere a datelor cerute...");

// 2. Serialize the DTO into string & write a file json file with it
foreach(var parallelCommit in parallelCommits)
{
  _ = parallelCommit.fetchingUser?.Username ?? throw new ArgumentNullException();
  _ = parallelCommit.fetchingUser.Repository ?? throw new ArgumentNullException();
  var filename = parallelCommit.fetchingUser.Username + "_" + parallelCommit.fetchingUser.Repository + ".json";
  var fileClient = new FileClient("paralel_" + filename);
  var jsonString = JsonConvert.SerializeObject(parallelCommit.commits);

  var task = fileClient.WriteFileContentsAsync(jsonString ?? "Something is definitely wrong.");
  parallelSerializationAndFileWrites.Add(task);
}

await Task.WhenAll(parallelSerializationAndFileWrites);

stopwatch.Stop();
TimeSpan elapsedParallelSerializationAndFileWrites = stopwatch.Elapsed;

logger.LogSequenceTime(string.Format("Timp paralel (serializare + scriere in fisiere json): {0} ms", elapsedParallelSerializationAndFileWrites.TotalMilliseconds));
logger.LogSequenceTime(string.Format("\nTimp paralel (total): {0} ms", (elapsedParallelSerializationAndFileWrites + elapsedParallelRestAndDeserialization).TotalMilliseconds), true);

//
// Performance Test: Serial Requests
//

logger.LogNewSequence("Executare Seriala");
logger.LogNewTaskSequence(string.Format("Se cer informatiile de pe GitHub pentru {0} utilizatori/repos...", userRepos.Count));

stopwatch.Restart();

var serialCommits = new List<GitHubRepoCommit>();

foreach(var githubFetchingUser in userRepos) 
{
  _ = githubFetchingUser.Username ?? throw new ArgumentNullException(nameof(githubFetchingUser.Username));
  _ = githubFetchingUser.Repository ?? throw new ArgumentNullException(nameof(githubFetchingUser.Repository));
  var ghClient = new GitHubClient(githubFetchingUser.Username);
  var ghCommit = await ghClient.FetchCommits(githubFetchingUser.Repository);
  var ghRepoCommit = new GitHubRepoCommit() { fetchingUser=githubFetchingUser, commits=ghCommit };
  serialCommits.Add(ghRepoCommit);
}

stopwatch.Stop();
TimeSpan elapsedRestAndDeserialization = stopwatch.Elapsed;

logger.LogSequenceTime(string.Format("Timp serial (rest api + deserializare JSON): {0} ms", elapsedRestAndDeserialization.TotalMilliseconds));

// Performance Test: Serial File Writing
stopwatch.Restart();

logger.LogNewTaskSequence("Se initializeaza scrierea in fisiere a datelor cerute...");

foreach(var repoCommit in serialCommits) 
{
  _ = repoCommit.fetchingUser?.Username ?? throw new ArgumentNullException();
  _ = repoCommit.fetchingUser.Repository ?? throw new ArgumentNullException();
  var filename = repoCommit.fetchingUser.Username + "_" + repoCommit.fetchingUser.Repository + ".json";
  var fileClient = new FileClient(filename);
  var jsonString = JsonConvert.SerializeObject(repoCommit.commits);
  await fileClient.WriteFileContentsAsync(jsonString ?? "Something is definitely wrong.");
}

stopwatch.Stop();
TimeSpan elapsedSerializationAndFileWriting = stopwatch.Elapsed;

logger.LogSequenceTime(string.Format("Timp serial (serializare + scriere in fisiere json): {0} ms", elapsedSerializationAndFileWriting.TotalMilliseconds));
logger.LogSequenceTime(string.Format("\nTimp serial (total): {0} ms", (elapsedSerializationAndFileWriting + elapsedRestAndDeserialization).TotalMilliseconds), true);
