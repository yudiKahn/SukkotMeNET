using IEsrog.Services;
using Microsoft.AspNetCore.Components;

namespace IEsrog.Models
{
    public class PageBase : ComponentBase, IDisposable
    {
        bool _UsedOnAfterRender;
        protected Action? OnDisposed;
        protected Action? OnStateChanged;

        [Inject]
        public MainStateService MainService { get; set; }

        public void Dispose()
        {
            MainService.StateHasChanged -= OnStateHasChanged;
            OnDisposed?.Invoke();
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
                MainService.StateHasChanged += OnStateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if(_UsedOnAfterRender) return;

            _UsedOnAfterRender = true;
            if (firstRender)
            {
                MainService.StateHasChanged += OnStateHasChanged;
            }
        }

        public virtual void OnStateHasChanged(object? sender, EventArgs args)
        {
            InvokeAsync(StateHasChanged);
            OnStateChanged?.Invoke();
        }


    }
}
