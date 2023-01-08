namespace DotnetParallelRequests.Client;

public class LoggerClient
{

  public void LogNewSequence(string name)
  {
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.BackgroundColor = ConsoleColor.Black;
    Console.WriteLine(string.Format("\n==={0}===", name));
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Black;
  }

  public void LogNewTaskSequence(string text) 
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.BackgroundColor = ConsoleColor.Black;
    Console.WriteLine(text);
    Console.ForegroundColor = ConsoleColor.White;
    Console.BackgroundColor = ConsoleColor.Black;
  }

  public void LogSequenceTime(string text, bool isFinal=false)
  {
    if (isFinal)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else 
    {
      Console.ForegroundColor = ConsoleColor.Blue;
    }
    Console.WriteLine(text);
  }

  public void LogTask(string text)
  {
    Console.WriteLine("> " + text);
  }
}