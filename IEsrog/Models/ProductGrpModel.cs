using IEsrog.Extensions;

namespace IEsrog.Models;

public class ProductGrpModel
{
    Product[] _Products = [];

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public (double, string)[] Prices { get; set; } = [];
    public string[] Options { get; set; } = [];
    public (double, string)[] Extras { get; set; } = [];


    public Product? GetProduct(double price, string? priceType = null, string? option = null, string? extra = null)
    {
        return _Products.FirstOrDefault(p =>
            p.Price == price && p.PricesType == priceType &&
            p.Options?.Contains(option) == true &&
            p.ExtraOptions?.Any(e => e.Option == extra) == true);
    }


    public static ProductGrpModel[] Build(IEnumerable<Product> prods)
    {
        var groups = prods.GroupBy(p =>
                Enum.TryParse<ProductCategory>(p.Category, out var c) &&
                c == ProductCategory.Sets ? p.Name : p.Category);

        var res = new List<ProductGrpModel>();


        foreach (var g in groups)
        {
            var p1 = g.First();

            res.Add(new ProductGrpModel
            {
                _Products = g.ToArray(),
                Name = p1.Name,
                Icon = p1.GetItemIcon(),
                Prices = g.Select(p => (p.Price, p.PricesType)).ToArray(),
                Options = g.SelectMany(p => p.Options ?? [])
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToArray(),
                Extras = g.SelectMany(p =>
                        p.ExtraOptions?.Select(e => (e.Price, e.Option)) ?? [])
                    .Distinct()
                    .ToArray()
            });
        }

        return res
            .OrderBy(p => p.Name)
            .ToArray();
    }
}