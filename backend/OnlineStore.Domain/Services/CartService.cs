﻿using System.Reflection.Metadata;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Services;

public class CartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public virtual async Task<Cart> GetCartForAccount(Guid accountId, CancellationToken cts) =>
        await _unitOfWork.CartRepository.GetByAccountId(accountId, cts);

    public virtual async Task<IReadOnlyCollection<CartItem>> GetItemsInCart(Guid accountId, CancellationToken cts)
    {
        var cart = await _unitOfWork.CartRepository.GetByAccountId(accountId, cts);
        var cartItems = cart.Items;
        return cartItems;
    }
    
    public virtual async Task AddItem(Guid accountId, Guid productId, CancellationToken cts, int quantity = 1)
    {
        var cart = await _unitOfWork.CartRepository.GetByAccountId(accountId, cts);
        var product = await _unitOfWork.ProductRepository.GetById(productId, cts);
        cart.Add(product, quantity);
        await _unitOfWork.CartRepository.Update(cart, cts);
        await _unitOfWork.SaveChangesAsync(cts);
    }
    
    // public virtual async Task<CartItem> DeleteItem(Guid id, CancellationToken cts)
    // {
    //     
    // }
}