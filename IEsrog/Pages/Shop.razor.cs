using IEsrog.Components;
using IEsrog.Models;
using IEsrog.Services;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace IEsrog.Pages;

public partial class Shop
{
    ProductGrpModel? _Product;
    ProductGrpModel[] _Products = [];

    [Parameter]
    public string? ForOrderWithId { get; set; }

    [Parameter]
    public string? ForUserId { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public AppStateService AppState { get; set; } = null!;

    public bool IsInUserSelection { get; set; }

    public string FilterName { get; set; }

    CopyOrderPopup _CopyOrder;

    // OrderItem OrderItem = new OrderItem();

    async void AddItem(Product product, int optInx, int n = 1, bool toOverride = false, ExtraOptions? option = null)
    {
        string? opt = null;
        if (optInx > -1 && product.Options?.Length > optInx)
        {
            opt = product.Options[optInx];
        }

        var item = new OrderItem
        {
            Id = product.Id,
            Qty = n,
            Price = product.Price,
            PriceType = product.PricesType,
            Option = opt,
            Name = product.Name,
            Category = product.Category,
            ProductId = product.Id,
            ExtraOption = option
        };

        if (!string.IsNullOrEmpty(ForOrderWithId))
        {
            if (ObjectId.TryParse(ForOrderWithId, out _))
            {
                _ = await MainService.AddItemToOrder(item, ForOrderWithId);
            }
        }
        else if (!string.IsNullOrWhiteSpace(ForUserId))
        {
            _ = await MainService.AddItemToCart(item, ForUserId, toOverride);
        }
        else
        {
            _ = await MainService.AddItemToCart(item, null, toOverride);
        }
        StateHasChanged();
    }

    async void SubtractItem(Product product, int optInx, int n = 1)
    {
        string? opt = null;
        if (optInx > -1 && product.Options?.Length > optInx)
        {
            opt = product.Options[optInx];
        }

        var item = new OrderItem
        {
            Id = product.Id,
            Qty = -Math.Abs(n),
            Price = product.Price,
            PriceType = product.PricesType,
            Option = opt,
            Name = product.Name,
            Category = product.Category,
            ProductId = product.Id
        };


        if (!string.IsNullOrEmpty(ForOrderWithId))
        {
            if (ObjectId.TryParse(ForOrderWithId, out _))
            {
                _ = await MainService.AddItemToOrder(item, ForOrderWithId);
            }
        }
        else if (!string.IsNullOrWhiteSpace(ForUserId))
        {
            _ = await MainService.AddItemToCart(item, ForUserId);
        }
        else
        {
            _ = await MainService.AddItemToCart(item);
        }
    }

    void CopyOrder()
    {
        var items = MainService.GetLastYearOrder();
        _CopyOrder.Show(items.ToArray());
    }

    async void CopyOrderOnClosed(object? sender, bool e)
    {
        if (!e) return;

        await MainService.CreateDuplicateOrder();
        StateHasChanged();
    }

    void SelectUser()
    {
        IsInUserSelection = true;
    }

    void SelectUser(User user)
    {
        State.ForUser = State.User.Id == user.Id ? null : user;
        IsInUserSelection = false;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            _CopyOrder.Closed += CopyOrderOnClosed;
            OnDisposed = () => _CopyOrder.Closed -= CopyOrderOnClosed;

            _Products = ProductGrpModel.Build(State.Products);
            StateHasChanged();
        }
    }

    public void ShowAddingItem(int i)
    {
        NavigationManager.NavigateTo($"/shop/product/{i}");
    }

}