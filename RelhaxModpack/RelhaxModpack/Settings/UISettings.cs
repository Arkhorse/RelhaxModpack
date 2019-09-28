using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Xml;
using System.IO;
using System.Windows.Controls;
using RelhaxModpack.Windows;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace RelhaxModpack
{
    public struct CustomBrushSetting
    {
        public Brush @Brush;
        public string SettingName;
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
        public CustomBrushSetting(string settingName, Brush brush)
        { Brush = brush; SettingName = settingName; }
    };
    /// <summary>
    /// Handles all custom UI settings
    /// </summary>
    public static class UISettings
    {
        #region statics and constants
        /// <summary>
        /// The name of the Xml element to hold all custom color settings
        /// </summary>
        /// <remarks>See CustomSettings array for list of custom colors</remarks>
        public const string CustomColorSettingsPathV1 = "CustomColorSettings";

        /// <summary>
        /// The parsed XML document containing the xml color settings
        /// </summary>
        public static XmlDocument UIDocument;

#warning the color stuff needs to be finished
        /// <summary>
        /// The color to use for when a component is selected in the selection list
        /// </summary>
        public static CustomBrushSetting SelectedPanelColor = new CustomBrushSetting(nameof(SelectedPanelColor), new SolidColorBrush(Colors.BlanchedAlmond));

        /// <summary>
        /// The color to use for when a component is not selection in the selection list
        /// </summary>
        public static CustomBrushSetting NotSelectedPanelColor = new CustomBrushSetting(nameof(NotSelectedPanelColor), new SolidColorBrush(Colors.BlanchedAlmond));

        /// <summary>
        /// The color to use when a component is selected in the selection list
        /// </summary>
        public static CustomBrushSetting SelectedTextColor = new CustomBrushSetting(nameof(SelectedTextColor), new SolidColorBrush(Colors.BlanchedAlmond));

        /// <summary>
        /// The color to use when a component is not selected in the selection list
        /// </summary>
        public static CustomBrushSetting NotSelectedTextColor = new CustomBrushSetting(nameof(NotSelectedTextColor), new SolidColorBrush(Colors.BlanchedAlmond));

        /// <summary>
        /// A list of custom colors for controlling color behavior of components that 
        /// </summary>
        /// <remarks>Settings that exist in here don't directly map to 1 setting and control other color settings.
        /// For example, changing the color of a selected component in the mod selection list</remarks>
        public static readonly CustomBrushSetting[] CustomColorSettings = new CustomBrushSetting[]
        {
            SelectedPanelColor,
            NotSelectedPanelColor,
            SelectedTextColor,
            NotSelectedTextColor
        };

        /// <summary>
        /// The color to use in the selection list for a tab which is not selected
        /// </summary>
        /// <remarks>It starts as null because the color is unknown (and can be different types based on the user's theme).
        /// It is set on user selection on a component in the selection list.</remarks>
        public static Brush NotSelectedTabColor = null;

        private static string parsedFormatVersion = string.Empty;
        #endregion

        /// <summary>
        /// Load the custom color definitions from XML
        /// </summary>
        public static void LoadSettings()
        {
            //first check if the file exists
            if(!File.Exists(Settings.UISettingsFileName))
            {
                Logging.Info("UIDocument file does not exist, using defaults");
                return;
            }

            //try to create a new one first in a temp. If it fails then abort.
            XmlDocument loadedDoc = XmlUtils.LoadXmlDocument(Settings.UISettingsFileName, XmlLoadType.FromFile);
            if(loadedDoc == null)
            {
                Logging.Error("failed to parse UIDocument, check messages above for parsing errors");
                return;
            }
            UIDocument = loadedDoc;
            Logging.Info("UIDocument xml file loaded successfully, loading custom color instances");

            SetDocumentVersion();
            if (string.IsNullOrWhiteSpace(parsedFormatVersion))
            {
                Logging.Error("UIDocument formatVersion string is null, aborting parsing");
                return;
            }
            switch (parsedFormatVersion)
            {
                case "1.0":
                    Logging.Info("parsing custom color instances file using V1 parse method");
                    ApplyCustomColorSettingsV1();
                    break;
                default:
                    //unknown
                    Logging.Error("Unknown format string or not supported: {0}", parsedFormatVersion);
                    return;
            }
            Logging.Info("Custom color instances loaded");
        }

        private static void ApplyCustomColorSettingsV1()
        {
            for(int i = 0; i < CustomColorSettings.Count(); i++)
            {
                CustomBrushSetting customBrush = CustomColorSettings[i];
                string instanceName = customBrush.SettingName;
                string customColorSettingXpath = string.Format("//{0}/{1}", Settings.UISettingsColorFile, instanceName);
                Logging.Debug("using xpath {0} to set color of custom property {1}", customColorSettingXpath, instanceName);
                XmlNode customColorNode = XmlUtils.GetXmlNodeFromXPath(UIDocument, customColorSettingXpath);
                if(customColorNode == null)
                {
                    Logging.Info("custom color instance {0} not defined, skipping", instanceName);
                    continue;
                }
                if(ApplyBrushSettings(instanceName,instanceName,customColorNode, out Brush customBrushOut))
                {
                    customBrush.Brush = customBrushOut;
                }
                else
                {
                    Logging.Warning("failed to apply color settings for custom instance {0}", instanceName);
                    continue;
                }
            }
        }

        private static void SetDocumentVersion()
        {
            //get the UI xml format version of the file
            string versionXpath = "//" + Settings.UISettingsColorFile + "/@version";
            parsedFormatVersion = XmlUtils.GetXmlStringFromXPath(UIDocument, versionXpath);
            Logging.Debug("using xpath search '{0}' found format version '{1}'", versionXpath, parsedFormatVersion.Trim());
            //trim it
            parsedFormatVersion = parsedFormatVersion.Trim();
        }

        #region Apply to window
        /// <summary>
        /// Applies custom color settings to a window
        /// </summary>
        /// <param name="window">The Window object to apply color settings to</param>
        public static void ApplyUIColorSettings(Window window)
        {
            if(UIDocument == null)
            {
                Logging.Info("UIDocument is null, no custom color settings to apply");
                return;
            }

            SetDocumentVersion();
            if (string.IsNullOrWhiteSpace(parsedFormatVersion))
            {
                Logging.Error("UIDocument formatVersion string is null, aborting parsing");
                return;
            }

            switch (parsedFormatVersion)
            {
                case "1.0":
                    Logging.Info("parsing color settings file using V1 parse method");
                    ApplyUIColorsettingsV1(window);
                    break;
                default:
                    //unknown
                    Logging.Error("Unknown format string or not supported: {0}", parsedFormatVersion);
                    return;
            }
        }
        
        /// <summary>
        /// Applies color settings to a window of Xml document format 1.0
        /// </summary>
        /// <param name="window">The window to apply changes to</param>
        private static void ApplyUIColorsettingsV1(Window window)
        {
            string windowType = window.GetType().Name;
            //using RelhaxWindow type allows us to directly control/check if the window should be color changed
            if (window is RelhaxWindow relhaxWindow && !relhaxWindow.ApplyColorSettings)
            {
                Logging.Warning("window of type '{0}' is set to not have color setting applied, skipping",windowType);
                return;
            }

            //build the xpath string
            //root of the file is the fileName, has an array of elements with each name is the type of the window
            string XpathWindow = string.Format("//{0}/{1}", Settings.UISettingsColorFile,windowType);
            Logging.Debug("using window type xpath string {0}", XpathWindow);
            XmlNode windowColorNode = XmlUtils.GetXmlNodeFromXPath(UIDocument, XpathWindow);

            if(windowColorNode == null)
            {
                Logging.Error("failed to get the window color node using xml search path {0}",XpathWindow);
                return;
            }
            //apply window color settings if exist
            ApplyBrushSettings(window, windowColorNode);

            //build list of all internal framework components
            List<FrameworkElement> allWindowControls = Utils.GetAllWindowComponentsVisual(window, false);
            foreach (FrameworkElement element in allWindowControls)
            {
                //make sure we have an element that we want color changing
                if(element.Tag is string ID && !string.IsNullOrWhiteSpace(ID))
                {
                    //https://msdn.microsoft.com/en-us/library/ms256086(v=vs.110).aspx
                    //get the xpath component
                    string XPathColorSetting = string.Format("//{0}/{1}/ColorSetting[@ID = \"{2}\"]", Settings.UISettingsColorFile, windowType, ID);
                    XmlNode brushSettings = XmlUtils.GetXmlNodeFromXPath(UIDocument, XPathColorSetting);
                    //make sure setting is there
                    if (brushSettings != null)
                        ApplyBrushSettings(element, brushSettings);
                }
            }
        }
        
        private static void ApplyBrushSettings(FrameworkElement element, XmlNode brushSettings)
        {
            if (element is Control control)
            {
                if(ApplyBrushSettings(control.Name, (string)control.Tag, brushSettings, out Brush backgroundColorToChange))
                {
                    control.Background = backgroundColorToChange;
                    if (!(element is Window))
                    {
                        if(ApplyTextBrushSettings(control.Name, (string)control.Tag, brushSettings, out Brush textColorToChange))
                            control.Foreground = textColorToChange;
                    }
                }
            }
            else if (element is Panel panel)
            {
                if(ApplyBrushSettings(panel.Name, (string)panel.Tag, brushSettings, out Brush backgroundColorToChange))
                {
                    panel.Background = backgroundColorToChange;
                }
            }  
        }

        private static bool ApplyTextBrushSettings(string componentName, string componentTag, XmlNode brushSettings, out Brush textColorToChange)
        {
            bool somethingApplied = false;
            textColorToChange = new SolidColorBrush();
            //make sure type is set correctly
            XmlAttribute brushType = brushSettings.Attributes["type"];
            if (brushType == null)
            {
                Logging.Warning("failed to apply brush setting: type attribute not exist!");
                return false;
            }
            XmlAttribute textColor = brushSettings.Attributes["textColor"];
            //try to apply the text color
            if (textColor != null)
            {
                if(ParseColorFromString(textColor.InnerText, out Color color))
                {
                    textColorToChange = new SolidColorBrush(color);
                    somethingApplied = true;
                }
                else
                {
                    Logging.WriteToLog(string.Format("failed to parse color to {0}, type={1}, color1={2}, ",
                        componentTag, brushType.InnerText, textColor.InnerText), Logfiles.Application, LogLevel.Warning);
                }
            }
            else
            {
                Logging.WriteToLog(string.Format("skipping text coloring of control {0}: textColor is null", componentName),
                    Logfiles.Application, LogLevel.Warning);
            }
            return somethingApplied;
        }

        private static bool ApplyBrushSettings(string componentName, string componentTag, XmlNode brushSettings, out Brush backgroundColorToChange)
        {
            bool someThingApplied = false;
            backgroundColorToChange = new SolidColorBrush();
            //make sure type is set correctly
            XmlAttribute brushType = brushSettings.Attributes["type"];
            if(brushType == null)
            {
                Logging.Warning("failed to apply brush setting: type attribute not exist!");
                return false;
            }
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.color.fromargb?view=netframework-4.7.2#System_Windows_Media_Color_FromArgb_System_Byte_System_Byte_System_Byte_System_Byte_
            XmlAttribute color1 = brushSettings.Attributes["color1"];
            XmlAttribute color2 = brushSettings.Attributes["color2"];
            XmlAttribute point1 = brushSettings.Attributes["point1"];
            XmlAttribute point2 = brushSettings.Attributes["point2"];
            if(color1 != null)
            {
                Point point_1;
                Point point_2;
                switch (brushType.InnerText)//TODO
                {
                    case "SolidColorBrush"://color=1
                        if (ParseColorFromString(color1.InnerText, out Color kolor1_solid))
                        {
                            backgroundColorToChange = new SolidColorBrush(kolor1_solid);
                            someThingApplied = true;
                            break;
                        }
                        else
                        {
                            Logging.WriteToLog(string.Format("failed to parse color to {0}, type={1}, color1={2}, ",
                                componentTag, brushType.InnerText, color1.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                    case "LinearGradientBrush"://color=2, point=2
                        if (color2 == null)
                        {
                            Logging.WriteToLog(string.Format("skipping coloring of control {0}: color2 is null, type={1}",
                                componentTag, brushType.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                        if (point1 == null)
                        {
                            Logging.WriteToLog(string.Format("skipping coloring of control {0}: point1 is null, type={1}",
                                componentTag, brushType.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                        if (point2 == null)
                        {
                            Logging.WriteToLog(string.Format("skipping coloring of control {0}: point2 is null, type={1}",
                                componentTag, brushType.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                        if (ParseColorFromString(color1.InnerText, out Color kolor1_linear) &&
                            ParseColorFromString(color2.InnerText, out Color kolor2_linear))
                        {
                            try
                            {
                                //https://docs.microsoft.com/en-us/dotnet/api/system.windows.point.parse?view=netframework-4.7.2
                                point_1 = Point.Parse(point1.InnerText);
                                point_2 = Point.Parse(point2.InnerText);
                            }
                            catch
                            {
                                Logging.WriteToLog(string.Format("failed to parse points, point1={0}, point2={1}",
                                    point1.InnerText, point2.InnerText), Logfiles.Application, LogLevel.Warning);
                                break;
                            }
                            VerifyPoints(point_1);
                            VerifyPoints(point_2);
                            backgroundColorToChange = new LinearGradientBrush(kolor1_linear, kolor2_linear, point_1, point_2);
                            someThingApplied = true;
                            break;
                        }
                        else
                        {
                            Logging.WriteToLog(string.Format("failed to parse color to {0}, type={1}, color1={2}, color2={3}",
                                componentTag, brushType.InnerText, color1.InnerText, color2.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                    case "RadialGradientBrush"://color=2
                        if (color2 == null)
                        {
                            Logging.WriteToLog(string.Format("skipping coloring of control {0}: color2 is null, type={1}",
                                componentTag, brushType.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                        if (ParseColorFromString(color1.InnerText, out Color kolor1_radial)
                            && ParseColorFromString(color2.InnerText, out Color kolor2_radial))
                        {
                            backgroundColorToChange = new RadialGradientBrush(kolor1_radial, kolor2_radial);
                            someThingApplied = true;
                            break;
                        }
                        else
                        {
                            Logging.WriteToLog(string.Format("failed to apply color to {0}, type={1}, color1={2}, color2={3}",
                                componentTag, brushType.InnerText, color1.InnerText, color2.InnerText), Logfiles.Application, LogLevel.Warning);
                            break;
                        }
                    default:
                        Logging.Warning(string.Format("unknown type parameter{0} in component {1} ", brushType.InnerText, componentTag));
                        break;
                }
            }
            else
            {
                Logging.WriteToLog(string.Format("skipping coloring of control {0}: color1 is null, type={1}",
                    componentName, brushType.InnerText), Logfiles.Application, LogLevel.Warning);
            }
            return someThingApplied;
        }
        
        /// <summary>
        /// Tries to parse a hex code color to a color object
        /// </summary>
        /// <param name="color">The string hex code for the color to use</param>
        /// <param name="outColor">The corresponding color object</param>
        /// <returns>True if color parsing was successful, a default color otherwise</returns>
        /// <remarks>Uses the 32bit color codes for generation (Alpha, Red, Green, Blue) Alpha is transparency</remarks>
        public static bool ParseColorFromString(string color, out Color outColor)
        {
            outColor = new Color();
            string aPart = string.Empty;
            string rPart = string.Empty;
            string gPart = string.Empty;
            string bPart = string.Empty;
            try
            {
                aPart = color.Substring(1,2);
                rPart = color.Substring(3,2);
                gPart = color.Substring(5,2);
                bPart = color.Substring(7,2);
            }
            catch(ArgumentException)
            {
              Logging.WriteToLog(string.Format("failed to parse color, a={0}, r={1}, g={2}, b={3}",aPart, rPart, gPart, bPart)
                  ,Logfiles.Application,LogLevel.Warning);
              return false;
            }
            if((byte.TryParse(aPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte a)) &&
                (byte.TryParse(rPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r))&&
                (byte.TryParse(gPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g))&&
                (byte.TryParse(bPart, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b)))
            {
                outColor = Color.FromArgb(a, r, g, b);
                return true;
            }
            else
                Logging.WriteToLog(string.Format("failed to parse color, a={0}, r={1}, g={2}, b={3}",aPart, rPart, gPart, bPart)
                    ,Logfiles.Application,LogLevel.Warning);
            return false;
        }
        
        /// <summary>
        /// Verifies that the points for applying color gradient directions are within 0-1
        /// </summary>
        /// <param name="p">The color gradient direction to verify</param>
        public static void VerifyPoints(Point p)
        {
            if(p.X > 1 || p.X < 0)
            {
                int settingToUse = p.X > 1 ? 1 : 0;
                Logging.Warning("point.X is out of bounds (must be between 0 and 1, current value={0}), setting to {1})", p.X,settingToUse);
                p.X = settingToUse;
            }
            if(p.Y > 1 || p.Y < 0)
            {
                int settingToUse = p.Y > 1 ? 1 : 0;
                Logging.Warning("point.Y is out of bounds (must be between 0 and 1, current value={0}), setting to {1})", p.Y, settingToUse);
                p.Y = settingToUse;
            }
        }
        #endregion

        #region Dump to file
        /// <summary>
        /// Saves all currently enabled color settings to an xml file
        /// </summary>
        /// <param name="savePath">The path to save the xml file to</param>
        public static void DumpAllWindowColorSettingsToFile(string savePath)
        {
            //make xml document and declaration
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

            //append declaration to document
            doc.AppendChild(dec);

            //make root element and version attribute
            XmlElement root = doc.CreateElement(Settings.UISettingsColorFile);

            //NOTE: version attribute should be incremented when large change in color loading structure is done
            //allows us to make whole new method to load UI settings
            XmlAttribute version = doc.CreateAttribute("version");

            //append to document
            version.Value = "1.0";
            root.Attributes.Append(version);
            doc.AppendChild(root);

            //add all window instances to document:
            //make windows for all appropriate windows
            DumpWindowColorSettingsToXml(root, doc, new MainWindow());

            //save custom color settings to document
            //for now, just use single solid color for these settings
            foreach(CustomBrushSetting customBrush in CustomColorSettings)
            {
                string name = customBrush.SettingName;
                Logging.Debug("saving custom color SolidColorBrush element {0}", name);
                XmlElement element = doc.CreateElement(name);
                XmlAttribute color = doc.CreateAttribute("color1");
                //color1.Value = solidColorBrush.Color.ToString(CultureInfo.InvariantCulture);
                color.Value = (customBrush.Brush as SolidColorBrush).Color.ToString(CultureInfo.InvariantCulture);
                element.Attributes.Append(color);
                root.AppendChild(element);
            }

            //save xml file
            doc.Save(savePath);
        }

        private static void DumpWindowColorSettingsToXml(XmlElement root, XmlDocument doc, Window window)
        {
            //make window element
            XmlElement windowElement = doc.CreateElement(window.GetType().Name);

            //save attributes to element
            ApplyColorattributesToElement(windowElement, window.Background, doc);

            //same to root
            root.AppendChild(windowElement);

            //get list of all framework elements in the window
            //TODO: this may not work due to visual not being shown
            //TODO: need to disable translations to save CPU TIME
            List<FrameworkElement> AllUIElements = Utils.GetAllWindowComponentsLogical(window, false);
            for(int i = 0; i < AllUIElements.Count; )
            {
                if (AllUIElements[i].Tag == null)
                {
                    AllUIElements.RemoveAt(i);
                    continue;
                }
                if (!(AllUIElements[i].Tag is string s))
                {
                    AllUIElements.RemoveAt(i);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(s))
                {
                    AllUIElements.RemoveAt(i);
                    continue;
                }
                i++;
            }

            //make xml entries for each UI element now
            foreach(FrameworkElement element in AllUIElements)
            {
                XmlElement colorSetting = doc.CreateElement("ColorSetting");
                string ID = (string)element.Tag;
                //save attributes to element
                XmlAttribute elementID = doc.CreateAttribute("ID");
                elementID.Value = ID;
                colorSetting.Attributes.Append(elementID);
                if (element is Panel panel)
                    ApplyColorattributesToElement(colorSetting, panel.Background, doc);
                else if (element is Control control)
                    ApplyColorattributesToElement(colorSetting, control.Background, doc, control.Foreground);
                else
                    continue;
                windowElement.AppendChild(colorSetting);
            }

            window.Close();
        }

        private static void ApplyColorattributesToElement(XmlElement colorEntry, Brush brush, XmlDocument doc, Brush textBrush = null)
        {
            XmlAttribute colorType, color1, textColor;

            if(brush is SolidColorBrush solidColorBrush)
            {
                //type
                colorType = doc.CreateAttribute("type");
                colorType.Value = nameof(SolidColorBrush);
                colorEntry.Attributes.Append(colorType);

                //color1
                color1 = doc.CreateAttribute("color1");
                color1.Value = solidColorBrush.Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(color1);
            }
            else if (brush is LinearGradientBrush linearGradientBrush)
            {
                //type
                colorType = doc.CreateAttribute("type");
                colorType.Value = nameof(LinearGradientBrush);
                colorEntry.Attributes.Append(colorType);

                //color1
                color1 = doc.CreateAttribute("color1");
                color1.Value = linearGradientBrush.GradientStops[0].Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(color1);

                //color2
                XmlAttribute color2 = doc.CreateAttribute("color2");
                color2.Value = linearGradientBrush.GradientStops[linearGradientBrush.GradientStops.Count-1].Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(color2);

                //point1
                XmlAttribute point1 = doc.CreateAttribute("point1");
                point1.Value = linearGradientBrush.StartPoint.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(point1);

                //point2
                XmlAttribute point2 = doc.CreateAttribute("point2");
                point2.Value = linearGradientBrush.EndPoint.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(point2);
            }
            else if (brush is RadialGradientBrush radialGradientBrush)
            {
                //type
                colorType = doc.CreateAttribute("type");
                colorType.Value = nameof(RadialGradientBrush);
                colorEntry.Attributes.Append(colorType);
                //color1
                color1 = doc.CreateAttribute("color1");
                color1.Value = radialGradientBrush.GradientStops[0].Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(color1);
                //color2
                XmlAttribute color2 = doc.CreateAttribute("color2");
                color2.Value = radialGradientBrush.GradientStops[radialGradientBrush.GradientStops.Count - 1].Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(color2);
            }
            else
            {
                Logging.WriteToLog("Unknown background type: " + brush.GetType().ToString(), Logfiles.Application, LogLevel.Debug);
            }
            if(textBrush != null)
            {
                //text color (forground)
                textColor = doc.CreateAttribute("textColor");

                //should all be solid color brushes...
                SolidColorBrush solidColorTextBrush = (SolidColorBrush)textBrush;
                textColor.Value = solidColorTextBrush.Color.ToString(CultureInfo.InvariantCulture);
                colorEntry.Attributes.Append(textColor);
            }
        }
        #endregion
    }
}