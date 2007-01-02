using System;
using System.Diagnostics;
using System.Drawing;
//using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
//using System.Data;
using System.Runtime.InteropServices;
//using System.Windows.Forms.Design;

namespace Sloppycode.UI
{
	// - Implements:
	//   + Auto scrolling
	//   + Target node highlighting when over a node
	//   + Custom cursor when dragging
	//   + Custom ghost icon + label when dragging
	//   + Escape key to cancel drag
	//	 + Blocks certain nodes from being dragged via cancel event
	//   + Sanity checks for dragging (no parent into children nodes, target isn't the source)

	// Gotchas:
	// - Explorer can tell if you have the treeview node selected or not
	// - The drag icon has to be dragged to the right, not in the center (or the form has 
	//  a fight with the treeview over focus)
	// - No auto opening of items

	//Modified by Steve Kain for the BetterReader project

	#region TreeViewDragDrop class

	/// <summary>
	/// A treeview with inbuilt drag-drop support and custom cursor/icon dragging.
	/// </summary>
	[
		ToolboxBitmap(typeof (TreeViewDragDrop), "Sloppycode.UI.DragDropTreeView.bmp"),
			Description("A treeview with inbuilt drag-drop support and custom cursor/icon dragging.")
		]
	public class TreeViewDragDrop : TreeView
	{
		#region Win32 api import, events

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, int lParam);

		/// <summary>
		/// Occurs when an item is starting to be dragged. This
		/// event can be used to cancel dragging of particular items.
		/// </summary>
		[
			Description(
				"Occurs when an item is starting to be dragged. This event can be used to cancel dragging of particular items."),
			]
		public event DragItemEventHandler DragStart;

		/// <summary>
		/// Occurs when an item is dragged and dropped onto another.
		/// </summary>
		[
			Description("Occurs when an item is dragged and dropped onto another."),
			]
		public event DragCompleteEventHandler DragComplete;

		/// <summary>
		/// Occurs when an item is dragged, and the drag is cancelled.
		/// </summary>
		[
			Description("Occurs when an item is dragged, and the drag is cancelled."),
			]
		public event DragItemEventHandler DragCancel;

		#endregion

		#region Public properties

		/// <summary>
		/// The imagelist control from which DragImage icons are taken.
		/// </summary>
		[
			Description("The imagelist control from which DragImage icons are taken."),
				Category("Drag and drop")
			]
		public ImageList DragImageList
		{
			get { return _formDrag.imageList1; }
			set
			{
				if (value == _formDrag.imageList1)
				{
					return;
				}

				_formDrag.imageList1 = value;

				// Change the picture box to use this image
				if (_formDrag.imageList1.Images.Count > 0 && _formDrag.imageList1.Images[_dragImageIndex] != null)
				{
					_formDrag.pictureBox1.Image = _formDrag.imageList1.Images[_dragImageIndex];
					_formDrag.Height = _formDrag.pictureBox1.Image.Height;
				}

				if (!base.IsHandleCreated)
				{
					return;
				}
				SendMessage((IntPtr) 4361, 0, ((value == null) ? IntPtr.Zero : value.Handle), 0);
			}
		}

		/// <summary>
		/// The default image index for the DragImage icon.
		/// </summary>
		[
			Description("The default image index for the DragImage icon."),
				Category("Drag and drop"),
				TypeConverter(typeof (ImageIndexConverter)),
				Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof (UITypeEditor))
			]
		public int DragImageIndex
		{
			get
			{
				if (_formDrag.imageList1 == null)
				{
					return -1;
				}

				if (_dragImageIndex >= _formDrag.imageList1.Images.Count)
				{
					return Math.Max(0, (_formDrag.imageList1.Images.Count - 1));
				}
				else

					return _dragImageIndex;
			}
			set
			{
				// Change the picture box to use this image
				if (_formDrag.imageList1.Images.Count > 0 && _formDrag.imageList1.Images[value] != null)
				{
					_formDrag.pictureBox1.Image = _formDrag.imageList1.Images[value];
					_formDrag.Size = new Size(_formDrag.Width, _formDrag.pictureBox1.Image.Height);
					_formDrag.labelText.Size = new Size(_formDrag.labelText.Width, _formDrag.pictureBox1.Image.Height);
				}

				_dragImageIndex = value;
			}
		}

		/// <summary>
		/// The custom cursor to use when dragging an item, if DragCursor is set to Custom.
		/// </summary>
		[
			Description("The custom cursor to use when dragging an item, if DragCursor is set to Custom."),
				Category("Drag and drop")
			]
		public Cursor DragCursor
		{
			get { return _dragCursor; }
			set
			{
				if (value == _dragCursor)
				{
					return;
				}

				_dragCursor = value;
				if (!base.IsHandleCreated)
				{
					return;
				}
			}
		}

		/// <summary>
		/// The cursor type to use when dragging - None uses the default drag and drop cursor, DragIcon uses an icon and label, Custom uses a custom cursor.
		/// </summary>
		[
			Description(
				"The cursor type to use when dragging - None uses the default drag and drop cursor, DragIcon uses an icon and label, Custom uses a custom cursor."
				),
				Category("Drag and drop")
			]
		public DragCursorType DragCursorType
		{
			get { return _dragCursorType; }
			set { _dragCursorType = value; }
		}

		/// <summary>
		/// Sets the font for the dragged node (shown as ghosted text/icon).
		/// </summary>
		[
			Description("Sets the font for the dragged node (shown as ghosted text/icon)."),
				Category("Drag and drop")
			]
		public Font DragNodeFont
		{
			get { return _formDrag.labelText.Font; }
			set
			{
				_formDrag.labelText.Font = value;

				// Set the drag form height to the font height
				_formDrag.Size = new Size(_formDrag.Width, (int) _formDrag.labelText.Font.GetHeight());
				_formDrag.labelText.Size =
					new Size(_formDrag.labelText.Width, (int) _formDrag.labelText.Font.GetHeight());
			}
		}

		/// <summary>
		/// Sets the opacity for the dragged node (shown as ghosted text/icon).
		/// </summary>
		[
			Description("Sets the opacity for the dragged node (shown as ghosted text/icon)."),
				Category("Drag and drop"),
				TypeConverter(typeof (OpacityConverter))
			]
		public double DragNodeOpacity
		{
			get { return _formDrag.Opacity; }
			set { _formDrag.Opacity = value; }
		}

		/// <summary>
		/// The background colour of the node being dragged over.
		/// </summary>
		[
			Description("The background colour of the node being dragged over."),
				Category("Drag and drop")
			]
		public Color DragOverNodeBackColor
		{
			get { return _dragOverNodeBackColor; }
			set { _dragOverNodeBackColor = value; }
		}

		/// <summary>
		/// The foreground colour of the node being dragged over.
		/// </summary>
		[
			Description("The foreground colour of the node being dragged over."),
				Category("Drag and drop")
			]
		public Color DragOverNodeForeColor
		{
			get { return _dragOverNodeForeColor; }
			set { _dragOverNodeForeColor = value; }
		}

		/// <summary>
		/// The drag mode (move,copy etc.)
		/// </summary>
		[
			Description("The drag mode (move,copy etc.)"),
				Category("Drag and drop")
			]
		public DragDropEffects DragMode
		{
			get { return _dragMode; }
			set { _dragMode = value; }
		}

		/// <summary>
		/// If this value is set to true (the default) then the WndProc override will replace a background erase message 
		/// with a null message.  If false then the background erase message is faithfully passed on to the underlying
		/// base method.
		/// </summary>
		public bool DisableBackgroundErase
		{
			get { return disableBackgroundErase; }
			set { disableBackgroundErase = value; }
		}

		#endregion

		#region Private members

		private int _dragImageIndex;
		private DragDropEffects _dragMode = DragDropEffects.Move;
		private Color _dragOverNodeForeColor = SystemColors.HighlightText;
		private Color _dragOverNodeBackColor = SystemColors.Highlight;
		private DragCursorType _dragCursorType;
		private Cursor _dragCursor = null;
		private TreeNode _previousNode;
		private TreeNode _selectedNode;
		private FormDrag _formDrag = new FormDrag();
		private bool disableBackgroundErase = true;

		#endregion

		#region Constructor

		public TreeViewDragDrop()
		{
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			AllowDrop = true;

			// Set the drag form to have ambient properties
			_formDrag.labelText.Font = Font;
			_formDrag.BackColor = BackColor;

			// Custom cursor handling
			if (_dragCursorType == DragCursorType.Custom && _dragCursor != null)
			{
				DragCursor = _dragCursor;
			}

			_formDrag.Show();
			_formDrag.Visible = false;
		}

		#endregion

		#region Over-ridden methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			if (disableBackgroundErase)
			{
				// Stop erase background message
				if (m.Msg == 0x0014)
				{
					m.Msg = 0x0000; // Set to null
				}
			}

			base.WndProc(ref m);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			if (e.Effect == _dragMode)
			{
				e.UseDefaultCursors = false;

				if (_dragCursorType == DragCursorType.Custom && _dragCursor != null)
				{
					// Custom cursor
					Cursor = _dragCursor;
				}
				else if (_dragCursorType == DragCursorType.DragIcon)
				{
					// This removes the default drag + drop cursor
					Cursor = Cursors.Default;
				}
				else
				{
					e.UseDefaultCursors = true;
				}
			}
			else
			{
				e.UseDefaultCursors = true;
				Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			_selectedNode = (TreeNode) e.Item;

			// Call dragstart event
			if (DragStart != null)
			{
				DragItemEventArgs ea = new DragItemEventArgs();
				ea.Node = _selectedNode;

				DragStart(this, ea);
			}
			// Change any previous node back 
			if (_previousNode != null)
			{
				_previousNode.BackColor = BackColor;
				_previousNode.ForeColor = ForeColor;
			}

			// Move the form with the icon/label on it
			// A better width measurement algo for the form is needed here

			int width = Width = _selectedNode.Text.Length*(int) _formDrag.labelText.Font.Size;
			if (_selectedNode.Text.Length < 5)
				width += 20;

			_formDrag.Size = new Size(width, _formDrag.Height);

			_formDrag.labelText.Size = new Size(width, _formDrag.labelText.Size.Height);
			_formDrag.labelText.Text = _selectedNode.Text;

			// Start drag drop
			DoDragDrop(e.Item, _dragMode);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragOver(DragEventArgs e)
		{
			// Change any previous node back
			if (_previousNode != null)
			{
				_previousNode.BackColor = BackColor;
				_previousNode.ForeColor = ForeColor;
			}

			// Get the node from the mouse position, colour it
			Point pt = PointToClient(new Point(e.X, e.Y));
			TreeNode treeNode = GetNodeAt(pt);
			treeNode.BackColor = _dragOverNodeBackColor;
			treeNode.ForeColor = _dragOverNodeForeColor;

			// Move the icon form
			if (_dragCursorType == DragCursorType.DragIcon)
			{
				_formDrag.Location = new Point(e.X + 5, e.Y - 5);
				_formDrag.Visible = true;
			}

			// Scrolling down/up
			if (pt.Y + 10 > ClientSize.Height)
				SendMessage(Handle, 277, (IntPtr) 1, 0);
			else if (pt.Y < Top + 10)
				SendMessage(Handle, 277, (IntPtr) 0, 0);

			// Remember the target node, so we can set it back
			_previousNode = treeNode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragLeave(EventArgs e)
		{
			if (_selectedNode != null)
			{
				SelectedNode = _selectedNode;
			}

			if (_previousNode != null)
			{
				_previousNode.BackColor = _dragOverNodeBackColor;
				_previousNode.ForeColor = _dragOverNodeForeColor;
			}

			_formDrag.Visible = false;
			Cursor = Cursors.Default;

			// Call cancel event
			if (DragCancel != null)
			{
				DragItemEventArgs ea = new DragItemEventArgs();
				ea.Node = _selectedNode;

				DragCancel(this, ea);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragEnter(DragEventArgs e)
		{
			e.Effect = _dragMode;

			// Reset the previous node var
			_previousNode = null;
			_selectedNode = null;
			Debug.WriteLine(_formDrag.labelText.Size);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragDrop(DragEventArgs e)
		{
			handleCustomCursor();

			// Check it's a treenode being dragged

			TreeNode dragNode = getTreeNodeFromDragEventArgs(e);
			if (dragNode != null)
			{
				// Get the target node from the mouse coords
				TreeNode targetNode = getTargetNodeFromDragEventArgs(e);

				// De-color it
				decolorNode(targetNode);


				if (dragIsValid(dragNode, targetNode))
				{
					moveNodeToNewParent(dragNode, targetNode);

					doPostDragTasks(dragNode, targetNode);
				}
			}
		}

		protected void moveNodeToNewParent(TreeNode dragNode, TreeNode targetNode)
		{
			// Copy the node, add as a child to the destination node
			TreeNode newTreeNode = (TreeNode) dragNode.Clone();
			targetNode.Nodes.Add(newTreeNode);
			targetNode.Expand();

			// Remove Original Node, set the dragged node as selected
			dragNode.Remove();
			SelectedNode = newTreeNode;
		}

		protected void doPostDragTasks(TreeNode dragNode, TreeNode targetNode)
		{
			Cursor = Cursors.Default;

			// Call drag complete event
			if (DragComplete != null)
			{
				DragCompleteEventArgs ea = new DragCompleteEventArgs();
				ea.SourceNode = dragNode;
				ea.TargetNode = targetNode;

				DragComplete(this, ea);
			}
		}

		protected void decolorNode(TreeNode targetNode)
		{
			targetNode.BackColor = BackColor;
			targetNode.ForeColor = ForeColor;
		}


		/// <summary>
		/// 1) Check we're not dragging onto ourself
		/// 2) Check we're not dragging onto one of our children 
		/// (this is the lazy way, will break if there are nodes with the same name,
		/// but it's quicker than checking all nodes below is)
		/// 3) Check we're not dragging onto our parent
		/// </summary>
		/// <param name="dragNode"></param>
		/// <param name="targetNode"></param>
		/// <returns></returns>
		protected bool dragIsValid(TreeNode dragNode, TreeNode targetNode)
		{
			return targetNode != dragNode && !targetNode.FullPath.StartsWith(dragNode.FullPath) &&
			       dragNode.Parent != targetNode;
		}

		protected TreeNode getTargetNodeFromDragEventArgs(DragEventArgs e)
		{
			Point pt = PointToClient(new Point(e.X, e.Y));
			TreeNode targetNode = GetNodeAt(pt);
			return targetNode;
		}

		protected TreeNode getTreeNodeFromDragEventArgs(DragEventArgs e)
		{
			if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
			{
				return (TreeNode) e.Data.GetData("System.Windows.Forms.TreeNode");
			}
			else
			{
				return null;
			}
		}

		protected void handleCustomCursor()
		{
			if (_dragCursorType == DragCursorType.DragIcon)
			{
				Cursor = Cursors.Default;
			}

			_formDrag.Visible = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				if (_selectedNode != null)
				{
					SelectedNode = _selectedNode;
				}

				if (_previousNode != null)
				{
					_previousNode.BackColor = BackColor;
					_previousNode.ForeColor = ForeColor;
				}

				Cursor = Cursors.Default;
				_formDrag.Visible = false;

				// Call cancel event
				if (DragCancel != null)
				{
					DragItemEventArgs ea = new DragItemEventArgs();
					ea.Node = _selectedNode;

					DragCancel(this, ea);
				}
			}
		}

		#endregion

		#region FormDrag form

		internal class FormDrag : Form
		{
			#region Components

			public Label labelText;
			public PictureBox pictureBox1;
			public ImageList imageList1;
			private Container components = null;

			#endregion

			#region Constructor, dispose

			public FormDrag()
			{
				InitializeComponent();
			}

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (components != null)
					{
						components.Dispose();
					}
				}
				base.Dispose(disposing);
			}

			#endregion

			#region Windows Form Designer generated code

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				components = new System.ComponentModel.Container();
				labelText = new System.Windows.Forms.Label();
				pictureBox1 = new System.Windows.Forms.PictureBox();
				imageList1 = new System.Windows.Forms.ImageList(components);
				SuspendLayout();
				// 
				// labelText
				// 
				labelText.BackColor = System.Drawing.Color.Transparent;
				labelText.Location = new System.Drawing.Point(16, 2);
				labelText.Name = "labelText";
				labelText.Size = new System.Drawing.Size(100, 16);
				labelText.TabIndex = 0;
				// 
				// pictureBox1
				// 
				pictureBox1.Location = new System.Drawing.Point(0, 0);
				pictureBox1.Name = "pictureBox1";
				pictureBox1.Size = new System.Drawing.Size(16, 16);
				pictureBox1.TabIndex = 1;
				pictureBox1.TabStop = false;
				// 
				// Form2
				// 
				AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				BackColor = System.Drawing.SystemColors.Control;
				ClientSize = new System.Drawing.Size(100, 16);
				Controls.Add(pictureBox1);
				Controls.Add(labelText);
				Size = new Size(300, 500);
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				Opacity = 0.3;
				ShowInTaskbar = false;
				ResumeLayout(false);
			}

			#endregion
		}

		#endregion
	}

	#endregion

	#region DragCursorType enum

	[Serializable]
	public enum DragCursorType
	{
		None,
		DragIcon,
		Custom
	}

	#endregion

	#region Event classes/delegates

	public delegate void DragCompleteEventHandler(object sender, DragCompleteEventArgs e);

	public delegate void DragItemEventHandler(object sender, DragItemEventArgs e);

	public class DragCompleteEventArgs : EventArgs
	{
		/// <summary>
		/// The node that was being dragged
		/// </summary>
		public TreeNode SourceNode
		{
			get { return _sourceNode; }
			set { _sourceNode = value; }
		}

		/// <summary>
		/// The node that the source node was dragged onto.
		/// </summary>
		public TreeNode TargetNode
		{
			get { return _targetNode; }
			set { _targetNode = value; }
		}

		private TreeNode _targetNode;
		private TreeNode _sourceNode;
	}

	public class DragItemEventArgs : EventArgs
	{
		/// <summary>
		/// The ndoe that was being dragged
		/// </summary>
		public TreeNode Node
		{
			get { return _node; }
			set { _node = value; }
		}

		private TreeNode _node;
	}

	#endregion
}
