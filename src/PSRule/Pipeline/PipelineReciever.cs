﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using PSRule.Data;
using PSRule.Parser;
using PSRule.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace PSRule.Pipeline
{
    public delegate IEnumerable<PSObject> VisitTargetObject(PSObject sourceObject);
    public delegate IEnumerable<PSObject> VisitTargetObjectAction(PSObject sourceObject, VisitTargetObject next);

    internal static class PipelineReceiverActions
    {
        private const string InputFileInfo_GitHead = ".git/HEAD";

        private static readonly PSObject[] EmptyArray = Array.Empty<PSObject>();

        public static IEnumerable<PSObject> PassThru(PSObject targetObject)
        {
            yield return targetObject;
        }

        public static IEnumerable<PSObject> DetectInputFormat(PSObject sourceObject, VisitTargetObject next)
        {
            var pathExtension = GetPathExtension(sourceObject);

            // Handle JSON
            if (pathExtension == ".json" || pathExtension == ".jsonc")
            {
                return ConvertFromJson(sourceObject, next);
            }
            // Handle YAML
            else if (pathExtension == ".yaml" || pathExtension == ".yml")
            {
                return ConvertFromYaml(sourceObject, next);
            }
            // Handle Markdown
            else if (pathExtension == ".md" || pathExtension == ".markdown")
            {
                return ConvertFromMarkdown(sourceObject, next);
            }
            // Handle PowerShell Data
            else if (pathExtension == ".psd1")
            {
                return ConvertFromPowerShellData(sourceObject, next);
            }
            return new PSObject[] { sourceObject };
        }

        public static IEnumerable<PSObject> ConvertFromJson(PSObject sourceObject, VisitTargetObject next)
        {
            // Only attempt to deserialize if the input is a string, file or URI
            if (!IsAcceptedType(sourceObject))
                return new PSObject[] { sourceObject };

            var reader = ReadAsReader(sourceObject, out TargetSourceInfo source);
            try
            {
                var jsonReader = AsJsonTextReader(reader);
                var d = new JsonSerializer();
                d.Converters.Add(new PSObjectArrayJsonConverter());
                var value = d.Deserialize<PSObject[]>(jsonReader);
                NoteSource(value, source);
                return VisitItems(value, next);
            }
            catch (Exception ex)
            {
                if (source != null && !string.IsNullOrEmpty(source.File))
                {
                    RunspaceContext.CurrentThread.Writer.ErrorReadFileFailed(source.File, ex);
                    return Array.Empty<PSObject>();
                }
                throw;
            }
            finally
            {
                reader.Dispose();
            }
        }

        public static IEnumerable<PSObject> ConvertFromYaml(PSObject sourceObject, VisitTargetObject next)
        {
            // Only attempt to deserialize if the input is a string, file or URI
            if (!IsAcceptedType(sourceObject))
                return new PSObject[] { sourceObject };

            var d = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new PSObjectYamlTypeConverter())
                .WithNodeDeserializer(
                    inner => new PSObjectYamlDeserializer(inner),
                    s => s.InsteadOf<YamlConvertibleNodeDeserializer>())
                .Build();

            var reader = ReadAsReader(sourceObject, out TargetSourceInfo source);
            try
            {
                var parser = new YamlDotNet.Core.Parser(reader);
                var result = new List<PSObject>();
                parser.TryConsume<StreamStart>(out _);
                while (parser.Current is DocumentStart)
                {
                    var item = d.Deserialize<PSObject[]>(parser);
                    if (item == null)
                        continue;

                    NoteSource(item, source);
                    var items = VisitItems(item.ToArray(), next);

                    if (items == null)
                        continue;

                    result.AddRange(items);
                }

                if (result.Count == 0)
                    return EmptyArray;

                return result.ToArray();
            }
            catch (Exception ex)
            {
                if (source != null && !string.IsNullOrEmpty(source.File))
                {
                    RunspaceContext.CurrentThread.Writer.ErrorReadFileFailed(source.File, ex);
                    return Array.Empty<PSObject>();
                }
                throw;
            }
            finally
            {
                reader.Dispose();
            }
        }

        public static IEnumerable<PSObject> ConvertFromMarkdown(PSObject sourceObject, VisitTargetObject next)
        {
            // Only attempt to deserialize if the input is a string or a file
            if (!IsAcceptedType(sourceObject))
                return new PSObject[] { sourceObject };

            var markdown = ReadAsString(sourceObject, out TargetSourceInfo source);
            var value = MarkdownConvert.DeserializeObject(markdown);
            NoteSource(value, source);
            return VisitItems(value, next);
        }

        public static IEnumerable<PSObject> ConvertFromPowerShellData(PSObject sourceObject, VisitTargetObject next)
        {
            // Only attempt to deserialize if the input is a string or a file
            if (!IsAcceptedType(sourceObject))
                return new PSObject[] { sourceObject };

            var data = ReadAsString(sourceObject, out TargetSourceInfo source);
            var ast = System.Management.Automation.Language.Parser.ParseInput(data, out _, out _);
            var hashtables = ast.FindAll(item => item is System.Management.Automation.Language.HashtableAst, false);
            if (hashtables == null)
                return EmptyArray;

            var result = new List<PSObject>();
            foreach (var hashtable in hashtables)
            {
                if (hashtable?.Parent?.Parent?.Parent?.Parent == ast)
                    result.Add(PSObject.AsPSObject(hashtable.SafeGetValue()));
            }
            var value = result.ToArray();
            NoteSource(value, source);
            return VisitItems(value, next);
        }

        public static IEnumerable<PSObject> ConvertFromGitHead(PSObject sourceObject, VisitTargetObject next)
        {
            // Only attempt to convert if Git HEAD file
            if (!IsGitHead(sourceObject))
                return new PSObject[] { sourceObject };

            var value = PSObject.AsPSObject(GetRepositoryInfo(sourceObject));
            return VisitItems(new PSObject[] { value }, next);
        }

        public static IEnumerable<PSObject> ReadObjectPath(PSObject sourceObject, VisitTargetObject source, string objectPath, bool caseSensitive)
        {
            if (!ObjectHelper.GetField(bindingContext: null, targetObject: sourceObject, name: objectPath, caseSensitive: caseSensitive, value: out object nestedObject))
                return EmptyArray;

            var nestedType = nestedObject.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(nestedType))
            {
                var result = new List<PSObject>();
                foreach (var item in (nestedObject as IEnumerable))
                    result.Add(PSObject.AsPSObject(item));

                return result.ToArray();
            }
            else
            {
                return new PSObject[] { PSObject.AsPSObject(nestedObject) };
            }
        }

        private static string GetPathExtension(PSObject sourceObject)
        {
            if (sourceObject.BaseObject is InputFileInfo inputFileInfo)
                return inputFileInfo.Extension;

            if (sourceObject.BaseObject is FileInfo fileInfo)
                return fileInfo.Extension;

            if (sourceObject.BaseObject is Uri uri)
                return Path.GetExtension(uri.OriginalString);

            return null;
        }

        private static bool IsAcceptedType(PSObject sourceObject)
        {
            return sourceObject.BaseObject is string || sourceObject.BaseObject is InputFileInfo || sourceObject.BaseObject is FileInfo || sourceObject.BaseObject is Uri;
        }

        private static bool IsGitHead(PSObject sourceObject)
        {
            return sourceObject.BaseObject is InputFileInfo info && info.DisplayName == InputFileInfo_GitHead;
        }

        private static RepositoryInfo GetRepositoryInfo(PSObject sourceObject)
        {
            if (!(sourceObject.BaseObject is InputFileInfo inputFileInfo))
                return null;

            var headRef = GitHelper.GetHeadRef(inputFileInfo.DirectoryName);
            return new RepositoryInfo(inputFileInfo.BasePath, headRef);
        }

        private static string ReadAsString(PSObject sourceObject, out TargetSourceInfo sourceInfo)
        {
            sourceInfo = null;
            if (sourceObject.BaseObject is string)
            {
                return sourceObject.BaseObject.ToString();
            }
            else if (sourceObject.BaseObject is InputFileInfo inputFileInfo)
            {
                sourceInfo = new TargetSourceInfo(inputFileInfo);
                using (var reader = new StreamReader(inputFileInfo.FullName))
                {
                    return reader.ReadToEnd();
                }
            }
            else if (sourceObject.BaseObject is FileInfo fileInfo)
            {
                sourceInfo = new TargetSourceInfo(fileInfo);
                using (var reader = new StreamReader(fileInfo.FullName))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                var uri = sourceObject.BaseObject as Uri;
                sourceInfo = new TargetSourceInfo(uri);
                using (var webClient = new WebClient())
                {
                    return webClient.DownloadString(uri);
                }
            }
        }

        private static TextReader ReadAsReader(PSObject sourceObject, out TargetSourceInfo sourceInfo)
        {
            sourceInfo = null;
            if (sourceObject.BaseObject is string)
            {
                return new StringReader(sourceObject.BaseObject.ToString());
            }
            else if (sourceObject.BaseObject is InputFileInfo inputFileInfo)
            {
                sourceInfo = new TargetSourceInfo(inputFileInfo);
                return new StreamReader(inputFileInfo.FullName);
            }
            else if (sourceObject.BaseObject is FileInfo fileInfo)
            {
                sourceInfo = new TargetSourceInfo(fileInfo);
                return new StreamReader(fileInfo.FullName);
            }
            else
            {
                var uri = sourceObject.BaseObject as Uri;
                sourceInfo = new TargetSourceInfo(uri);
                using (var webClient = new WebClient())
                {
                    return new StringReader(webClient.DownloadString(uri));
                }
            }
        }

        private static IEnumerable<PSObject> VisitItems(PSObject[] value, VisitTargetObject next)
        {
            if (value == null)
                return EmptyArray;

            var result = new List<PSObject>();
            foreach (var item in value)
            {
                var items = next(item);
                if (items == null)
                    continue;

                result.AddRange(items);
            }

            if (result.Count == 0)
                return EmptyArray;

            return result.ToArray();
        }

        private static void NoteSource(PSObject[] value, TargetSourceInfo source)
        {
            if (value == null || value.Length == 0 || source == null)
                return;

            for (var i = 0; i < value.Length; i++)
                NoteSource(value[i], source);
        }

        private static void NoteSource(PSObject value, TargetSourceInfo source)
        {
            if (value == null || source == null)
                return;

            value.UseTargetInfo(out PSRuleTargetInfo targetInfo);
            targetInfo.UpdateSource(source);
        }

        private static JsonTextReader AsJsonTextReader(TextReader reader)
        {
            return new JsonTextReader(reader);
        }
    }
}
