using LogicUI.FancyTextRendering;
using LogicUI.FancyTextRendering.MarkdownLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class MarkdownParser
{
    public MarkdownRenderingSettings settings;
    public string ToRichText(string markdownText)
    {
        if (string.IsNullOrEmpty(markdownText))
        {
            return string.Empty;
        }

        // Separate into lines
        var lines = new List<MarkdownLine>();

        using (var reader = new StringReader(markdownText))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(new MarkdownLine()
                {
                    Builder = new StringBuilder(line)
                });
            }
        }

        // Send lines to be processed
        foreach (var processor in BuiltInLineProcessors)
            processor.Process(lines, settings);


        var builder = new StringBuilder();

        foreach (var line in lines)
        {
            if (!line.DeleteLineAfterProcessing)
                builder.AppendLine(line.Finish());
        }

        return builder.ToString();
    }

    private static readonly IReadOnlyList<MarkdownLineProcessorBase> BuiltInLineProcessors = new MarkdownLineProcessorBase[]
    {
            // Order of processing here does matter, be mindful when adding to this list.
            new AutoLinksHttp(),
            new AutoLinksHttps(),
            new UnorderedLists(),
            new OrderedLists(),
            new Bold(),
            new Italics(),
            new Strikethrough(),
            new SuperscriptChain(), // Important to process chain before single!
            new SuperscriptSingle(),
            new Monospace(),
            new Headers(),
            new Links(),
    };
}
