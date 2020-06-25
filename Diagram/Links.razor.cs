﻿using Excubo.Blazor.Diagrams.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Excubo.Blazor.Diagrams
{
    public partial class Links
    {
        /// <summary>
        /// Default link type for links created as <Link />
        /// </summary>
        [Parameter] public LinkType DefaultType { get; set; }
        /// <summary>
        /// Default arrow type for links created as <Link />
        /// </summary>
        [Parameter] public Arrow DefaultArrow { get; set; }
        /// <summary>
        /// Callback for when a link is added.
        /// </summary>
        [Parameter] public Action<LinkBase> OnAdd { get; set; }
        /// <summary>
        /// Callback that is executed if the remove action wasn't cancelled.
        /// </summary>
        [Parameter] public Action<LinkBase> OnRemove { get; set; }
        /// <summary>
        /// Callback for when a link source or target is changed.
        /// </summary>
        [Parameter] public Action<LinkBase> OnModified { get; set; } // TODO use
        [CascadingParameter] public Diagram Diagram { get; set; }
        protected override void OnParametersSet()
        {
            System.Diagnostics.Debug.Assert(Diagram != null);
            Diagram.Links = this;
            base.OnParametersSet();
        }
        private readonly List<LinkBase> all_links = new List<LinkBase>();
        private readonly List<LinkData> internally_generated_links = new List<LinkData>();
        internal void Register(LinkBase link)
        {
            if (!all_links.Contains(link))
            {
                all_links.Add(link);
                OnAdd?.Invoke(link);
            }
        }
        internal void Add(LinkBase link)
        {
            all_links.Add(link);
            if (link.Deleted)
            {
                link.MarkUndeleted();
            }
            else
            {
                internally_generated_links.Add(new LinkData { Source = link.Source, Target = link.Target, Arrow = link.Arrow, Type = link.GetType(), OnCreate = (_) => { } });
                generated_links_ref.TriggerStateHasChanged();
                OnAdd?.Invoke(link);
            }
        }
        internal void Remove(LinkBase link)
        {
            _ = all_links.Remove(link);
            var match = internally_generated_links.FirstOrDefault(l => l.Source == link.Source && l.Target == link.Target);
            if (match != null)
            {
                _ = internally_generated_links.Remove(match);
                generated_links_ref.TriggerStateHasChanged();
            }
            else
            {
                link.MarkDeleted();
            }
            OnRemove?.Invoke(link);
        }
        internal void AddNewLink(NodeBase node, MouseEventArgs e, Action<LinkBase> on_link_create)
        {
            var source_point = new NodeAnchor
            {
                Node = node,
                RelativeX = e.RelativeXTo(node),
                RelativeY = e.RelativeYTo(node)
            };
            var target_point = new NodeAnchor
            {
                RelativeX = e.RelativeXToOrigin(Diagram),
                RelativeY = e.RelativeYToOrigin(Diagram)
            };
            internally_generated_links.Add(new LinkData { Source = source_point, Target = target_point, LinkType = DefaultType, Arrow = DefaultArrow, OnCreate = on_link_create });
            generated_links_ref.TriggerStateHasChanged();
        }
        internal void TriggerStateHasChanged() => StateHasChanged();
        internal void Redraw()
        {
            foreach (var link in all_links)
            {
                link.TriggerStateHasChanged();
            }
        }
    }
}
