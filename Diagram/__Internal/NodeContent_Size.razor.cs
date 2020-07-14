﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Excubo.Blazor.Diagrams.__Internal
{
    public partial class NodeContent : IDisposable
    {
        [Inject] private IJSRuntime js { get; set; }
        private DotNetObjectReference<NodeContent> js_interop_reference_to_this;
        protected override async Task OnAfterRenderAsync(bool first_render)
        {
            if (first_render)
            {
                var result = await js.GetDimensionsAsync(element);
                js_interop_reference_to_this ??= DotNetObjectReference.Create(this);
                await js.RegisterResizeObserverAsync(element, js_interop_reference_to_this);
                SizeCallback?.Invoke(result);
            }
            await base.OnAfterRenderAsync(first_render);
        }
        public class Dimensions
        {
            public double Width { get; set; }
            public double Height { get; set; }
        }
        [JSInvokable]
        public void OnResize(Dimensions dimensions)
        {
            SizeCallback?.Invoke((dimensions.Width, dimensions.Height));
        }
        public void Dispose()
        {
            if (element.Id != null)
            {
                js.UnobserveResizesAsync(element);
            }
            if (js_interop_reference_to_this != null)
            {
                js_interop_reference_to_this.Dispose();
            }
        }
    }
}