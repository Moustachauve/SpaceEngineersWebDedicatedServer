using ServerExtender.Plugins.Exceptions;
using ServerPlugin;
using ServerPlugin.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerExtender.Plugins
{
	public class PluginManager
	{
		public const string PLUGIN_FOLDER_PATH = @"plugins\";
		public const string PLUGIN_WEB_FOLDER_PATH = @"www\" + PLUGIN_FOLDER_PATH + @"\";

		private static PluginManager instance;
		public static PluginManager Instance
		{
			get
			{
				if (instance == null)
					instance = new PluginManager();
				return instance;
			}
		}

		private List<PluginContainer> plugins;

		private PluginManager()
		{
			plugins = new List<PluginContainer>();
		}

		public void ScanPlugins()
		{
			if (!Directory.Exists(PLUGIN_FOLDER_PATH))
			{
				Directory.CreateDirectory(PLUGIN_FOLDER_PATH);
			}
			if (!Directory.Exists(PLUGIN_WEB_FOLDER_PATH))
			{
				Directory.CreateDirectory(PLUGIN_WEB_FOLDER_PATH);
			}

			string[] pluginFolders = Directory.GetDirectories(PLUGIN_FOLDER_PATH);

			if (pluginFolders.Length > 0)
			{
				foreach (string pluginFolder in pluginFolders)
				{
					string[] pluginsDlls = Directory.GetFiles(pluginFolder, "*.dll");
					foreach (string pluginDll in pluginsDlls)
					{
						InitializePlugin(pluginDll);
					}
				}
			}
			else
			{
				Console.WriteLine("No plugin found.");
			}
		}

		public void LoadPlugins()
		{
			var pluginsToLoad = plugins.Where(p => !p.IsLoaded);
			foreach (var pluginContainer in pluginsToLoad)
			{
				pluginContainer.Load();
				Console.WriteLine("Plugin \"" + pluginContainer.Plugin.Name + "\" [" + pluginContainer.Plugin.Version + "] loaded.");
			}
		}

		public List<WebResource> GetRequiredWebResources()
		{
			var requiredWebResources = new List<WebResource>();

			foreach (var pluginContainer in plugins)
			{
				if (!pluginContainer.IsLoaded)
					continue;

				if (pluginContainer.Plugin.RequiredWebResources != null)
				{
					foreach (var webRessource in pluginContainer.Plugin.RequiredWebResources)
					{
						string resourcePath = Path.Combine(PLUGIN_FOLDER_PATH, pluginContainer.FileName, webRessource.Path);
						requiredWebResources.Add(new WebResource(resourcePath));
					}
				}
			}

			return requiredWebResources;
		}

		public List<AngularControllerResource> GetAngularRoutes()
		{
			var requiredWebResources = new List<AngularControllerResource>();

			foreach (var pluginContainer in plugins)
			{
				if (!pluginContainer.IsLoaded)
					continue;

				if (pluginContainer.Plugin.RequiredWebResources != null)
				{
					string pluginPathPrefix = Path.Combine(PLUGIN_FOLDER_PATH, pluginContainer.FileName);
					var controllerResources = pluginContainer.Plugin.RequiredWebResources.OfType<AngularControllerResource>();
					foreach (var controllerResource in controllerResources)
					{
						string route = '/' + pluginContainer.FileName + '/' + controllerResource.Route;
						string controllerPath = Path.Combine(pluginPathPrefix, controllerResource.Path).Replace('\\', '/');
						string viewPath = Path.Combine(pluginPathPrefix, controllerResource.ViewPath).Replace('\\', '/');
						requiredWebResources.Add(new AngularControllerResource(route, controllerPath, viewPath));
					}
				}
			}

			return requiredWebResources;
		}

		public void InitializePlugin(string pluginPath)
		{
			PluginContainer pluginContainer = null;
			try
			{
				pluginContainer = new PluginContainer(pluginPath);
			}
			catch (InvalidPluginException ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}

			if (!plugins.Any(p => p.PluginClass == pluginContainer.PluginClass))
			{
				plugins.Add(pluginContainer);
				Console.WriteLine("Found plugin \"" + pluginContainer.FileName + "\"");
			}
			else
			{
				Console.WriteLine("Plugin \"" + pluginContainer.FileName + "\" already initialized.");
			}
		}
	}
}
