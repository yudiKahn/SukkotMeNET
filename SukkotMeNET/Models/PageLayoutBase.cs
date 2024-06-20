using IEsrog.Services;
using Microsoft.AspNetCore.Components;

namespace IEsrog.Models
{
    public class PageLayoutBase : LayoutComponentBase
    {
        bool _UsedOnAfterRender;

        [Inject]
        public MainStateService MainService { get; set; }

        public void Dispose()
        {
            MainService.StateHasChanged -= InvokeStateHasChanged;
        }

        public bool SetProperty<T>(ref T prop, T val)
        {
            if (EqualityComparer<T>.Default.Equals(prop, val))
                return false;

            prop = val;
            StateHasChanged();
            return true;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (_UsedOnAfterRender) return;

            _UsedOnAfterRender = true;
            if (firstRender)
                MainService.StateHasChanged += InvokeStateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (_UsedOnAfterRender) return;

            _UsedOnAfterRender = true;
            if (firstRender)
            {
                MainService.StateHasChanged += InvokeStateHasChanged;
            }
        }

        void InvokeStateHasChanged(object? sender, EventArgs args) => InvokeAsync(StateHasChanged);
    }
}
