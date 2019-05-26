﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace RelhaxModpack
{
    #region JSON Patcher workaround
    //This class is for saving all the lines in an .xc xvm config file
    //the "best json api" can't handle "$" refrences, so they must be removed
    //prior to patching. This class stores all required information for that purpose.
    //TODO: remove this lol
    public struct StringSave
    {
        //the name of the property to put it back on later
        public string Name { get; set; }
        //the value of the property (the refrence)
        public string Value { get; set; }
    }
    #endregion

    public static class PatchUtils
    {
        #region Main Patch Method
        public static void RunPatch(Patch p)
        {
            //check if file exists
            if (!File.Exists(p.CompletePath))
            {
                Logging.Warning("File {0} not found", p.CompletePath);
                return;
            }

            //if from the editor, enable verbose logging (allows it to get debug log statements)
            bool tempVerboseLoggingSetting = ModpackSettings.VerboseLogging;
            if(p.FromEditor && !ModpackSettings.VerboseLogging)
            {
                Logging.Debug("p.FromEditor=true and ModpackSettings.VerboseLogging=false, setting to true for duration of patch method");
                ModpackSettings.VerboseLogging = true;
            }

            //macro parsing needs to go here
            Logging.Info(p.DumpPatchInfoForLog);

            //actually run the patches based on what type it is
            switch (p.Type.ToLower())
            {
                case "regex":
                case "regx":
                    if (p.Lines == null || p.Lines.Count() == 0)
                    {
                        Logging.Debug("Running regex patch as all lines, line by line");
                        RegxPatch(p, null);
                    }
                    else if (p.Lines.Count() == 1 && p.Lines[0].Trim().Equals("-1"))
                    {
                        Logging.Debug("Running regex patch as whole file");
                        RegxPatch(p, new int[] { -1 });
                    }
                    else
                    {
                        Logging.Debug("Running regex patch as specified lines, line by line");
                        int[] lines = new int[p.Lines.Count()];
                        for (int i = 0; i < p.Lines.Count(); i++)
                        {
                            lines[i] = int.Parse(p.Lines[i].Trim());
                        }
                        RegxPatch(p, lines);
                    }
                    break;
                case "xml":
                    XMLPatch(p);
                    break;
                case "json":
                    JsonPatch(p);
                    break;
                case "xvm":
                    throw new BadMemeException("xvm patches are not supported, please use the json patch method");
            }
            Logging.Debug("patch complete");
            //set the verbose setting back
            Logging.Debug("temp logging setting={0}, ModpackSettings.VerboseLogging={1}, setting logging back to temp");
            ModpackSettings.VerboseLogging = tempVerboseLoggingSetting;
        }
        #endregion

        #region XML
        private static void XMLPatch(Patch p)
        {
            //load the xml document
            XmlDocument doc = XMLUtils.LoadXmlDocument(p.CompletePath,XmlLoadType.FromFile);
            if (doc == null)
            {
                Logging.Error("xml document from xml path is null");
                return;
            }

            //check to see if it has the header info at the top to see if we need to remove it later
            bool hadHeader = false;
            string xmlDec = string.Empty;
            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    hadHeader = true;
                    xmlDec = node.OuterXml;
                    break;
                }
            }

            //determines which version of patching will be done
            switch (p.Mode.ToLower())
            {
                case "add":
                    //check to see if it's already there
                    //make the full node path
                    Logging.Debug("checking if xml element to add already exists, creating full xml path");

                    //the add syntax works by using "/" as the node creation path. the last one is the value to put in
                    //there should therefore be at least 2 split element array
                    string[] replacePathSplit = p.Replace.Split('/');

                    //join the base path with the "would-be" path of the new element to add
                    string fullNodePath = p.Path;
                    for (int i = 0; i < replacePathSplit.Count() - 1; i++)
                    {
                        fullNodePath = fullNodePath + "/" + replacePathSplit[i];
                    }

                    //in each node check if the element exist with the replace innerText
                    Logging.Debug("full path to check if exists created as '{0}'", fullNodePath);
                    XmlNodeList fullPathNodeList = doc.SelectNodes(fullNodePath);
                    if (fullPathNodeList.Count > 0)
                    {
                        foreach (XmlElement fullPathMatch in fullPathNodeList)
                        {
                            //get the last element in the replace syntax as value to compare against
                            string innerTextToMatch = replacePathSplit[replacePathSplit.Count() - 1];

                            //remove any tabs and white-spaces first before testing
                            innerTextToMatch = innerTextToMatch.Trim();

                            if (fullPathMatch.InnerText.Trim().Equals(innerTextToMatch))
                            {
                                Logging.Debug("full path found entry with matching text, aborting (no need to patch)");
                                return;
                            }
                            else
                                Logging.Debug("full path found entry, but text does not match. proceeding with add");
                        }
                    }
                    else
                        Logging.Debug("full path entry not found, proceeding with add");

                    //get to the node where to add the element
                    XmlNode xmlPath = doc.SelectSingleNode(p.Path);
                    if(xmlPath == null)
                    {
                        Logging.Error("patch xmlPath returns null!");
                        return;
                    }

                    //create node(s) to add to the element
                    Logging.Debug("Total inner xml elements to make: {0}", replacePathSplit.Count()-1);
                    List<XmlElement> nodesListToAdd = new List<XmlElement>();
                    for (int i = 0; i < replacePathSplit.Count() - 1; i++)
                    {
                        //make the next element using the array replace name
                        XmlElement addElementToMake = doc.CreateElement(replacePathSplit[i]);
                        //the last one is the text to add
                        if (i == replacePathSplit.Count() - 2)
                        {
                            string textToAddIntoNode = replacePathSplit[replacePathSplit.Count() - 1];
                            textToAddIntoNode = Utils.MacroReplace(textToAddIntoNode, ReplacementTypes.PatchArguements);
                            Logging.Debug("adding text: {0}", textToAddIntoNode);
                            addElementToMake.InnerText = textToAddIntoNode;
                        }
                        //add it to the list
                        nodesListToAdd.Add(addElementToMake);
                    }

                    //add nodes to the element in reverse for hierarchy order
                    for (int i = nodesListToAdd.Count - 1; i > -1; i--)
                    {
                        if (i == 0)
                        {
                            //getting here means this is the highest node
                            //that needs to be modified
                            xmlPath.InsertAfter(nodesListToAdd[i], xmlPath.FirstChild);
                            break;
                        }
                        XmlElement parrent = nodesListToAdd[i - 1];
                        XmlElement child = nodesListToAdd[i];
                        parrent.InsertAfter(child, parrent.FirstChild);
                    }
                    Logging.Debug("xml add complete");
                    break;

                case "edit":
                    //check to see if it's already there
                    Logging.Debug("checking if element exists in all results");

                    XmlNodeList xpathResults = doc.SelectNodes(p.Path);
                    if(xpathResults.Count == 0)
                    {
                        Logging.Error("xpath not found");
                        return;
                    }

                    //keep track if all xpath results equal this result
                    int matches = 0;
                    foreach (XmlElement match in xpathResults)
                    {
                        //matched, but trim and check if it matches the replace value
                        if (match.InnerText.Trim().Equals(p.Replace))
                        {
                            Logging.Debug("found replace match for path search, incrementing match counter");
                            matches++;
                        }
                    }
                    if (matches == xpathResults.Count)
                    {
                        Logging.Info("all {0} path results have values equal to replace, so can skip", matches);
                        return;
                    }
                    else
                        Logging.Info("{0} of {1} path results match, running patch");

                    //find and replace
                    foreach (XmlElement replaceMatch in xpathResults)
                    {
                        if (Regex.IsMatch(replaceMatch.InnerText, p.Search))
                        {
                            Logging.Debug("found match, oldValue={0}, new value={1}", replaceMatch.InnerText, p.Replace);
                            replaceMatch.InnerText = p.Replace;
                        }
                        else
                        {
                            Logging.Warning("Regex never matched for this xpath result: {0}",p.Path);
                        }
                    }
                    Logging.Debug("xml edit complete");
                    break;

                case "remove":
                    //check to see if it's there
                    XmlNodeList xpathMatchesToRemove = doc.SelectNodes(p.Path);
                    foreach (XmlElement match in xpathMatchesToRemove)
                    {
                        if (Regex.IsMatch(match.InnerText.Trim(), p.Search))
                        {
                            match.RemoveAll();
                        }
                        else
                        {
                            Logging.Warning("xpath match found, but regex search not matched");
                        }
                    }

                    //remove empty elements
                    Logging.Debug("Removing any empty xml elements");
                    XDocument doc2 = XMLUtils.DocumentToXDocument(doc);
                    //note that XDocuemnt toString drops declaration
                    //https://stackoverflow.com/questions/1228976/xdocument-tostring-drops-xml-encoding-tag
                    doc2.Descendants().Where(e => string.IsNullOrEmpty(e.Value)).Remove();

                    //update doc with doc2
                    doc = XMLUtils.LoadXmlDocument(doc2.ToString(), XmlLoadType.FromString);
                    Logging.Debug("xml remove complete");
                    break;
            }

            //check to see if we need to remove the header
            bool hasHeader = false;
            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    hasHeader = true;
                    break;
                }
            }
            //if not had header and has header, remove header
            //if had header and has header, no change
            //if not had header and not has header, no change
            //if had header and not has header, add header
            Logging.Debug("hadHeader={0}, hasHeader={1}", hadHeader, hasHeader);
            if (!hadHeader && hasHeader)
            {
                Logging.Debug("removing header");
                foreach (XmlNode node in doc)
                {
                    if (node.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        doc.RemoveChild(node);
                        break;
                    }
                }
            }
            else if (hadHeader && !hasHeader)
            {
                Logging.Debug("adding header");
                if(string.IsNullOrEmpty(xmlDec))
                {
                    throw new BadMemeException("nnnice.");
                }
                string[] splitDec = xmlDec.Split('=');
                string xmlVer = splitDec[1].Substring(1).Split('"')[0];
                string xmlenc = splitDec[2].Substring(1).Split('"')[0];
                string xmlStandAlone = splitDec[3].Substring(1).Split('"')[0];
                XmlDeclaration dec = doc.CreateXmlDeclaration(xmlVer, xmlenc, xmlStandAlone);
                doc.InsertBefore(dec, doc.DocumentElement);
            }

            //save to disk
            Logging.Debug("saving to disk");
            doc.Save(p.CompletePath);
            Logging.Debug("xml patch completed successfully");
        }
        #endregion

        #region REGEX
        //method to patch a standard text or json file
        //fileLocation is relative to res_mods folder
        private static void RegxPatch(Patch p, int[] lines)
        {
            //replace all "fake escape characters" with real escape characters
            p.Search = Utils.MacroReplace(p.Search, ReplacementTypes.TextUnescape);

            //legacy compatibility: if the replace text has "newline", then replace it with "\n" and log the warning
            if(p.Replace.Contains("newline"))
            {
                Logging.Warning("This patch has the \"newline\" replace syntax and should be updated");
                p.Replace = p.Replace.Replace("newline", "\n");
            }

            //load file from disk
            string file = File.ReadAllText(p.CompletePath);

            //parse each line into an index array
            string[] fileParsed = file.Split('\n');
            StringBuilder sb = new StringBuilder();
            try
            {
                if (lines == null || lines.Count() == 0)
                {
                    //search entire file and replace each instance
                    bool everReplaced = false;
                    for (int i = 0; i < fileParsed.Count(); i++)
                    {
                        if (Regex.IsMatch(fileParsed[i], p.Search))
                        {
                            Logging.Debug("line {0} matched ({1})", i + 1, fileParsed[i]);
                            fileParsed[i] = Regex.Replace(fileParsed[i], p.Search, p.Replace);
                            everReplaced = true;
                        }
                        //we split by \n so put it back in by \n
                        sb.Append(fileParsed[i] + "\n");
                    }
                    if (!everReplaced)
                    {
                        Logging.Warning("Regex never matched");
                        return;
                    }
                }
                else if (lines.Count() == 1 && lines[0] == -1)
                {
                    //search entire file and string and make one giant regex replacement
                    //but remove newlines first
                    file = Regex.Replace(file, "\n", "newline");
                    if (Regex.IsMatch(file, p.Search))
                    {
                        file = Regex.Replace(file, p.Search, p.Replace);
                        file = Regex.Replace(file, "newline", "\n");
                        sb.Append(file);
                    }
                    else
                    {
                        Logging.Warning("Regex never matched");
                        return;
                    }
                }
                else
                {
                    bool everReplaced = false;
                    for (int i = 0; i < fileParsed.Count(); i++)
                    {
                        //factor for "off by one" (no line number 0 in the text file)
                        if (lines.Contains(i+1))
                        {
                            if (Regex.IsMatch(fileParsed[i], p.Search))
                            {
                                Logging.Debug("line {0} matched ({1})", i + 1, fileParsed[i]);
                                fileParsed[i] = Regex.Replace(fileParsed[i], p.Search, p.Replace);
                                fileParsed[i] = Regex.Replace(fileParsed[i], "newline", "\n");
                                everReplaced = true;
                            }
                        }
                        sb.Append(fileParsed[i] + "\n");
                    }
                    if (!everReplaced)
                    {
                        Logging.Warning("Regex never matched");
                        return;
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Logging.Error("Invalid regex command");
                Logging.Debug(ex.ToString());
            }

            //save the file back into the string and then the file
            file = sb.ToString().Trim();
            File.WriteAllText(p.CompletePath, file);
            Logging.Debug("regex patch completed successfully");
        }
        #endregion

        #region JSON
        private static void JsonPatch(Patch p)
        {
            //apply and log legacy compatibilities
            //if no search parameter, set it to the regex default "match all" search option
            if(string.IsNullOrEmpty(p.Search))
            {
                Logging.Warning("Patch should have search value specified, please update it!");
                p.Search = @".*";
            }
            //if no mode, treat as edit
            if(string.IsNullOrWhiteSpace(p.Mode))
            {
                Logging.Warning("Patch should have mode value specified, please update it!");
                p.Mode = "edit";
            }
            //arrayEdit is not a valid mode, but may be specified by mistake
            if(p.Mode.Equals("arrayEdit"))
            {
                Logging.Warning("Patch mode \"arrayEdit\" is not a valid mode and will be treated as \"edit\", please update it!");
                p.Mode = "edit";
            }

            //load the file into a string
            string file = File.ReadAllText(p.CompletePath);

            //if the file is xc then check it for xvm style references (clean it up for the json parser)
            if (Path.GetExtension(p.CompletePath).ToLower().Equals(".xc"))
            {
                //and also check if the replace value is an xvm direct reference, we don't allow those (needs to be escaped)
                if (Regex.IsMatch(p.Replace, @"\$[ \t]*\{[ \t]*"""))
                {
                    Logging.Error("patch replace value detected as xvm reference, but is not in escaped form! must be escaped!");
                    return;
                }

                //replace all xvm references with escaped versions that can be parsed
                file = EscapeXvmRefrences(file);
            }

            //escape the "$ref" meta-data header 
            file = file.Replace("\"$ref\"", "\"[dollar]ref\"");

            //now that the string file is ready, parse it into json.net
            //load the settings
            JsonLoadSettings settings = new JsonLoadSettings()
            {
                //ignore comments and load line info
                //"jsOn DoeSnT sUpPorT coMmAs"
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Load
            };
            JObject root = null;

            //if it's from the editor, then dump the file to disk to show an escaped version for debugging
            if (Settings.ApplicationVersion == ApplicationVersions.Alpha && p.FromEditor)
            {
                string filenameForDump = Path.GetFileNameWithoutExtension(p.CompletePath) + "_escaped" + Path.GetExtension(p.CompletePath);
                string filePathForDump = Path.Combine(Path.GetDirectoryName(p.CompletePath), filenameForDump);
                Logging.Debug("Dumping escaped file for debug before json.net parse: " + filePathForDump);
                File.WriteAllText(filePathForDump, file);
            }
            try
            {
                root = JObject.Parse(file, settings);
            }
            catch (JsonReaderException j)
            {
                Logging.Error("Failed to parse json file! {0}", Path.GetFileName(p.File));
                Logging.Debug(j.ToString());
                return;
            }

            //switch how it is handled based on the mode of the patch
            switch(p.Mode.ToLower())
            {
                case "add":
                    JsonAdd(p, root);
                    break;
                case "edit":
                    JsonEditRemove(p, root, true);
                    break;
                case "remove":
                    JsonEditRemove(p, root, false);
                    break;
                case "arrayadd":
                    JsonArrayAdd(p, root);
                    break;
                case "arrayremove":
                    JsonArrayRemoveClear(p, root, true);
                    break;
                case "arrayclear":
                    JsonArrayRemoveClear(p, root, false);
                    break;
                default:
                    Logging.Error("ERROR: Unknown json patch mode, {0}", p.Mode);
                    return;
            }

            //un-escape the string with all ref metadata and xvm references
            file = Utils.MacroReplace(file, ReplacementTypes.PatchFiles);

            //write to disk and finish
            File.WriteAllText(p.CompletePath, root.ToString());
            Logging.Debug("json patch completed successfully");
        }
        #endregion

        #region Json modes
        private static void JsonAdd(Patch p, JObject root)
        {
            //3 modes for json adding: regular add, add blank array, add blank object

            //match replace with [array] or [object] at the end, special case
            if(Regex.IsMatch(p.Replace, @".*\[array\]$"))
            {
                Logging.Debug("adding blank array detected");
                p.Replace = Regex.Replace(p.Replace, @".*\[array\]$", string.Empty);
                JsonAddBlank(p, root, false);
                return;
            }
            else if (Regex.IsMatch(p.Replace, @".*\[object\]$"))
            {
                Logging.Debug("adding blank object detected");
                p.Replace = Regex.Replace(p.Replace, @".*\[object\]$", string.Empty);
                JsonAddBlank(p, root, true);
                return;
            }

            //here means it's a standard json add
            //split the replace into array to make path for new object
            Logging.Debug("adding standard value");
            List<string> addPathArray = null;
            addPathArray = p.Replace.Split('/').ToList();

            //check it has at least 2 values
            if(addPathArray.Count < 2)
            {
                Logging.Error("add syntax or replace value must have at least 2 values separated by \"/\" in its path");
                return;
            }

            //last item in array is item to add
            string valueToAdd = addPathArray[addPathArray.Count - 1];
            valueToAdd = Utils.MacroReplace(valueToAdd, ReplacementTypes.PatchArguements);

            //then remove it
            addPathArray.RemoveAt(addPathArray.Count - 1);

            //same idea for the property name
            string propertyName = addPathArray[addPathArray.Count - 1];
            addPathArray.RemoveAt(addPathArray.Count - 1);

            //now form the full path (including any extra object paths used in the replace syntax)
            string fullPath = p.Path;
            if(addPathArray.Count > 0)
            {
                foreach(string s in addPathArray)
                {
                    fullPath = fullPath + "." + s;
                }
            }

            //get the root object from the original jsonPath
            JContainer objectRoot = null;
            try
            {
                objectRoot = (JContainer)root.SelectToken(p.Path);
            }
            catch (Exception exVal)
            {
                Logging.Error("error in jsonPath syntax: {0}", p.Path);
                Logging.Debug(exVal.ToString());
                return;
            }
            if(objectRoot == null)
            {
                Logging.Error("jsonPath does not exist: {0}", p.Path);
                return;
            }

            //foreach string still in the addPath array, go into the inner object
            foreach(string s in addPathArray)
            {
                objectRoot = (JContainer)objectRoot.SelectToken(s);

                //if it's null, then it does not exist, so make it
                if(objectRoot == null)
                {
                    //make a new property with key of name and value of object
                    JObject nextObject = new JObject();
                    JProperty nextProperty = new JProperty(s, nextObject);
                    objectRoot.Add(nextProperty);
                    objectRoot = nextObject;
                }
            }

            //add the property to the object
            JProperty prop = CreateJsonProperty(propertyName, valueToAdd);
            objectRoot.Add(prop);
        }

        private static void JsonAddBlank(Patch p, JObject root, bool jObject)
        {
            //replace field has now already been parsed
            //see if the object already exists in the full form (so include replace being the new object/array name)
            JContainer result = null;
            try
            {
                result = (JContainer)root.SelectToken(p.Path + "." + p.Replace);
            }
            catch (Exception array)
            {
                Logging.Error("error in replace syntax: {0}\n{1}", p.Replace, array.ToString());
                return;
            }
            if (result != null)
            {
                Logging.Error("cannot add blank array or object when already exists");
                return;
            }

            //here means the object/array does not exist, and can be added
            //get the container for adding the new blank object/array to
            JContainer pathForArray = (JContainer)root.SelectToken(p.Path);

            //make object reference and make it array or object
            JContainer newObject = null;
            if (jObject)
                newObject = new JObject();
            else
                newObject = new JArray();

            //make the property to hold the new object/array, key-value style
            JProperty prop = new JProperty(p.Replace, newObject);

            //add it to the container
            pathForArray.Add(prop);
        }

        private static void JsonEditRemove(Patch p, JObject root, bool edit)
        {
            //get the list of all items that match the path
            IEnumerable<JToken> jsonPathresults = null;
            try
            {
                jsonPathresults = root.SelectTokens(p.Path);
            }
            catch (Exception exResults)
            {
                Logging.Error("Error with jsonPath: {0}", p.Path);
                Logging.Error(exResults.ToString());
            }
            if (jsonPathresults == null || jsonPathresults.Count() == 0)
            {
                Logging.Warning("no results from jsonPath search");
                return;
            }

            //make sure results are all JValue
            List<JValue> Jresults = new List<JValue>();
            foreach (JToken jt in jsonPathresults)
            {
                if (jt is JValue Jvalue)
                {
                    Jresults.Add(Jvalue);
                }
                else
                {
                    Logging.Error("Expected results of type JValue, returned {0}", jt.Type.ToString());
                    return;
                }
            }

            //check that we have results
            Logging.Debug("number of Jvalues: {0}", Jresults.Count);
            if (Jresults.Count == 0)
            {
                Logging.Warning("Jresults count is 0 (is this the intent?)");
                return;
            }

            //foreach match from json search, match the result with search parameter
            foreach(JValue result in Jresults)
            {
                //parse the value to a string for comparison
                string jsonValue = JsonGetCompare(result);

                //only update the value if the regex search matches
                if(Regex.IsMatch(jsonValue,p.Search))
                {
                    if (edit)
                    {
                        Logging.Debug("regex match for result {0}, applying edit to {1}", jsonValue, p.Search);
                        UpdateJsonValue(result, p.Replace);
                    }
                    else
                    {
                        Logging.Debug("regex match for result {0}, removing", jsonValue);
                        //check if parent is array, we should not be removing from an array in this function
                        if(result.Parent is JArray)
                        {
                            Logging.Error("Selected from p.path is JValue and parent is JArray. Use arrayRemove for this function");
                            return;
                        }
                        //get the jProperty above it and remove itself
                        else if (result.Parent is JProperty prop)
                        {
                            prop.Remove();
                        }
                        else
                        {
                            Logging.Error("unknown parent type: {0}", result.Parent.GetType().ToString());
                        }
                    }
                }
                else
                {
                    Logging.Debug("json value {0} matches jsonPath but does not match regex search {1}", jsonValue, p.Search);
                }
            }
        }

        private static void JsonArrayAdd(Patch p, JObject root)
        {
            //check syntax of what was added
            List<string> addPathArray = null;
            addPathArray = p.Replace.Split('/').ToList();

            //maximum number of args for replace is 2
            if(addPathArray.Count > 2)
            {
                Logging.Error("invalid replace syntax: maximum arguments is 2. given: {0}", addPathArray.Count);
                return;
            }

            //get the property name (if exists, and value, and index to add to)
            string propertyName = addPathArray.Count == 2 ? addPathArray[0] : string.Empty;
            string valueToAdd = addPathArray[0];

            //check for index value in p.replace (name/value[index=NUMBER])
            string indexString = valueToAdd.Split(new string[] { @"[index=" }, StringSplitOptions.None)[1];

            //split off the end brace, default is index 0 (add it to array at bottom if none provided)
            int index = Utils.ParseInt(indexString.Split(']')[0], -1);

            //and get it out of the valueToAdd
            valueToAdd = valueToAdd.Split(new string[] { @"[index=" }, StringSplitOptions.None)[0];

            JArray array = JsonArrayGet(p, root);
            if (array == null)
            {
                Logging.Error("JArray is null");
                return;
            }

            //if index value is greater then count, then warning and set it to -1 (tells it to add to bottom of array)
            if(index >= array.Count)
            {
                if (index != 0)
                    Logging.Warning("index value ({0})>= array count ({1}), putting at end of the array (is this the intent?)", index, array.Count);
                index = -1;
            }

            //check that the correct type of array was found for expected add (key-value to value array, vise versa)
            if (array.Count > 0)
            {
                if ((array[0] is JValue) && (addPathArray.Count() == 2))
                {
                    Logging.WriteToLog("array is of JValues and 2 replace arguments given", Logfiles.Application, LogLevel.Error);
                    return;
                }
                else if (!(array[0] is JValue) && (addPathArray.Count() == 1))
                {
                    Logging.WriteToLog("array is not of JValues and only 1 replace arguments given", Logfiles.Application, LogLevel.Error);
                    return;
                }
            }

            //add the value/key-value pair to the array at the specified index
            JValue val = CreateJsonValue(valueToAdd);
            if (addPathArray.Count() == 2)
            {
                //add object with property
                if (index == -1)
                {
                    array.Add(new JObject(new JProperty(propertyName, val)));
                }
                else
                {
                    array.Insert(index, (new JObject(new JProperty(propertyName, val))));
                }
            }
            else
            {
                //add value
                if (index == -1)
                {
                    array.Add(val);
                }
                else
                {
                    array.Insert(index, val);
                }
            }
        }

        private static void JsonArrayRemoveClear(Patch p, JObject root, bool remove)
        {
            JArray array = JsonArrayGet(p, root);
            if (array == null)
            {
                Logging.Error("JArray is null");
                return;
            }

            //can't remove from an array if it's empty #rollSafe
            if (array.Count == 0)
            {
                Logging.Error("array is already empty");
                return;
            }

            //search and remove each item that matches. if it's remove mode, then stop at the first one
            bool found = false;
            for (int i = 0; i < array.Count; i++)
            {
                string jsonResult = JsonGetCompare(array[i] as JValue);
                if (Regex.IsMatch(jsonResult, p.Search))
                {
                    found = true;
                    array[i].Remove();
                    i--;
                    if (remove)
                        break;
                }
            }
            if (!found)
            {
                Logging.Warning("no results found for search \"{0}\", with path \"{1}\"", p.Search, p.Path);
                return;
            }
        }
        #endregion

        #region Helpers

        private static string JsonGetCompare(JValue result)
        {
            //parse the value to a string for comparison
            string jsonValue = string.Empty;
            if (result.Value is string str)
                jsonValue = str;
            else if (result.Value is char c)
                jsonValue = c.ToString();
            else if (result.Value is bool b)
                jsonValue = b.ToString().ToLower();
            else
                jsonValue = result.Value.ToString();
            return jsonValue;
        }

        private static void UpdateJsonValue(JValue jvalue, string value)
        {
            //determine what type value should be used for the json item based on attempted parsing
            if (Utils.ParseBool(value, out bool resultBool))
                jvalue.Value = resultBool;
            else if (Utils.ParseFloat(value, out float resultFloat))
                jvalue.Value = resultFloat;
            else if (Utils.ParseInt(value, out int resultInt))
                jvalue.Value = resultInt;
            else
                jvalue.Value = value;
            Logging.Debug("Json value parsed as {0}", jvalue.Value.GetType().ToString());
        }

        private static JValue CreateJsonValue(string value)
        {
            //determine what type value should be used for the json item based on attempted parsing
            JValue jvalue = null;
            if (Utils.ParseBool(value, out bool resultBool))
                jvalue = new JValue(resultBool);
            else if (Utils.ParseFloat(value, out float resultFloat))
                jvalue = new JValue(resultFloat);
            else if (Utils.ParseInt(value, out int resultInt))
                jvalue = new JValue(resultInt);
            else
                jvalue = new JValue(value);
            Logging.Debug("Json value parsed as {0}", jvalue.Value.GetType().ToString());
            return jvalue;
        }

        private static JProperty CreateJsonProperty(string propertyName, string value)
        {
            JValue jvalue = CreateJsonValue(value);
            return new JProperty(propertyName, jvalue);
        }

        private static JArray JsonArrayGet(Patch p, JObject root)
        {
            //get the root object from the original jsonPath
            JContainer objectRoot = null;
            try
            {
                objectRoot = (JContainer)root.SelectToken(p.Path);
            }
            catch (Exception exVal)
            {
                Logging.Error("error in jsonPath syntax: {0}", p.Path);
                Logging.Debug(exVal.ToString());
            }
            if (objectRoot == null)
            {
                Logging.Error("path does not exist: {0}", p.Path);
            }
            return objectRoot as JArray;
        }

        private static string EscapeXvmRefrences(string file)
        {
            //replace all xvm style references with escaped versions that won't cause invalid parsing
            //split regex based on the start of the xvm reference (the dollar, whitespace, left bracket, whitespace, quote)
            //note it will need to be put back in
            string[] fileSplit = Regex.Split(file, @"\$[ \t]*\{[ \t]*""");
            for (int i = 1; i < fileSplit.Length; i++)
            {
                fileSplit[i] = @"""[xvm_dollar][lbracket][quote]" + fileSplit[i];
                //looks like:      "[xvm_dollar][lbracket][quote]damageLog.log.x" }
                //"battleLoading": "[xvm_dollar][lbracket][quote]battleLoading.xc":"battleLoading"},

                //split it again so we don't replace more than we need to
                //stop at the next right bracket to indicate the end of the xvm reference
                //the escaped string can always be treated as a value in the key-value pair system
                string[] splitAgain = fileSplit[i].Split('}');

                //check for if it is a file:path reference and escape it
                if (Regex.IsMatch(splitAgain[0], @"""[\t ]*\:[\t ]*"""))
                    splitAgain[0] = Regex.Replace(splitAgain[0], @"""[\t ]*\:[\t ]*""", @"[quote][colon][quote]");
                //if that style, would look like "battleLoading": "[xvm_dollar][lbracket][quote]battleLoading.xc[quote][colon][quote]battleLoading"},

                //join it back to fileSplit
                fileSplit[i] = string.Join("}", splitAgain);

                //match the first occurrence only of the end of the reference ("})
                Match m = Regex.Match(fileSplit[i], @"""[\t ]*\}");
                if (m.Success)
                {
                    //create the string of everything before the match (up to and not include the quote)
                    string before = fileSplit[i].Substring(0, m.Index);

                    //create the string of everything after the match (after and not include the right bracket)
                    string after = fileSplit[i].Substring(m.Index + m.Length);

                    //make the string with the escape for the end of the xvm reference
                    fileSplit[i] = before + @"[quote][xvm_rbracket]""" + after;
                    //finished result: "[xvm_dollar][lbracket][quote]damageLog.log.x[quote][xvm_rbracket]"
                    //"battleLoading": "[xvm_dollar][lbracket][quote]battleLoading.xc[quote][colon][quote]battleLoading[quote][xvm_rbracket]",
                }
            }
            file = string.Join("", fileSplit);
            return file;
        }

        //returns the folder(s) to get to the xvm config folder directory
        public static string GetXvmFolderName()
        {
            //form where it should be
            string xvmBootFile = Path.Combine(Settings.WoTDirectory, "res_mods\\configs\\xvm\\xvm.xc");

            //check if it exists there
            if (!File.Exists(xvmBootFile))
            {
                Logging.Error("extractor asked to get location of xvm folder name, but boot file does not exist! returning \"default\"");
                return "default";
            }

            string fileContents = File.ReadAllText(xvmBootFile);

            //patch block comments out
            fileContents = Regex.Replace(fileContents, @"\/\*.*\*\/", string.Empty, RegexOptions.Singleline);

            //remove return character
            fileContents = fileContents.Replace("\r",string.Empty);

            //patch single line comments out
            string[] removeComments = fileContents.Split('\n');
            StringBuilder bootBuilder = new StringBuilder();
            foreach (string s in removeComments)
            {
                if (Regex.IsMatch(s, @"\/\/.*$"))
                    continue;
                bootBuilder.Append(s + "\n");
            }
            fileContents = bootBuilder.ToString().Trim();

            //get the path from the json style
            Match match = Regex.Match(fileContents, @"\${.*:.*}");
            string innerJsonValue = match.Value;

            //the second quote set is what we're interested in, the file path
            string[] splitIt = innerJsonValue.Split('"');
            string filePath = splitIt[1];

            //split again to get just the folder name
            string folderName = filePath.Split('/')[0];

            return folderName;
        }
        #endregion
    }
}
