using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BetterReader.Backend;

namespace BetterReader.UIManagers
{
	/// <summary>
	/// FeedItemsListManager is a helper class to mediate between the FeedItems ListView UI and the FeedItemCollection
	/// data structure.
	/// </summary>
	class FeedItemsListManager
	{
		private Font feedItemsNormalFont;
		private Font feedItemsBoldFont;
		private ListView feedItemsLV;
		private FeedSubscription currentlyDisplayedFeedSubscription;
		private Label itemLinkLBL;
		private Label itemTitleLBL;
		private ToolStripButton showUnreadFirstBTN;
		private ToolStripLabel lastDownloadLBL;
		private ToolStripButton markAllReadBTN;
		private Dictionary<FeedItem, ListViewItem> listViewItemsByTag;
		private FeedSubTreeManager feedSubManager;

		public FeedSubscription CurrentlyDisplayedFeedSubscription
		{
			get { return currentlyDisplayedFeedSubscription; }
			set { currentlyDisplayedFeedSubscription = value; }
		}

		public FeedItemsListManager(ListView lFeedItemsLV, Label lItemLinkLBL, Label lItemTitleLBL, ToolStripLabel lLastDownloadLBL,
			ToolStripButton lShowUnreadFirstBTN, ToolStripButton lMarkAllReadBTN, FeedSubTreeManager lFeedSubManager)
		{
			feedItemsLV = lFeedItemsLV;
			itemLinkLBL = lItemLinkLBL;
			itemTitleLBL = lItemTitleLBL;
			lastDownloadLBL = lLastDownloadLBL;
			showUnreadFirstBTN = lShowUnreadFirstBTN;
			markAllReadBTN = lMarkAllReadBTN;
			feedItemsNormalFont = feedItemsLV.Font;
			feedItemsBoldFont = new Font(feedItemsNormalFont, FontStyle.Bold);
			currentlyDisplayedFeedSubscription = null;
			feedSubManager = lFeedSubManager;
		}

		public void BindFeedItemsToListView(FeedItemCollection feedItems)
		{
			//feedItemsLV.SuspendLayout();
			addFeedItemColumnsToListView(currentlyDisplayedFeedSubscription.Feed.IncludedFeedItemProperties);
			if (feedItems.Count < 1)
			{
				feedItemsLV.Items.Add(new ListViewItem("No items found.")).ImageIndex = 2;
				feedItemsLV.Enabled = false;
				return;
			}

			feedItemsLV.Enabled = true;
			foreach (FeedItem fi in feedItems)
			{
				ListViewItem lvi = new ListViewItem(fi.Title);
				lvi.Tag = fi;
				lvi.IndentCount = 0;
				setListViewItemSubItems(lvi, fi, currentlyDisplayedFeedSubscription.Feed.IncludedFeedItemProperties);
				if (fi.HasBeenRead)
				{
					lvi.Font = feedItemsNormalFont;
				}
				else
				{
					lvi.Font = feedItemsBoldFont;
				}


				lvi.ImageIndex = 2;

				feedItemsLV.Items.Add(lvi);
				listViewItemsByTag.Add(fi, lvi);
			}

			setFeedItemColumnWidths();
			//feedItemsLV.ResumeLayout();
		}

		public void DisplayFeedItems(FeedSubscription feedSubscription)
		{
			//lock (feedItemsLV)
			//{
			currentlyDisplayedFeedSubscription = feedSubscription;
			itemLinkLBL.Visible = false;
			itemTitleLBL.Visible = false;
			feedSubscription.ResetUpdateTimer();
			feedItemsLV.ListViewItemSorter = currentlyDisplayedFeedSubscription.ColumnSorter;
			showUnreadFirstBTN.Visible = true;
			showUnreadFirstBTN.Checked = currentlyDisplayedFeedSubscription.ColumnSorter.SmartSortEnabled;
			lastDownloadLBL.Visible = true;
			lastDownloadLBL.Text = "Last Downloaded: " + feedSubscription.Feed.LastDownloadAttempt.ToString();
			markAllReadBTN.Visible = true;

			listViewItemsByTag = new Dictionary<FeedItem, ListViewItem>();
			//feedItemsLV.BeginUpdate();
			feedItemsLV.Clear();
			if (feedSubscription.Feed.ReadSuccess)
			{
				BindFeedItemsToListView(feedSubscription.Feed.FeedItems);
				feedItemsLV.Enabled = true;
			}
			else
			{
				if (feedSubscription.Feed.ReadException == null)
				{
					feedItemsLV.Columns.Add("Title");
					ListViewItem lvi = new ListViewItem("Loading");
					feedItemsLV.Items.Add(lvi);
					feedItemsLV.Enabled = false;
				}
				else
				{
					feedItemsLV.Columns.Add("Error");
					ListViewItem lvi = new ListViewItem(feedSubscription.Feed.ReadException.ToString());
					feedItemsLV.Items.Add(lvi);
					feedItemsLV.Enabled = false;
				}

				//feedItemsLV.EndUpdate();
				return;
			}
			feedItemsLV.Sort();

			clearColumnHeaderIcons();
			if (currentlyDisplayedFeedSubscription.ColumnSorter.SortColumn < feedItemsLV.Columns.Count)
			{
				feedItemsLV.Columns[currentlyDisplayedFeedSubscription.ColumnSorter.SortColumn].ImageIndex =
						getArrowImageIndexForSortColumn(currentlyDisplayedFeedSubscription.ColumnSorter);
			}
			else
			{
				MessageBox.Show("A recoverable error has been encountered: The current sort column index (" +
					currentlyDisplayedFeedSubscription.ColumnSorter.SortColumn.ToString() + ") is greater than " +
					"the number of columns in the FeedItems ListView (" + feedItemsLV.Columns.Count.ToString() + ")");
			}
			//feedItemsLV.EndUpdate();
			//}
		}


		private void clearColumnHeaderIcons()
		{
			foreach (ColumnHeader ch in feedItemsLV.Columns)
			{
				ch.ImageIndex = -1;
			}
		}


		private int getArrowImageIndexForSortColumn(FeedItemsListViewColumnSorter lvwColumnSorter)
		{
			int arrowImageIndex;
			if (lvwColumnSorter.Order == SortOrder.Ascending)
			{
				arrowImageIndex = 0;
			}
			else
			{
				arrowImageIndex = 1;
			}
			return arrowImageIndex;
		}



		private void setFeedItemColumnWidths()
		{
			foreach (ColumnHeader ch in feedItemsLV.Columns)
			{
				ch.Width = -1;
			}
		}


		private void setListViewItemSubItems(ListViewItem lvi, FeedItem fi, FeedItemProperties itemProps)
		{
			if ((itemProps & FeedItemProperties.PubDate) == FeedItemProperties.PubDate)
			{
				lvi.SubItems.Add(fi.PubDate.ToString());
			}

			if ((itemProps & FeedItemProperties.HasBeenRead) == FeedItemProperties.HasBeenRead)
			{
				lvi.SubItems.Add(fi.HasBeenRead.ToString());
			}

			if (((itemProps & FeedItemProperties.DownloadDate) == FeedItemProperties.DownloadDate) &&
				((itemProps & FeedItemProperties.PubDate) != FeedItemProperties.PubDate))
			{
				//only show downloadDate if pubDate is unavailable
				lvi.SubItems.Add(fi.DownloadDate.ToString());
			}

			if ((itemProps & FeedItemProperties.Category) == FeedItemProperties.Category)
			{
				lvi.SubItems.Add(fi.Category.ToString());
			}

			if ((itemProps & FeedItemProperties.Author) == FeedItemProperties.Author)
			{
				lvi.SubItems.Add(fi.Author.ToString());
			}
		}


		private void addFeedItemColumnsToListView(FeedItemProperties itemProps)
		{
			feedItemsLV.Columns.Add("Title");

			if ((itemProps & FeedItemProperties.PubDate) == FeedItemProperties.PubDate)
			{
				feedItemsLV.Columns.Add("PubDate");
			}

			if ((itemProps & FeedItemProperties.HasBeenRead) == FeedItemProperties.HasBeenRead)
			{
				feedItemsLV.Columns.Add("Read");
			}

			if (((itemProps & FeedItemProperties.DownloadDate) == FeedItemProperties.DownloadDate) &&
				((itemProps & FeedItemProperties.PubDate) != FeedItemProperties.PubDate))
			{
				//only show downloadDate if pubDate is unavailable
				feedItemsLV.Columns.Add("DownloadDate");
			}

			if ((itemProps & FeedItemProperties.Category) == FeedItemProperties.Category)
			{
				feedItemsLV.Columns.Add("Category");
			}

			if ((itemProps & FeedItemProperties.Author) == FeedItemProperties.Author)
			{
				feedItemsLV.Columns.Add("Author");
			}

		}

		internal void MarkFeedItemRead(FeedItem fi)
		{
			if (fi.HasBeenRead == false)
			{
				fi.MarkRead();
			}
			feedItemsLV.SelectedItems[0].Font = feedItemsNormalFont;
			feedSubManager.UpdateNodeFromFeedSubscription(fi.ParentFeed.ParentSubscription);
			fi.ParentFeed.FeedItems.ArchiveItems();
		}

		public void SortFeedItemsLV(int columnIndex)
		{
			FeedItemsListViewColumnSorter lvwColumnSorter = (FeedItemsListViewColumnSorter)feedItemsLV.ListViewItemSorter;

			clearColumnHeaderIcons();


			// Determine if clicked column is already the column that is being sorted.
			if (columnIndex == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;

				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.SortColumn = columnIndex;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			int arrowImageIndex = getArrowImageIndexForSortColumn(lvwColumnSorter);

			feedItemsLV.Columns[columnIndex].ImageIndex = arrowImageIndex;

			// Perform the sort with these new sort options.
			feedItemsLV.Sort();
		}
	}
}
