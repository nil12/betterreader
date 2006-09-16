using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	public class FeedItemsListViewColumnSorter : System.Collections.IComparer
	{
		/// <summary>
		/// Specifies the column to be sorted
		/// </summary>
		private int columnToSort;
		private bool smartSortEnabled;
		/// <summary>
		/// Specifies the order in which to sort (i.e. 'Ascending').
		/// </summary>
		private SortOrder orderOfSort;
		/// <summary>
		/// Case insensitive comparer object
		/// </summary>
		private System.Collections.CaseInsensitiveComparer objectCompare;

		public bool SmartSortEnabled
		{
			get { return smartSortEnabled; }
			set { smartSortEnabled = value; }
		}

		/// <summary>
		/// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
		/// </summary>
		public int SortColumn
		{
			set
			{
				columnToSort = value;
			}
			get
			{
				return columnToSort;
			}
		}

		/// <summary>
		/// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
		/// </summary>
		public SortOrder Order
		{
			set
			{
				orderOfSort = value;
			}
			get
			{
				return orderOfSort;
			}
		}
	

		

		/// <summary>
		/// Class constructor.  Initializes various elements
		/// </summary>
		public FeedItemsListViewColumnSorter()
		{
			// Initialize the column to '0'
			columnToSort = 0;

			// Initialize the sort order to 'none'
			orderOfSort = SortOrder.None;

			// Initialize the CaseInsensitiveComparer object
			objectCompare = new System.Collections.CaseInsensitiveComparer();

			smartSortEnabled = true;
		}

		/// <summary>
		/// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
		/// </summary>
		/// <param name="x">First object to be compared</param>
		/// <param name="y">Second object to be compared</param>
		/// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listViewX, listViewY;

			// Cast the objects to be compared to ListViewItem objects
			listViewX = (ListViewItem)x;
			listViewY = (ListViewItem)y;

			int smartSortResult = 0;

			if (smartSortEnabled)
			{
				smartSortResult = smartSort((FeedItem)listViewX.Tag, (FeedItem)listViewY.Tag);
			}

			if (smartSortResult != 0)
			{
				//use the smart sort result
				return smartSortResult;
			}

			// Compare the two items
			compareResult = objectCompare.Compare(listViewX.SubItems[columnToSort].Text, listViewY.SubItems[columnToSort].Text);

			// Calculate correct return value based on object comparison
			if (orderOfSort == SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if (orderOfSort == SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		private int smartSort(FeedItem feedItemX, FeedItem feedItemY)
		{
			int result = 0;

			if (feedItemX.HasBeenRead != feedItemY.HasBeenRead)
			{
				if (feedItemX.HasBeenRead)
				{
					result = 1;
				}
				else
				{
					result = -1;
				}
			}

			if (result == 0)
			{
			    if (feedItemX.PubDate != feedItemY.PubDate)
			    {
					if (feedItemX.PubDate == null)
					{
						result = -1;
					}
					else if (feedItemY.PubDate == null)
					{
						result = 1;
					}
					else
					{
						if (feedItemX.PubDate > feedItemY.PubDate)
						{
							result = -1;
						}
						else
						{
							result = 1;
						}
					}

				}

				if (result == 0)
				{
					if (feedItemX.DownloadDate != feedItemY.DownloadDate)
					{
						if (feedItemX.DownloadDate > feedItemY.DownloadDate)
						{
							result = -1;
						}
						else
						{
							result = 1;
						}
					}
				}
			}
			return result;
		}


	}
}
