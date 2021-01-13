using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloserLook.Data;

namespace DonationStation.Interface
{
	public class DataMonitor
	{
		public DataMonitor(string ConnectionString = "")
		{
			dataContext = 
				string.IsNullOrEmpty(ConnectionString)
				? new CloserLookDataContext()
				: new CloserLookDataContext(ConnectionString);

			// STart Timer
			ThreadPool.QueueUserWorkItem(delegate
			{
				while (!isCheckingDataCount)
				{
					CheckLatestCount();
					Thread.Sleep(millisecondsBetweenScanCheck);
				}
			});

		}

		private CloserLookDataContext dataContext;
		private int currentCount;
		private int previousCount;
		private bool isCheckingDataCount;

		public int CurrentCount
		{
			get { return currentCount; }
		}

		public int PreviousCount
		{
			get { return previousCount; }
		}

		public string EngagementId { get; set; }

		public int PerScanMultiplier
		{
			get { return perScanMultiplier; }
			set { perScanMultiplier = value; }
		}

		public int MillisecondsBetweenScanCheck
		{
			get { return millisecondsBetweenScanCheck; }
			set
			{
				if (value < 1)
				{
					throw new Exception("Must have a value greater than 0 for Scan Frequency Milliseconds.");
				}
				millisecondsBetweenScanCheck = value;
			}
		}

		// Private delegate linked list (explicitly defined)
		private EventHandler<System.EventArgs> CountChangedEventHandlerDelegate;

		// Main event definition
		public event EventHandler<System.EventArgs> CountChanged
		{
			// Explicit event definition with accessor methods
			add
			{
				CountChangedEventHandlerDelegate = (EventHandler<System.EventArgs>)Delegate.Combine(CountChangedEventHandlerDelegate, value);
			}
			remove
			{
				CountChangedEventHandlerDelegate = (EventHandler<System.EventArgs>)Delegate.Remove(CountChangedEventHandlerDelegate, value);
			}
		}

		// This is the method that is responsible for notifying
		// receivers that the event occurred
		protected virtual void OnCountChanged(System.EventArgs e)
		{
			if (CountChangedEventHandlerDelegate != null)
			{
				CountChangedEventHandlerDelegate(this, e);
			}
		}

		public void CheckLatestCount()
		{
			isCheckingDataCount = true;
			int latestCount;

			latestCount = simulateCountIncrease() * perScanMultiplier;

			//try
			//{
			//	latestCount = (string.IsNullOrEmpty(EngagementId) 
			//		? dataContext.BadgeScans.Count(x => x.EngagementId == "UNKNOWN ENGAGEMENT ID") 
			//		: dataContext.BadgeScans.Count(x => x.EngagementId == EngagementId))
			//		* perScanMultiplier;
			//}
			//catch (Exception e)
			//{
			//	latestCount = 0;
			//}

			if (currentCount != latestCount)
			{
				previousCount = currentCount;
				currentCount = latestCount;
				OnCountChanged(new EventArgs());
			}
			isCheckingDataCount = false;
		}


		private int simulatedCount ;
		private int perScanMultiplier = 1;
		private int millisecondsBetweenScanCheck=1000;	// Default of 1 second

		private int simulateCountIncrease()
		{
			var r = new Random();
			if (r.Next(0, 3) == 0)
			{
				simulatedCount += r.Next(0, 4);
			}
			return simulatedCount;
		}

	}
}
