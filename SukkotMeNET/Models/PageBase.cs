using Microsoft.AspNetCore.Components;
using SukkotMeNET.Services;

namespace SukkotMeNET.Models
{
    public class PageBase : ComponentBase, IDisposable
    {
        [Inject]
        public MainStateService MainService { get; set; }

        public void Dispose()
        {
            MainService.StateHasChanged -= InvokeStateHasChanged;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                MainService.StateHasChanged += InvokeStateHasChanged;
            }
        }

        void InvokeStateHasChanged(object? sender, EventArgs args) => InvokeAsync(StateHasChanged);
    }
}
