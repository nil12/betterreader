using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace BetterReader
{
	public class MainFormState
	{
		private Point windowLocation;
		private int splitContainer1SplitterDistance;
		private int splitContainer2SplitterDistance;
		private int splitContainer3SplitterDistance;
		private int splitContainer4SplitterDistance;
		private int splitContainer5SplitterDistance;
		private Size windowSize;
		private FormWindowState windowState;

		public Point WindowLocation
		{
			get
			{
				return windowLocation;
			}
			set
			{
				windowLocation = value;
			}
		}

		public int SplitContainer1SplitterDistance
		{
			get
			{
				return splitContainer1SplitterDistance;
			}
			set
			{
				splitContainer1SplitterDistance = value;
			}
		}

		public int SplitContainer2SplitterDistance
		{
			get
			{
				return splitContainer2SplitterDistance;
			}
			set
			{
				splitContainer2SplitterDistance = value;
			}
		}

		public int SplitContainer3SplitterDistance
		{
			get
			{
				return splitContainer3SplitterDistance;
			}
			set
			{
				splitContainer3SplitterDistance = value;
			}
		}

		public int SplitContainer4SplitterDistance
		{
			get
			{
				return splitContainer4SplitterDistance;
			}
			set
			{
				splitContainer4SplitterDistance = value;
			}
		}

		public int SplitContainer5SplitterDistance
		{
			get
			{
				return splitContainer5SplitterDistance;
			}
			set
			{
				splitContainer5SplitterDistance = value;
			}
		}

		public Size WindowSize
		{
			get
			{
				return windowSize;
			}
			set
			{
				windowSize = value;
			}
		}

		public FormWindowState WindowState
		{
			get
			{
				return windowState;
			}
			set
			{
				windowState = value;
			}
		}




		internal static MainFormState Load(string filepath)
		{
			MainFormState mfs = null;

			if (File.Exists(filepath))
			{
				using (TextReader tr = new StreamReader(filepath))
				{
					XmlSerializer xs = new XmlSerializer(typeof(MainFormState));
					try
					{
						mfs = (MainFormState)xs.Deserialize(tr);
					}
					catch
					{
						//error reading file, no big deal
						mfs = null;
					}
				}
			}

			if (mfs == null)
			{
				mfs = new MainFormState();
				mfs.windowLocation = new Point(0, 0);
				mfs.splitContainer1SplitterDistance = 310;
				mfs.splitContainer2SplitterDistance = 37;
				mfs.splitContainer3SplitterDistance = 268;
				mfs.splitContainer4SplitterDistance = 49;
				mfs.splitContainer5SplitterDistance = 37;
				mfs.windowSize = new Size(941, 758);
				mfs.windowState = FormWindowState.Normal;
			}

			return mfs;
		}

		internal void Save(string filepath)
		{
			using (TextWriter tw = new StreamWriter(filepath))
			{
				XmlSerializer xs = new XmlSerializer(typeof(MainFormState));
				xs.Serialize(tw, this);
			}
		}
	}
}
