using ServerExtender.Plugins.Exceptions;
using ServerPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender.Plugins
{
	public class PluginContainer
	{
		const string WEB_FILES_FOLDER = @"www\";
		private static Type pluginType = typeof(IPlugin);

		public bool IsLoaded { get { return Plugin != null; } }

		public IPlugin Plugin { get; private set; }

		public string DllPath { get; private set; }

		public string FileName { get { return Path.GetFileNameWithoutExtension(DllPath); } }

		public Type PluginClass { get; private set; }

		public PluginContainer(string dllPath)
		{
			DllPath = dllPath;
			InitFromPath();
		}

		public void Load()
		{
			if (IsLoaded)
				return;

			Plugin = (IPlugin)Activator.CreateInstance(PluginClass);
			CopyWebFile();
		}

		private void InitFromPath()
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(DllPath);
			Assembly assembly = Assembly.Load(assemblyName);
			if (assembly == null)
			{
				throw new InvalidPluginException("File \"" + DllPath + "\" is not a valid plugin.");
			}

			Type pluginFound = null;
			Type[] types = null;

			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				throw new InvalidPluginException("File \"" + DllPath + "\" cannot be loaded. See inner exception for more details.", ex);
			}

			foreach (Type type in types)
			{
				if (type.IsInterface || type.IsAbstract)
					continue;

				if (type.GetInterface(pluginType.FullName) != null)
				{
					if (pluginFound == null)
					{
						pluginFound = type;
					}
					else
					{
						Console.WriteLine("File \"" + DllPath + "\" contains two classes with the type \"" + pluginType + "\".");
					}
				}
			}

			if (pluginFound == null)
			{
				throw new InvalidPluginException("File \"" + DllPath + "\" contain no class implementing the type \"" + pluginType + "\".");
			}

			PluginClass = pluginFound;
		}

		private void CopyWebFile()
		{
			var pluginWebFilesFolder = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(DllPath), WEB_FILES_FOLDER));
			if (!pluginWebFilesFolder.Exists)
			{
				Console.WriteLine("Plugin \"" + Plugin.Name + "\" does not contain any web file.");
				return;
			}

			var destination = new DirectoryInfo(Path.Combine(PluginManager.PLUGIN_WEB_FOLDER_PATH, FileName));

			CopyAll(pluginWebFilesFolder, destination);
		}

		private void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			foreach (FileInfo fi in source.GetFiles())
			{
				Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
				fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
			}

			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}
	}
}
