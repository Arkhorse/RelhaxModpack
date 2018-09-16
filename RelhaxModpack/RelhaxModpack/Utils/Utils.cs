﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RelhaxModpack
{
    /// <summary>
    /// A utility class for static functions used in various places in the modpack
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// A generic Progress Indicator window to be used for when loading things on a UI thread
        /// Use this in a using statement
        /// </summary>
        public static Windows.ProgressIndicator ProgressIndicator;
        /// <summary>
        /// Return the entire assembely version
        /// </summary>
        /// <returns>The entire assembely version string (major, minor, build, revision)</returns>
        public static string GetApplicationVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        /// <summary>
        /// Return the date and time in EN-US form, the time that the application was built
        /// </summary>
        /// <returns>the application build date and time in EN-US form</returns>
        public static string GetCompileTime()
        {
            return CiInfo.BuildTag + " (EN-US date format)";
        }

        public static List<Visual> GetAllWindowComponentsVisual(Window window, bool includeWindow)
        {
            //https://stackoverflow.com/questions/874380/wpf-how-do-i-loop-through-the-all-controls-in-a-window
            List<Visual> windowComponents = new List<Visual>();
            if (includeWindow)
                windowComponents.Add(window);
            if (VisualTreeHelper.GetChildrenCount(window) > 0)
                GetAllWindowComponentsVisual(window, windowComponents);
            return windowComponents;
        }

        private static void GetAllWindowComponentsLogical(Visual v, List<Visual> allWindowComponents)
        {
            //NOTE: v has been added
            //have to use var here cause i got NO CLUE what type it is #niceMeme
            var children = LogicalTreeHelper.GetChildren(v);
            //Type temp = children.GetType();
            foreach(var child in children)
            {
                //Type temp2 = child.GetType();
                if(child is Visual childVisual)
                {
                    allWindowComponents.Add(childVisual);
                    GetAllWindowComponentsLogical(childVisual, allWindowComponents);
                }
            }
        }

        private static void GetAllWindowComponentsVisual(Visual v, List<Visual> allWindowComponents)
        {
            int ChildrenComponents = VisualTreeHelper.GetChildrenCount(v);
            for (int i = 0; i < ChildrenComponents; i++)
            {
                Visual subV = (Visual)VisualTreeHelper.GetChild(v, i);
                allWindowComponents.Add(subV);
                if (subV is TabControl tabControl)
                {
                    foreach(Visual tabVisual in tabControl.Items)
                    {
                        allWindowComponents.Add(tabVisual);
                        GetAllWindowComponentsLogical(tabVisual, allWindowComponents);
                    }
                }
                int childrenCount = VisualTreeHelper.GetChildrenCount(subV);
                if (childrenCount > 0)
                    GetAllWindowComponentsVisual(subV, allWindowComponents);
            }
        }
    }
}
