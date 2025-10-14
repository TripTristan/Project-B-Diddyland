using System;
using System.Collections.Generic;
using System.Linq;

public class OrderLogic
{
    private readonly MenuLogic _menuLogic;
    private readonly List<CartLine> _cart = new();

    public OrderLogic(MenuLogic menuLogic)
    {
        _menuLogic = menuLogic;
    }

    public IEnumerable<MenuModel> GetAllMenuItems() => _menuLogic.GetAll();
    public IReadOnlyList<CartLine> GetCart() => _cart;
}

public class CartLine { }
