using System;
using System.Drawing;
using System.Xml.Serialization;

namespace Clf.Common.ImageProcessing
{
	[XmlRoot(ElementName = "color")]
	public class Colour
	{
    private const string LED_GREEN_ON = "00DE0A";
    public const string LED_GREEN_ON_HEX = $"#{LED_GREEN_ON}";
    private const string LED_GREEN_OFF = "005A02";
    public const string LED_GREEN_OFF_HEX = $"0x{LED_GREEN_OFF}";

    private const string LED_ERROR_ON = "FF0000";
    public const string LED_ERROR_ON_HEX = $"#{LED_ERROR_ON}";
    private const string LED_ERROR_OFF = "8C0600";
    public const string LED_ERROR_OFF_HEX = $"#{LED_ERROR_OFF}";

    private const string LED_WARN_ON = "FFD600";
    public const string LED_WARN_ON_HEX = $"#{LED_WARN_ON}";
    private const string LED_WARN_OFF = "AA6100";
    public const string LED_WARN_OFF_HEX = $"#{LED_WARN_OFF}";

    private const string ENUM_OPTION_1 = "66ccff";
		private const string ENUM_OPTION_2 = "0080ff";
    private const string ENUM_OPTION_3 = "9999ff";
    private const string ENUM_OPTION_4 = "00ccff";
    private const string ENUM_OPTION_5 = "3399ff";
    private const string ENUM_OPTION_6 = "3366ff";
    private const string ENUM_OPTION_7 = "0099cc";
    private const string ENUM_OPTION_8 = "333399";

    public const string ENUM_OPTION_1_HEX = $"#{ENUM_OPTION_1}";
    public const string ENUM_OPTION_2_HEX = $"#{ENUM_OPTION_2}";
    public const string ENUM_OPTION_3_HEX = $"#{ENUM_OPTION_3}";
    public const string ENUM_OPTION_4_HEX = $"#{ENUM_OPTION_4}";
    public const string ENUM_OPTION_5_HEX = $"#{ENUM_OPTION_5}";
    public const string ENUM_OPTION_6_HEX = $"#{ENUM_OPTION_6}";
    public const string ENUM_OPTION_7_HEX = $"#{ENUM_OPTION_7}";
    public const string ENUM_OPTION_8_HEX = $"#{ENUM_OPTION_8}";
    
    private const string DONT_CARE = "0094FF";
    public const string DONT_CARE_HEX = $"#{DONT_CARE}";
	 
		private const string NOT_CONN = "FA00FA";
    public const string NOT_CONN_HEX = $"#{NOT_CONN}";

    private const string INVALID = "D0D5DD";
    public const string INVALID_HEX = $"#{INVALID}";

    [XmlAttribute(AttributeName = "name")]
		public string? Name { get; set; }

		[XmlAttribute(AttributeName = "red")]
		public int Red { get; set; }

		[XmlAttribute(AttributeName = "green")]
		public int Green { get; set; }

		[XmlAttribute(AttributeName = "blue")]
		public int Blue { get; set; }

		[XmlAttribute(AttributeName = "alpha")]
		public int Alpha { get; set; } = 255;

		[XmlIgnore]
		public double NormalizedAlpha => (Alpha > 255 ? 255 : Alpha) / 255.0; // Normalized Alpha between 0.0-1.0 for html rgba

		[XmlIgnore]
		public string HtmlRgbaString => $"rgba({Red},{Green},{Blue},{NormalizedAlpha})";

		[XmlIgnore]
		public static Colour Transparent => new Colour() { Red = 0, Green = 0, Blue = 0, Alpha = 0 };

		public string HTMLColor => String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", Red, Green, Blue, Alpha);

		public string HexColour => String.Format("0x{0:X2}{1:X2}{2:X2}{3:X2}", Red, Green, Blue, Alpha);

		public static Colour SystemDrawingColorToColour(System.Drawing.Color color)
		{
			return new Colour() { Alpha = color.A, Red = color.R, Green = color.G, Blue = color.B, Name = color.Name };
		}

		public static Colour HexStringToColour(string hex)
		{
			if (hex.StartsWith("#"))
			{
				return new Colour()
				{
					Red = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber),
					Green = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber),
					Blue = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber),
					Alpha = hex.Length == 9 ? byte.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber) : byte.Parse("FF", System.Globalization.NumberStyles.HexNumber)
				};
			}
			else
				return new Colour();
		}
	}
}
