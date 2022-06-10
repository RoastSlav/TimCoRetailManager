using Caliburn.Micro;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Windows;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels;

public class SalesViewModel : Screen
{
    private readonly IProductEndpoint _productEndpoint;
    private readonly IConfiguration _config;
    private readonly ISaleEndpoint _saleEndpoint;
    private readonly IMapper _mapper;
    private readonly StatusInfoViewModel _status;
    private readonly IWindowManager _window;

    public SalesViewModel(IProductEndpoint productEndpoint, IConfiguration config, 
        ISaleEndpoint saleEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
    {
        _productEndpoint = productEndpoint;
        _config = config;
        _saleEndpoint = saleEndpoint;
        _mapper = mapper;
        _status = status;
        _window = window;
    }

    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        try
        {
            await LoadProducts();
        }
        catch (Exception ex)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.Title = "System Error";

            if (ex.Message == "Unauthorized")
            {
                _status.UpdateMessage("Unauthorized Access",
                    "You do not have permission to interact with the Sales Form.");
                await  _window.ShowDialogAsync(_status, null, settings);
            }
            else
            {
                _status.UpdateMessage("Fatal Exception", ex.Message);
                await _window.ShowDialogAsync(_status, null, settings);
            }

            await TryCloseAsync();
        }
    }

    private BindingList<ProductDisplayModel> _products;

    public BindingList<ProductDisplayModel> Products
    {
        get { return _products; }
        set 
        { 
            _products = value; 
            NotifyOfPropertyChange(() => Products);
        }
    }

    private async Task LoadProducts()
    {
        var productsList = await _productEndpoint.GetAll();
        var products = _mapper.Map<List<ProductDisplayModel>>(productsList);
        Products = new(products);
    }

    private async Task ResetSalesViewModel()
    {
        Cart = new();
        await LoadProducts();

        NotifyOfPropertyChange(() => SubTotal);
        NotifyOfPropertyChange(() => Total);
        NotifyOfPropertyChange(() => Tax);
        NotifyOfPropertyChange(() => CanCheckOut);
    }

    private ProductDisplayModel _selectedProduct;

    public ProductDisplayModel SelectedProduct
    {
        get { return _selectedProduct; }
        set 
        { 
            _selectedProduct = value;
            NotifyOfPropertyChange(() => SelectedProduct);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
    }

    private CartItemDisplayModel _selectedCartItem;
    public CartItemDisplayModel SelectedCartItem
    {
        get { return _selectedCartItem; }
        set
        {
            _selectedCartItem = value;
            NotifyOfPropertyChange(() => SelectedCartItem);
            NotifyOfPropertyChange(() => CanRemoveFromCart);
        }
    }


    private BindingList<CartItemDisplayModel> _cart = new();

    public BindingList<CartItemDisplayModel> Cart
    {
        get { return _cart; }
        set 
        { 
            _cart = value; 
            NotifyOfPropertyChange(() => Cart);
        }
    }

    public string SubTotal
    {           
        get 
        {
            return CalculateSubTotal().ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }

    private decimal CalculateSubTotal()
    {
        decimal subTotal = 0;

        foreach (var item in Cart)
        {
            subTotal += item.Product.RetailPrice * item.QuantityInCart;
        }

        return subTotal;
    }

    private decimal CalculateTax()
    {
        decimal taxRate = decimal.Parse(_config.GetValue<string>("taxRate")) / 100;
        decimal taxAmount = 0;

        taxAmount = Cart
            .Where(item => item.Product.IsTaxable)
            .Sum(item => (item.Product.RetailPrice * item.QuantityInCart * taxRate));

        return taxAmount;
    }

    public string Total
    {
        get
        {
            decimal total = CalculateSubTotal() + CalculateTax();
            return total.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }

    public string Tax
    {
        get
        { 
            return CalculateTax().ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }


    private int _itemQuantity = 1;

    public int ItemQuantity
    {
        get { return _itemQuantity; }
        set 
        { 
            _itemQuantity = value; 
            NotifyOfPropertyChange(() => ItemQuantity);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
    }

    public bool CanAddToCart
    {
        get
        {
            //Checks if an item is selected and if it has enough of it to add to the cart.
            bool output = ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity;

            return output;
        }
    }

    public void AddToCart()
    {
        CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            
        if(existingItem != null)
        {
            existingItem.QuantityInCart += ItemQuantity;
        }
        else
        {
            CartItemDisplayModel item = new()
            {
                Product = SelectedProduct,
                QuantityInCart = ItemQuantity
            };
            Cart.Add(item);
        }  
            
        SelectedProduct.QuantityInStock -= ItemQuantity;
        ItemQuantity = 1;
        NotifyOfPropertyChange(() => SubTotal);
        NotifyOfPropertyChange(() => Total);
        NotifyOfPropertyChange(() => Tax);
        NotifyOfPropertyChange(() => CanCheckOut);
    }

    public bool CanRemoveFromCart
    {
        get
        {
            //Checks if an item is selected and that it has enough of it to be removed
            bool output = SelectedCartItem is {QuantityInCart: > 0};

            return output;
        }
    }

    public void RemoveFromCart()
    {
        SelectedCartItem.Product.QuantityInStock += 1;
        if (SelectedCartItem.QuantityInCart > 1)
        {
            SelectedCartItem.QuantityInCart -= 1;
        }
        else
        {
            Cart.Remove(SelectedCartItem);
        }

        NotifyOfPropertyChange(() => SubTotal);
        NotifyOfPropertyChange(() => Total);
        NotifyOfPropertyChange(() => Tax);
        NotifyOfPropertyChange(() => CanCheckOut);
        NotifyOfPropertyChange(() => CanAddToCart);
    }

    public bool CanCheckOut
    {
        get
        {
            bool output = Cart.Count > 0;

            return output;
        }
    }

    public async Task CheckOut()
    {
        SaleModel sale = new();

        foreach (var item in Cart)
        {
            sale.SaleDetails.Add((new()
            {
                ProductId = item.Product.Id,
                Quantity = item.QuantityInCart
                    
            }));
        }
           
        await _saleEndpoint.PostSale(sale);
        await ResetSalesViewModel();
    }
}