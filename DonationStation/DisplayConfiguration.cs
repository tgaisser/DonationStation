using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Signpost
{
	public static class DisplayConfiguration
	{
		private static bool isWindowWidthRead;
		private static int windowWidth;
		public static int WindowWidth
		{
			get
			{
				if (isWindowWidthRead) return windowWidth;

				int.TryParse(GetSettingValue("WindowWidth", "1920"), out windowWidth);
				isWindowWidthRead = true;
				return windowWidth;
			}
		}

		private static bool isWindowHeightRead;
		private static int windowHeight;
		public static int WindowHeight
		{
			get
			{
				if (isWindowHeightRead) return windowHeight;

				int.TryParse(GetSettingValue("WindowHeight", "1080"), out windowHeight);
				isWindowHeightRead = true;
				return windowHeight;
			}
		}

		private static bool isBackgroundVideoRead;
		private static string backgroundVideo;
		public static string BackgroundVideo {
			get
			{
				if (isBackgroundVideoRead) return backgroundVideo;


				backgroundVideo = GetSettingValue("BackgroundVideo");
				isBackgroundVideoRead = true;
				return backgroundVideo;
			}
		}

		private static bool isBackgroundImageRead;
		private static string backgroundImage;
		public static string BackgroundImage
		{
			get
			{
				if (isBackgroundImageRead) return backgroundImage;

				backgroundImage = GetSettingValue("BackgroundImage");
				isBackgroundImageRead = true;
				return backgroundImage;
			}
		}

		private static bool isBackgroundColorRead;
		private static string backgroundColorValue;
		public static string BackgroundColorValue
		{
			get
			{
				if (isBackgroundColorRead) return backgroundColorValue;

				backgroundColorValue = GetSettingValue("BackgroundColor");
				isBackgroundColorRead = true;

				if (! string.IsNullOrWhiteSpace(backgroundColorValue))
				{
					var backColor = ColorConverter.ConvertFromString(backgroundColorValue);
					if (backColor != null)
					{
						backgroundColor = (Color) backColor;
					}
				}

				return backgroundColorValue;
			}
		}

		private static Color backgroundColor;
		public static Color BackgroundColor
		{
			get
			{
				if (isBackgroundColorRead) return backgroundColor;
				// ReSharper disable once UnusedVariable
				string bgCol = BackgroundColorValue;
				return backgroundColor;
			}
		}

		private static SolidColorBrush backgroundColorBrush;
		public static SolidColorBrush BackgroundColorBrush
		{
			get
			{
				if (isBackgroundColorRead) return backgroundColorBrush;

				backgroundColorBrush = new SolidColorBrush { Opacity = BackgroundOpacity, Color = BackgroundColor };
				return backgroundColorBrush;
			}
		}

		private static bool isBackgroundOpacityRead;
		private static double backgroundOpacity = 1.0;
		public static double BackgroundOpacity
		{
			get
			{
				if (isBackgroundOpacityRead) return backgroundOpacity;

				double.TryParse(GetSettingValue("BackgroundOpacity", "0.0"), out backgroundOpacity);
				isBackgroundOpacityRead = true;
				return backgroundOpacity;
			}
		}


		private static bool isFontFamilyRead;
		private static string fontFamily;
		public static string FontFamily
		{
			get
			{
				if (isFontFamilyRead) return fontFamily;

				fontFamily = GetSettingValue("FontFamily", "Arial");
				isFontFamilyRead = true;
				return fontFamily;
			}
		}

		private static bool isFontSizeRead;
		private static double fontSize;
		public static double FontSize
		{
			get
			{
				if (isFontSizeRead) return fontSize;

				double.TryParse(GetSettingValue("FontSize", "100"), out fontSize);
				isFontSizeRead = true;
				return fontSize;
			}
		}

		private static bool isFontColorValueRead;
		private static string fontColorValue;
		public static string FontColorValue
		{
			get
			{
				if (isFontColorValueRead) return fontColorValue;

				fontColorValue = GetSettingValue("FontColor", "Green");
				isFontColorValueRead = true;

				if (!string.IsNullOrWhiteSpace(fontColorValue))
				{
					var fntColor = ColorConverter.ConvertFromString(fontColorValue);
					if (fntColor != null)
					{
						fontColor = (Color)fntColor;
					}
				}

				return fontColorValue;
			}
		}

		private static Color fontColor;
		public static Color FontColor
		{
			get
			{
				if (isFontColorValueRead) return fontColor;
				// ReSharper disable once UnusedVariable
				string fntColor = FontColorValue;
				return fontColor;
			}
		}

		private static SolidColorBrush fontColorBrush;
		public static SolidColorBrush FontColorBrush
		{
			get
			{
				if (isFontColorValueRead) return fontColorBrush;

				fontColorBrush = new SolidColorBrush { Opacity = 1.0, Color = FontColor };
				return fontColorBrush;
			}
		}


		private static bool isUseTransitionsRead;
		private static bool useTransitions;
		public static bool UseTransitions
		{
			get
			{
				if (isUseTransitionsRead) return useTransitions;

				useTransitions = GetSettingValue("UseTransitions", "FALSE").ToUpper() == "TRUE";
				isUseTransitionsRead = true;
				return useTransitions;
			}
		}

		private static bool isIncrementByRead;
		private static int incrementBy;
		public static int IncrementBy
		{
			get
			{
				if (isIncrementByRead) return incrementBy;

				int.TryParse(GetSettingValue("IncrementBy", "0"), out incrementBy);
				isIncrementByRead = true;
				return incrementBy;
			}
		}

		private static bool isMillisecondsBetweenIncrement;
		private static int millisecondsBetweenIncrement;
		public static int MillisecondsBetweenIncrement
		{
			get
			{
				if (isMillisecondsBetweenIncrement) return millisecondsBetweenIncrement;

				int.TryParse(GetSettingValue("MillisecondsBetweenIncrement", "0"), out millisecondsBetweenIncrement);
				isMillisecondsBetweenIncrement = true;
				return millisecondsBetweenIncrement;
			}
		}

		private static bool isLeadingCharacterRead;
		private static string leadingCharacter;
		public static string LeadingCharacter
		{
			get
			{
				if (isLeadingCharacterRead) return leadingCharacter;

				leadingCharacter = GetSettingValue("LeadingCharacter");
				isLeadingCharacterRead = true;
				return leadingCharacter;
			}
		}

		private static bool isStartingValueRead;
		private static string startingValue;
		public static string StartingValue
		{
			get
			{
				if (isStartingValueRead) return startingValue;

				startingValue = GetSettingValue("StartingValue");
				isStartingValueRead = true;
				return startingValue;
			}
		}

		public static int NumericStartingValue
		{
			get
			{
				if (string.IsNullOrWhiteSpace(StartingValue)) return 0;

				int retValue;
				int.TryParse(StartingValue, out retValue);
				return retValue;
			}
		}

		private static bool isMainTextTopRead;
		private static Thickness mainTextTop;
		public static Thickness MainTextTop
		{
			get
			{
				if (isMainTextTopRead) return mainTextTop;

				string top = GetSettingValue("MainTextTop", "0");
				var o = (new ThicknessConverter().ConvertFromString("0," + top + ",0,0"));
				if (o != null)
					mainTextTop = (Thickness)o;

				isMainTextTopRead = true;
				return mainTextTop;
			}
		}

		private static bool isFontMonospaceWidthRead;
		private static int fontMonospaceWidth;
		public static int FontMonospaceWidth
		{
			get
			{
				if (isFontMonospaceWidthRead) return fontMonospaceWidth;

				int.TryParse(GetSettingValue("FontMonospaceWidth", "0"), out fontMonospaceWidth);
				isFontMonospaceWidthRead = true;
				return fontMonospaceWidth;
			}
		}

		private static bool isNumberGraphicsRead;
		private static readonly string[] numberGraphics = new string[10];
		public static string[] NumberGraphics
		{
			get
			{
				if (isNumberGraphicsRead) return numberGraphics;
				for (int i = 0; i < 10; i++)
				{
					numberGraphics[i] = GetSettingValue(String.Format("Graphic_{0}", i));
				}
				isNumberGraphicsRead = true;
				return numberGraphics;
			}
		}

		public static bool NumberGraphicExists(int numberToCheck)
		{
			return ! string.IsNullOrWhiteSpace(NumberGraphics[numberToCheck]);
		}

		private static bool isUsingGraphicsDetermined;
		private static bool isUsingNumberGraphics = true;
		public static bool IsUsingNumberGraphics
		{
			get
			{
				if (isUsingGraphicsDetermined) return isUsingNumberGraphics;

				foreach (string numberGraphic in NumberGraphics)
				{
					isUsingNumberGraphics = ! string.IsNullOrWhiteSpace(numberGraphic);
					if (! isUsingNumberGraphics) break;
				}
				return isUsingNumberGraphics;
			}
		}

		public static string GetSettingValue(string keyName, string defaultValue = "")
		{
			string returnValue = defaultValue;
			if (ConfigurationManager.AppSettings[keyName] != null)
			{
				returnValue = Convert.ToString(ConfigurationManager.AppSettings[keyName]);
			}
			return returnValue.Trim();
		}
}
}
