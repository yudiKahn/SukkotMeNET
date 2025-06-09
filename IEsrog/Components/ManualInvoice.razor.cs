using EllipticCurve.Utils;
using IEsrog.Extensions;
using IEsrog.Models;
using IEsrog.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;

namespace IEsrog.Components;

public partial class ManualInvoice
{
    const int height = 1500;
    const int width = 800;

    [Inject] IJSRuntime Js { get; set; } = null!;
    [Inject] InvoiceService InvoiceService { get; set; } = null!;
    [Inject] MainStateService MainService { get; set; } = null!;
    [Inject] NavigationManager NavManager { get; set; } = null!;

    [Parameter]
    public Action<Order, User>? OnSave { get; set; }

    CopyOrderPopup _CopyOrder = null!;
    Order? _FromOrder;

    [Parameter]
    public Order? FromOrder
    {
        get => _FromOrder;
        set
        {
            _FromOrder = value;
            if (value != null)
            {
                _Order = value.Clone();
                if (!string.IsNullOrWhiteSpace(_Order.UserId))
                {
                    _User = State.AdminState.AllUsers
                        .First(u => u.Id == _Order.UserId)
                        .Clone();
                }
            }
        }
    }

    [Parameter]
    public bool DisableUserSelection { get; set; } = false;

    string? _InvoicePdf;
    public string InvoicePdf
    {
        get => _InvoicePdf ?? string.Empty;
        set => _InvoicePdf = value;
    }

    #region Property: NameFilter

    string _NameFilter = string.Empty;

    public string NameFilter
    {
        get => _NameFilter;
        set
        {
            _NameFilter = value;

            _FilteredUsers = State.AdminState.AllUsers
                .Where(u => $"{u.FirstName}{u.LastName}".Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (_FilteredUsers.Count == 0)
            {
                _Order.Recipient = value;
            }
            else
            {
                _Order.Recipient = null;
            }
        }
    }

    #endregion

    Order _Order = new();
    User _User = new();
    List<User> _FilteredUsers = new();
    bool _ViewOnly;



    void ChangeViewOnly()
    {
        if (_ViewOnly && (!MainService.CanEditOrder(_FromOrder) || !MainService.CanEditOrder(_Order)))
            return;
        _ViewOnly = !_ViewOnly;
        StateHasChanged();
    }

    public void AddItem(OrderItem item)
    {
        _Order.Items.AddOrMerge(item.Clone());
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _Order.Id = ObjectId.GenerateNewId().ToString();
        _Order.CreatedAt = DateTime.Now;
        _FilteredUsers = State.AdminState.AllUsers;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (string.IsNullOrWhiteSpace(InvoicePdf) && _FromOrder is { } order)
        {
            var html = InvoiceService.GetInvoiceHtml(order, _User);
            InvoicePdf = await Js.InvokeAsync<string>(
                Constants.JavaScriptFunctions.GetImageFromHTML,
                html, width, height
            );
            StateHasChanged();
        }

        if (!_ViewOnly && _FromOrder != null &&
            (!MainService.CanEditOrder(_FromOrder) || !MainService.CanEditOrder(_Order)))
        {
            _ViewOnly = true;
            StateHasChanged();
        }
    }

    void Save()
    {
        OnSave?.Invoke(_Order, _User);
    }
    void OnProdChange(ChangeEventArgs e, OrderItem item)
    {
        const string del = "~~~";
        if (e.Value is not string pid || string.IsNullOrWhiteSpace(pid)) return;

        string? opt = null;
        string? extraOption = null;
        if (pid.Contains(del))
        {
            var s = pid.Split(del);
            pid = s[0];
            opt = s[1];
            if (s.Length == 3)
                extraOption = s[1];

        }

        var prod = State.Products.First(p => p.Id == pid).Clone();
        item.Name = prod.Name;
        item.Price = prod.Price;
        item.ProductId = prod.Id;
        item.PriceType = prod.PricesType;
        item.Category = prod.Category;
        item.Option = opt;
        item.ExtraOption = prod.ExtraOptions?.FirstOrDefault(eo => eo.Option == extraOption)?.Clone();
    }

    void Add() => _Order.Items.Add(new OrderItem() { ProductId = string.Empty });

    void Remove(OrderItem item) => _Order.Items.Remove(item);

    void OnUserChanged(ChangeEventArgs e)
    {
        if (e.Value is not string uid || string.IsNullOrWhiteSpace(uid)) return;

        const string newUser = "-1";

        if (uid == newUser)
        {
            NavManager.NavigateTo("/admin/createuser/admin-manualInvoice");
            return;
        }

        var user = State.AdminState.AllUsers.First(u => u.Id == uid).Clone();
        _User = user;
        _Order.UserId = user.Id;
        _NameFilter = user.ToString();
        StateHasChanged();
    }

    void Reorder()
    {
        var uid = _User?.Id;
        if (string.IsNullOrWhiteSpace(uid))
            return;

        var allOrders = State.AdminState.AllOrders
            .Where(o => o.UserId == uid && (_FromOrder is null || o.Id != _FromOrder.Id))
            .OrderBy(o => o.CreatedAt)
            .ToArray();

        //var items = MainService.GetLastYearOrder(uid);

        _CopyOrder.Show(allOrders);
        // _Order.Items.AddOrMergeRange(items.ToArray());
    }

    async void Print()
    {
        if (_FromOrder is not { } order) return;
        
        var html = InvoiceService.GetInvoiceHtml(order, _User);
        await Js.InvokeAsync<string>(
            Constants.JavaScriptFunctions.PrintImageFromHTML,
            html, width, height
        );
    }

}