using System;
using System.Diagnostics;
using System.IO;

public static class Program
{
	private static void Main(string[] args)
	{
		string filePath = Path.Combine(Environment.CurrentDirectory, "./EscapeFromTarkov.exe");
		string launchArgs = "-bC5vLmcuaS5u=eyBlbWFpbDogInVzZXIwQGpldC5jb20iLCBwYXNzd29yZDogInBhc3N3b3JkIiwgdG9nZ2xlOiB0cnVlLCB0aW1lc3RhbXA6IDEzMjE3ODA5NzYzNTM2MTQ4M30= -token=0 -screenmode=fullscreen";

		Process.Start(filePath, launchArgs);
	}
}
