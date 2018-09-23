using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class NSidekick
{
	private const string _version = "0.9.5.8-beta";

	private static bool _attached = false;

	internal static Result Attach()
	{
		return AttachWith(
			new SequentialNSidekickLocator(new PackagesNSidekickLocator(), new RegistryNSidekickLocator()),
			new DefaultNSidekickLoader(),
			DefaultShellFactoryTypeName
			);
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static Result AttachWith(INSidekickLocator locator, INSidekickLoader loader, string shellFactoryTypeName)
	{
		var result = new Result();

		if (_attached)
		{
			result.FailReason = FailReason.AlreadyAttached;
			return result;
		}

		dynamic shell = null;
		
		try
		{
			result.FailReason = FailReason.NSidekickNotFound;
			string path = locator.TryGetExecutablePath();

			bool wasFound = !string.IsNullOrEmpty(path) && File.Exists(path);
			if (!wasFound)
				return result;

			result.FailReason = FailReason.CouldNotLoadAssembly;
			var assembly = loader.Load(path);
			if (assembly == null)
				return result;

			result.FailReason = FailReason.CouldNotFindShellFactory;
			var creatorType = assembly.GetType(shellFactoryTypeName);
			if (creatorType == null)
				return result;

			result.FailReason = FailReason.CouldNotCreateInstanceOfShellFactory;
			dynamic creator = Activator.CreateInstance(creatorType);
			if (creator == null)
				return result;

			result.FailReason = FailReason.CouldNotCreateInstanceOfShell;
			shell = creator.CreateShell();
			if (shell == null)
				return result;

			result.FailReason = FailReason.CouldNotAttachShell;
			shell.Attach();

			if (shell.HasRequiredPlugins)
			{
				_attached = true;
				result.FailReason = FailReason.None;
			}
			else
			{
				result.FailReason = FailReason.NoPlugins;
			}
		}
		catch (Exception ex)
		{
			result.Exception = ex;
			
			if (shell != null && shell.Log != null)
				shell.Log.Fatal(result.ToString());
		}

		if (!result.Success)
			System.Diagnostics.Debug.WriteLine(result.ToString());

		return result;
	}

	internal static void ResetState()
	{
		_attached = false;
	}

	internal static string DefaultShellFactoryTypeName
	{
		get { return "NSidekick.ShellFactory"; }
	}

	internal interface INSidekickLocator
	{
		string TryGetExecutablePath();
	}

	internal class SequentialNSidekickLocator : INSidekickLocator
	{
		public SequentialNSidekickLocator(params INSidekickLocator[] locators)
		{
			if (locators == null)
				throw new ArgumentNullException(nameof(locators));
		
			Locators = locators;
		}

		public string TryGetExecutablePath()
		{
			foreach (var locator in Locators)
			{
				var path = locator.TryGetExecutablePath();
				if (!string.IsNullOrEmpty(path))
					return path;
			}

			return null;
		}

		public INSidekickLocator[] Locators { get; }
	}

	internal class PackagesNSidekickLocator : INSidekickLocator
	{
		public string TryGetExecutablePath()
		{
			var path = Assembly.GetEntryAssembly().Location;

			DirectoryInfo directory;
			DirectoryInfo packagesDirectory = null;

			do
			{
				directory = new DirectoryInfo(path).Parent;
				path = directory.FullName;

				packagesDirectory = directory.EnumerateDirectories("packages").FirstOrDefault();
			}
			while (directory.Parent != null && packagesDirectory == null);

			if (packagesDirectory == null)
				return null;

			// TODO -> Edition? Version?
			// use * to find different editions of the same version
			string directoryPatternWithVersion = "NSidekick*" + _version;
			var executables = packagesDirectory
				.EnumerateDirectories(directoryPatternWithVersion, SearchOption.TopDirectoryOnly)
				.SelectMany(d => d.EnumerateFiles("NSidekick*.exe", SearchOption.AllDirectories));

			var executable = executables.LastOrDefault();
			if (executable == null)
				return null;

			return executable.FullName;
		}
	}

	internal class RegistryNSidekickLocator : INSidekickLocator
	{
		public string TryGetExecutablePath()
		{
			var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\sodacore studios\NSidekick");
			if (key == null)
				return null;

			return (string)key.GetValue("Path");
		}
	}

	internal interface INSidekickLoader
	{
		Assembly Load(string path);
	}

	internal class DefaultNSidekickLoader : INSidekickLoader
	{
		public Assembly Load(string path)
		{
			return Assembly.LoadFrom(path);
		}
	}

	internal enum FailReason
	{
		None = 0,
		AlreadyAttached = 1,
		NSidekickNotFound = 2,
		CouldNotLoadAssembly = 3,
		CouldNotFindShellFactory = 4,
		CouldNotCreateInstanceOfShellFactory = 5,
		CouldNotCreateInstanceOfShell = 6,
		CouldNotAttachShell = 7,
		NoPlugins = 8
	}

	internal class Result
	{
		public Result()
		{
			FailReason = FailReason.None;
		}

		public FailReason FailReason { get; set; }

		public Exception Exception { get; set; }

		public bool Success
		{
			get { return FailReason == FailReason.None; }
		}

		public override string ToString()
		{
			if (Success)
				return "Successfully attached.";

			string message = "Failed. Reason was: " + FailReason.ToString();

			var exception = Exception;
			int exceptionNumber = 1;

			while (exception != null)
			{
				message += Environment.NewLine + exceptionNumber++ + ") " + GetStringFromException(exception);
				exception = exception.InnerException;
			}

			return message;
		}

		private string GetStringFromException(Exception exception)
		{
			var typeLoadException = exception as ReflectionTypeLoadException;
			var exceptionTypeName = exception.GetType().Name;

			if (typeLoadException != null)
			{
				var loaderExceptions = typeLoadException.LoaderExceptions
					.Select(ex => "   ->" + GetStringFromException(ex));

				return exceptionTypeName + ":" + Environment.NewLine + string.Join(Environment.NewLine, loaderExceptions);
			}

			return exceptionTypeName + ": " + exception.Message;
		}
	}
}
