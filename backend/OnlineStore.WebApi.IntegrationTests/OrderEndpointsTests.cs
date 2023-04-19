﻿using Bogus;
using FluentAssertions;
using OnlineStore.HttpApiClient;
using OnlineStore.Models.Requests;
using Xunit.Abstractions;

namespace OnlineStore.WebApi.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Faker _faker = new("ru");

    public OrderEndpointsTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Creating_New_Order_happens()
    {
        // Arrange
        var registerRequest = new RegisterRequest()
        {
            Email = _faker.Person.Email,
            Name = _faker.Person.UserName,
            Password = _faker.Internet.Password()
        };
        var httpClient = _factory.CreateClient();
        var client = new ShopClient(httpClient: httpClient);
        var registerResponse = await client.Register(registerRequest);
        var accountId = registerResponse.AccountId;
        var address = _faker.Person.Address;
        var items = new List<OrderItemRequest>()
        {
            new()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1,
                Price    = 50
            }
        };

        var orderRequest = new PlaceOrderRequest
        {
            OrderId = Guid.NewGuid(),
            AccountId = accountId,
            OrderDate = DateTimeOffset.Now,
            Address = address.Suite + ", " + address.Street,
            City = address.City,
            Items = items
        };

        // Act
        var order = await client.PlaceOrder(orderRequest.AccountId);
        _testOutputHelper.WriteLine("ORDER = " + order);
        
        // Assert
        order.Should().NotBeNull();
        order.AccountId.Should().Be(accountId);
        order.Items.Should().NotBeNull().And.HaveCountGreaterThan(0);
        order.Items.Should().HaveCount(1);
    }
}