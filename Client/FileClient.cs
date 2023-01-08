namespace DotnetParallelRequests.Client;

public class FileClient {

  private LoggerClient loggerClient;
  private string filename;

  public FileClient(string filename) 
  {
    this.filename = filename;
    this.loggerClient = new LoggerClient();
  }

  public async Task WriteFileContentsAsync(string content) 
  {
    var filepath = "data/"+filename;
    loggerClient.LogTask(string.Format("Se scrie {0}...", filepath));
    await File.WriteAllTextAsync(filepath, content);
  }
}