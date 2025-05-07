using System;
using System.Collections.Generic;
using System.Text;

namespace LogicUI.FancyTextRendering.MarkdownLogic
{
    internal abstract class MarkdownLineProcessorBase
    {
        public void Process(IReadOnlyList<MarkdownLine> lines, MarkdownRenderingSettings settings)
        {
            if (!AllowedToProcess(settings))
                return;

            ProcessInternal(lines, settings);
        }

        protected virtual bool AllowedToProcess(MarkdownRenderingSettings settings) => true;
        protected abstract void ProcessInternal(IReadOnlyList<MarkdownLine> lines, MarkdownRenderingSettings settings);
    }
}