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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SCS.Web.UI.WebControls.Design
{
	[System.Xml.Serialization.XmlRootAttribute("TabStripSettings", IsNullable = false)]
	public class TabStripSettings
	{
		[System.Xml.Serialization.XmlElementAttribute("SavedSettings")]
		public SavedSettings SavedTabStripSettings = new SavedSettings();

		public TabStripSettings()
		{		   
		}

		public void Serialize(string path, TabStripSettings settings)
		{
			//if (! File.Exists(path))
			//{
			//	File.WriteAllText(path, _createXml);
			//}

			using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TabStripSettings));

				// TODO: figure out why the formatting is not working.
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;
				writer.IndentChar = char.Parse(" ");	

				serializer.Serialize(writer, settings);
			}
		}
		public TabStripSettings Deserialize(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("A saved settings file was not found.", path);

			using (XmlTextReader reader = new XmlTextReader(path))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TabStripSettings));
				TabStripSettings settings = (TabStripSettings)serializer.Deserialize(reader);

				return settings;
			}
		}
	}

	[System.Xml.Serialization.XmlTypeAttribute()]
	public class SavedSettings
	{
		#region Fields
		private int _bottomPadding; 
		private string _tabColor; 
		private string _borderColor; 
		private int _borderWidth; 
		private string _selectedTabColor; 
		private int _tabWidth; 
		private int _tabHeight; 
		private string _tabType; 
		private string _tabCornerType; 
		private int _cornerWidth;
		private int _cornerHeight;
		private string _outputDirectory; 
		#endregion

		#region Properties
		[System.Xml.Serialization.XmlAttributeAttribute("TabColor")]
		public string TabColor
		{
			get { return _tabColor; }
			set { _tabColor = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("BorderColor")]
		public string BorderColor
		{
			get { return _borderColor; }
			set { _borderColor = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("SelectedTabColor")]
		public string SelectedTabColor
		{
			get { return _selectedTabColor; }
			set { _selectedTabColor = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("TabType")]
		public string TabType
		{
			get { return _tabType; }
			set { _tabType = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("TabCornerType")]
		public string TabCornerType
		{
			get { return _tabCornerType; }
			set { _tabCornerType = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("OutputDirectory")]
		public string OutputDirectory
		{
			get { return _outputDirectory; }
			set { _outputDirectory = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("BottomPadding")]
		public int BottomPadding
		{
			get { return _bottomPadding; }
			set { _bottomPadding = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("BorderWidth")]
		public int BorderWidth
		{
			get { return _borderWidth; }
			set { _borderWidth = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("TabWidth")]
		public int TabWidth
		{
			get { return _tabWidth; }
			set { _tabWidth = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("TabHeight")]
		public int TabHeight
		{
			get { return _tabHeight; }
			set { _tabHeight = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("CornerWidth")]
		public int CornerWidth
		{
			get { return _cornerWidth; }
			set { _cornerWidth = value; }
		}

		[System.Xml.Serialization.XmlAttributeAttribute("CornerHeight")]
		public int CornerHeight
		{
			get { return _cornerHeight; }
			set { _cornerHeight = value; }
		}

		#endregion
	}
}
