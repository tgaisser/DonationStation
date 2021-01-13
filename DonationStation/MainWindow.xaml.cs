using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DonationStation.Interface;
using Signpost.Interface;

namespace Signpost
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Counter counter = new Counter { StepValue = DisplayConfiguration.IncrementBy, MillisecondsBetweenStep = DisplayConfiguration.MillisecondsBetweenIncrement };
		private readonly DataMonitor dataMonitor = new DataMonitor(DisplayConfiguration.GetSettingValue("DatabaseConnection"));

		private readonly List<string> animRunningList = new List<string>(); 
		private CachedBitmap[] cachedBitmaps = new CachedBitmap[10];

		public MainWindow()
		{
			InitializeComponent();

			this.Cursor = Cursors.None;

			this.Width = DisplayConfiguration.WindowWidth;
			this.Height = DisplayConfiguration.WindowHeight;
			this.counterPanel.Margin = DisplayConfiguration.MainTextTop;

			populateCachedImages();
			dataMonitor.EngagementId = DisplayConfiguration.GetSettingValue("EngagementId");
			int msBetweenScan;
			int.TryParse(DisplayConfiguration.GetSettingValue("MillisecondsBetweenScanCheck", "1"), out msBetweenScan);
			dataMonitor.MillisecondsBetweenScanCheck = msBetweenScan;

			int scanMulti;
			int.TryParse(DisplayConfiguration.GetSettingValue("PerScanDonationMultiplier", "1"), out scanMulti);
			dataMonitor.PerScanMultiplier = scanMulti;

			if (!string.IsNullOrWhiteSpace(DisplayConfiguration.BackgroundImage))
			{
				ImageBrush imgBrush =
					new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), DisplayConfiguration.BackgroundImage)))
					{
						Opacity = DisplayConfiguration.BackgroundOpacity
					};

				MainGrid.Background = imgBrush;
			}
			else
			{
				MainGrid.Background = DisplayConfiguration.BackgroundColorBrush;
			}

			counter.CountChange += counter_CountChange;
			dataMonitor.CountChanged += dataMonitor_CountChanged;

			counter.Count = DisplayConfiguration.NumericStartingValue;
			UpdateText(DisplayConfiguration.LeadingCharacter + DisplayConfiguration.StartingValue);
		}

		private void populateCachedImages()
		{
			if (! DisplayConfiguration.IsUsingNumberGraphics) return;

			for (int i = 0; i < 10; i++)
			{
				BitmapSource bitmapSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), DisplayConfiguration.NumberGraphics[i]));
				CachedBitmap cbBitmap = new CachedBitmap(bitmapSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

				cachedBitmaps[i] = cbBitmap;
			}
		}

		void counter_CountChange(object sender, EventArgs e)
		{
			UpdateText(DisplayConfiguration.LeadingCharacter + counter.Count.ToString());
		}

		void dataMonitor_CountChanged(object sender, EventArgs e)
		{
			counter.IncrementToCount(DisplayConfiguration.NumericStartingValue + dataMonitor.CurrentCount);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			//x UpdateCounter();
			if (!string.IsNullOrWhiteSpace(DisplayConfiguration.BackgroundVideo))
			{
				_meBackground.LoadedBehavior = MediaState.Manual;

				string backgroundVid = DisplayConfiguration.BackgroundVideo;
				if (! (backgroundVid.StartsWith(".") || backgroundVid.StartsWith("/") || backgroundVid.StartsWith("\\")))
				{
					backgroundVid = "./" + backgroundVid;
				}
				_meBackground.Source = new Uri( Path.GetFullPath(backgroundVid));
				_meBackground.Visibility = Visibility.Visible;
				_meBackground.MediaEnded += _meBackground_MediaEnded;
				_meBackground.Opacity = DisplayConfiguration.BackgroundOpacity;
				_meBackground.Play();
			}
			else
			{
				_meBackground.Visibility = Visibility.Hidden;
			}

		}

		private TextBlock newTextBlock()
		{
			// ReSharper disable once UseObjectOrCollectionInitializer
			TextBlock txtBox = new TextBlock();
			txtBox.Name = "txtBox_" + txtBoxCount++;
			txtBox.TextAlignment = TextAlignment.Center;
			txtBox.Width = DisplayConfiguration.FontMonospaceWidth;
			txtBox.FontSize = DisplayConfiguration.FontSize;
			txtBox.Foreground = DisplayConfiguration.FontColorBrush;
			txtBox.FontFamily = new FontFamily(DisplayConfiguration.FontFamily);
			txtBox.Background = Brushes.Transparent;
			//x txtBox.Effect = new DropShadowEffect {BlurRadius = 25, Color = Colors.Aquamarine, Opacity = .75, ShadowDepth = 5};
			return txtBox;
		}

		private Image newImage()
		{
			Image newImg = new Image();
			newImg.Name = "imgBox_" + txtBoxCount++;
			newImg.HorizontalAlignment = HorizontalAlignment.Center;
			newImg.Width = DisplayConfiguration.FontMonospaceWidth;
			//x newImg.Effect = new DropShadowEffect {BlurRadius = 25, Color = Colors.Aquamarine, Opacity = .75, ShadowDepth = 5};
			return newImg;
		}

		private void addTextBlock()
		{
			if (DisplayConfiguration.IsUsingNumberGraphics)
			{
				Image newImg = newImage();
				counterPanel.Children.Add(newImg);
				return;
			}
			TextBlock newBlock = newTextBlock();
			counterPanel.Children.Add(newBlock);
			//x counterPanel.RegisterName(txtBox.Name, txtBox);
		}
		
		private void removeTextBlock()
		{
			txtBoxCount--;
			//x counterPanel.Children.Remove((TextBlock)counterPanel.FindName("txtBox_" + txtBoxCount));
			counterPanel.Children.RemoveAt(counterPanel.Children.Count - 1);
		}

		private void _meBackground_MediaEnded(object sender, RoutedEventArgs e)
		{
			_meBackground.Position = new TimeSpan(0, 0, 0, 0, 1);
			_meBackground.Play();
		}


		private int txtBoxCount;
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					Close();
					break;
				//case Key.Add:
				//	counter.AddToCount(10);
				//	break;
				//case Key.Subtract:
				//	counter.SubtractFromCount(10);
				//	break;
				//case Key.K:
				//	counter.IncrementToCount(1000);
				//	break;
			}
		}

		public void UpdateText(string text)
		{
			//x formValue.StringValue = text;
			if (text.Length > txtBoxCount)
			{
				// need more txt boxes
				int add = text.Length - txtBoxCount;
				Dispatcher.Invoke(() =>
				{
					for (int i = 0; i < add; i++)
					{
						addTextBlock();
					}
				});
			}
			else if (text.Length < txtBoxCount)
			{
				// too many text boxes
				int subtract = txtBoxCount - text.Length;
				Dispatcher.Invoke(() =>
				{
					for (int i = 0; i < subtract; i++)
					{
						removeTextBlock();
					}
				});
			}

			Dispatcher.Invoke(() =>
			{


				if (DisplayConfiguration.IsUsingNumberGraphics)
				{
					UpdateNumberGraphics(text);
				}
				else
				{
					UpdateTextCounter(text);
				}
			});

		}

		private void UpdateTextCounter(string text)
		{
			int i = 0;
			foreach (TextBlock textBox in FindVisualChildren<TextBlock>(this.counterPanel))
			{
				if (textBox.Text != text[i].ToString())
				{
					if (DisplayConfiguration.UseTransitions)
					{
						if (!animRunningList.Contains("anim_" + i))
						{
							ScaleTransform trans = new ScaleTransform();
							textBox.RenderTransform = trans;
							textBox.RenderTransformOrigin = new Point(0.5, 0.5);

							DoubleAnimation anim = new DoubleAnimation();
							anim.Name = "anim_" + i;
							anim.To = -1;
							anim.Duration = TimeSpan.FromMilliseconds(500);
							anim.AutoReverse = true;
							anim.Completed += anim_Completed;

							trans.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
							animRunningList.Add("anim_" + i);
						}
					}
					textBox.Text = text[i].ToString();
				}
				i++;
			}
		}

		private void UpdateNumberGraphics(string text)
		{
			int i = 0;
			foreach (Image image in FindVisualChildren<Image>(this.counterPanel))
			{
				if ((image.Tag ?? "").ToString() != text[i].ToString())
				{
					int textVal;
					int.TryParse(text[i].ToString(), out textVal);
					image.Source = cachedBitmaps[textVal];
					image.Tag = text[i].ToString();
				}
				i++;
			}
		}

		void anim_Completed(object sender, EventArgs e)
		{
			AnimationClock animationClock = (AnimationClock) sender;
			animRunningList.Remove(animationClock.Timeline.Name);
		}

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null)
			{
				yield break;
			}
			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);
				if (child is T)
				{
					yield return (T)child;
				}

				foreach (var childOfChild in FindVisualChildren<T>(child))
				{
					yield return childOfChild;
				}
			}
		}

	}
}
