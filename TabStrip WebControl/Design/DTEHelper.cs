/*
Copyright (c) 2007 Bill Davidsen (wdavidsen@yahoo.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using EnvDTE;
using EnvDTE80;

namespace SCS.Web.UI.WebControls.Design
{
	public enum ProjectPropertyType { Url = 0, FullPath, LocalPath, AssemblyName, AssemblyFileVersion }

	public sealed class DTEHelper
	{
		#region Fields
		private static EnvDTE.DTE _dte;

		public const string HC_Url = "Url";
		public const string HC_FullPath = "FullPath";
		public const string HC_LocalPath = "LocalPath";
		public const string HC_AssemblyName = "AssemblyName";
		public const string HC_AssemblyFileVersion = "AssemblyFileVersion";

		// singleton
		public static readonly DTEHelper instance = new DTEHelper();
		#endregion

		#region Constructors
		private DTEHelper()
		{
			string programId = "!VisualStudio.DTE.9.0:" + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
			SetDTE(programId);
		}
		public static DTEHelper Instance
		{
			get { return instance; }
		}
		#endregion

		#region Public Methods
		public static NameValueCollection GetProjectProperties(Project project)
		{
			if (project == null)
				throw new ArgumentException("Project cannot be null.", "project");

			NameValueCollection propertyCollection = new NameValueCollection();

			for (int i = 0; i < project.Properties.Count; i++)
			{
				string value = string.Empty;

				try
				{
					project.Properties.Item(i);
				}
				catch
				{
					continue;
				}

				propertyCollection.Add(project.Properties.Item(i).Name, value);
			}

			return propertyCollection;
		}

		public static object GetProjectPropertyValue(Project project, ProjectPropertyType projectProperty)
		{
			NameValueCollection props = GetProjectProperties(project);

			if (props != null)
				return props[EnumToStringName(projectProperty)];

			return null;
		}

		public static object GetCurrentProjectPropertyValue(ProjectPropertyType projectProperty)
		{
			Project proj = GetCurrentProject();

			if (proj != null)
			{
				try
				{
					Property prop = proj.Properties.Item(EnumToStringName(projectProperty));

					if (prop != null)
						return prop.Value;
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message);
					throw ex;
				}
			}

			return null;
		}

		public static object[] GetPropertyValuesFromAllProjects(ProjectPropertyType projectProperty)
		{
			if (_dte.Solution.Projects.Count < 1)
				return null;

			object[] values = new object[_dte.Solution.Projects.Count];

			for (int i = 0; i < values.Length; i++)
			{
				NameValueCollection propertyCollection = GetProjectProperties(DTE.Solution.Projects.Item(i));
				values[i] = propertyCollection[EnumToStringName(projectProperty)];
			}

			return values;
		}

		public static Project GetCurrentProject()
		{
			try
			{
				Projects projs = DTE.Solution.Projects;

				for (int i = 0; i < projs.Count; i++)
				{
					try
					{
						Project proj = projs.Item(i);
						Property prop = proj.Properties.Item(HC_LocalPath);

						string filePath = DTE.ActiveDocument.Path;
						string propValue = (string)prop.Value;

						if (prop != null && filePath.IndexOf(propValue, StringComparison.OrdinalIgnoreCase) > -1)
							return proj;
					}
					catch (Exception)
					{
						continue;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
				return null;
			}

			return null;
		}

		public static string GetActiveDocumentDirectory()
		{
			return DTE.ActiveDocument.Path.Substring(0, DTE.ActiveDocument.Path.LastIndexOf("\\") + 1);
		}

		public static bool IsFileInCurrentProject(string path)
		{
			object currentProjectDir = GetCurrentProjectPropertyValue(ProjectPropertyType.LocalPath);

			if (currentProjectDir != null)
			{
				if (path.IndexOf((string)currentProjectDir) > -1)
				{
					return true;
				}
			}

			return false;
		}

		public static string ResolveLocalPathToCurrentWeb(string path)
		{
			string currentProjectDir = (string)GetCurrentProjectPropertyValue(ProjectPropertyType.LocalPath);

			string resolvedPath = path.Substring(path.IndexOf((string)currentProjectDir, StringComparison.OrdinalIgnoreCase)
				+ currentProjectDir.Length);

			resolvedPath = resolvedPath.Replace(@"\", @"/");

			if (!resolvedPath.StartsWith(@"/"))
				resolvedPath = "/" + resolvedPath;

			resolvedPath = "~" + resolvedPath;

			return resolvedPath;
		}
		#endregion

		#region Private Methods
		private static void SetDTE(string programId)
		{
			IRunningObjectTable objectTable;
			IEnumMoniker moniker;

			GetRunningObjectTable(0, out objectTable);
			objectTable.EnumRunning(out moniker);

			moniker.Reset();
			System.IntPtr fetched = new System.IntPtr();

			IMoniker[] pmon = new IMoniker[1];

			while (moniker.Next(1, pmon, fetched) == 0)
			{
				IBindCtx bindContext;
				CreateBindCtx(0, out bindContext);
				string s;
				pmon[0].GetDisplayName(bindContext, null, out s);

				if (s == programId)
				{
					object returnObject;

					objectTable.GetObject(pmon[0], out returnObject);

					object ide = (object)returnObject;

					if (ide != null)
					{
						_dte = (EnvDTE.DTE)ide;
						break;
					}
				}
			}
		}

		private static string EnumToStringName(ProjectPropertyType propType)
		{
			return Enum.GetName(typeof(ProjectPropertyType), propType);
		}
		#endregion

		#region COM Imports
		[DllImport("ole32.dll")]
		public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable objectTable);

		[DllImport("ole32.dll")]
		public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
		#endregion

		#region Properties
		public static EnvDTE.DTE DTE
		{
			get { return _dte; }
		}
		#endregion
	}
}
