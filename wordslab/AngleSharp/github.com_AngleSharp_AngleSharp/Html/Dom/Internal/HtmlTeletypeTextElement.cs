﻿namespace AngleSharp.Html.Dom
{
    using AngleSharp.Dom;
    using System;

    /// <summary>
    /// The tt HTML element.
    /// </summary>
    sealed class HtmlTeletypeTextElement : HtmlElement
    {
        public HtmlTeletypeTextElement(Document owner, String? prefix = null)
            : base(owner, TagNames.Tt, prefix, NodeFlags.HtmlFormatting)
        {
        }
    }
}
