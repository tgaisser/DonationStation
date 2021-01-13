using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Signpost.Interface
{
	public class Counter
	{
		private readonly LinkedList<CounterValue> newValueStack;
		private bool isProcessingStack;
		private bool isStepping;

		public Counter()
		{
			newValueStack = new LinkedList<CounterValue>();
			ThreadPool.QueueUserWorkItem(delegate
			{
				while (!isProcessingStack)
				{
					processStack();
					Thread.Sleep(100);
				}
			});
		}

		private void processStack()
		{
			if (isProcessingStack) return;
			if (isStepping) return;

			if (newValueStack.Count == 0) return;

			isProcessingStack = true;
			while (newValueStack.Count > 0)
			{
				if (!isStepping)
				{
					//int stackCount = newValueStack.Sum();
					//stepCounter(stackCount);
					//newValueStack.Clear();
					CounterValue counterValue = newValueStack.First.Value;
					newValueStack.RemoveFirst();
					stepCounter(counterValue);
				}
			}
			isProcessingStack = false;
		}

		private int countTo;
		private int countFrom;
		private int count;
		public int Count
		{
			get { return count; }
			set
			{
				count = value;
				OnCountChange(new EventArgs());
			}
		}

		private int stepValue = 0;
		public int StepValue
		{
			get { return stepValue; }
			set { stepValue = value; }
		}

		private int millisecondsBetweenStep = 0;
		public int MillisecondsBetweenStep
		{
			get { return millisecondsBetweenStep; }
			set { millisecondsBetweenStep = value; }
		}

		// Private delegate linked list (explicitly defined)
		private EventHandler<System.EventArgs> countChangeEventHandlerDelegate;

		// Main event definition
		public event EventHandler<System.EventArgs> CountChange
		{
			// Explicit event definition with accessor methods
			add
			{
				countChangeEventHandlerDelegate = (EventHandler<System.EventArgs>)Delegate.Combine(countChangeEventHandlerDelegate, value);
			}
			remove
			{
				countChangeEventHandlerDelegate = (EventHandler<System.EventArgs>)Delegate.Remove(countChangeEventHandlerDelegate, value);
			}
		}

		// This is the method that is responsible for notifying
		// receivers that the event occurred
		protected virtual void OnCountChange(EventArgs e)
		{
			if (countChangeEventHandlerDelegate != null)
			{
				countChangeEventHandlerDelegate(this, e);
			}
		}

		public void IncrementToCount(int newCount)
		{
			newValueStack.Clear();
			newValueStack.AddLast(new CounterValue{	CountType = CountType.SetCount, Number = newCount});
		}

		public void AddToCount(int addend)
		{
			if (addend == 0) return;

			if (newValueStack.Count > 0)
			{
				newValueStack.Last.Value.Number += addend;
			}
			else
			{
				newValueStack.AddLast(new CounterValue { CountType = CountType.Add, Number = addend });
			}
		}

		public void SubtractFromCount(int subtrahend)
		{
			if (subtrahend == 0) return;

			if (newValueStack.Count > 0)
			{
				newValueStack.Last.Value.Number += (0 - subtrahend);
			}
			else
			{
				newValueStack.AddLast(new CounterValue { CountType = CountType.Add, Number = (0 - subtrahend) });
			}
		}

		private async void stepCounter(CounterValue counterValue)
		{
			if (stepValue == 0 || millisecondsBetweenStep == 0)	// no stepping, just set value and return
			{
				if (counterValue.CountType == CountType.SetCount)
					Count = counterValue.Number;

				Count = count + counterValue.Number;
				return;
			}

			int oldCount = count;
			int newNumber;

			if (counterValue.CountType == CountType.SetCount)
				newNumber = counterValue.Number;
			else
				newNumber = oldCount + counterValue.Number;

			int modifiedStepValue = Math.Abs(stepValue);
			isStepping = true;

			if (oldCount > newNumber)	// Step it back
			{
				modifiedStepValue = 0 - modifiedStepValue;

				await Task.Run(() =>
				{
					for (int i = oldCount; i >= newNumber; i += modifiedStepValue)
					{
						Count = i;
						Thread.Sleep(millisecondsBetweenStep);
					}
				});
				if (count != newNumber) Count = newNumber;
			}
			else   // Go forward
			{
				await Task.Run(() =>
				{
					for (int i = oldCount; i <= newNumber; i += modifiedStepValue)
					{
						Count = i;
						Thread.Sleep(millisecondsBetweenStep);
					}
				});
				if (count != newNumber) Count = newNumber;
			}
			isStepping = false;
		}

	}

	public class CounterValue
	{
		public int Number { get; set; }
		public CountType CountType { get; set; }
	}

	public enum CountType
	{
		Add,
		SetCount
	}
}
