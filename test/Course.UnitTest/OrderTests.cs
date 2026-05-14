using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Domain.Entities;
using Course.Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.UnitTest;

public class OrderTests
{
    private readonly Mock<ICustomerRepository> _customers = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IPaymentGateway> _payments= new();

    private readonly OrderService _service;
    public OrderTests()
    {
        _service = new OrderService(
            _customers.Object,
            _products.Object,
            _orders.Object,
            _payments.Object);
    }

    [Fact]
    public async Task Create_WhenOrderIsValid_ShouldCreateOrder() { 

        // Arrange 
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var itemsRequest = new List<CreateOrderItemRequest>();
        itemsRequest.Add(new CreateOrderItemRequest ( productId, 2 ));

        var request = new CreateOrderRequest(customerId, itemsRequest);

        var customer = new Customer(customerId, "John Doe", "john.doe@bch.hn");
        var product = new Product(productId, "Laptop", 1000, 10);
        //var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 2000, PaymentStatus.Approved, "");

        _customers.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _products.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _payments.Setup(x => x.PayAsync(It.IsAny<Guid>(),2000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid orderId, decimal amount, CancellationToken _) => 
            new Payment(Guid.NewGuid(), orderId, amount, PaymentStatus.Approved, "")
            );

        

        // Act

        var response = await _service.CreateAsync(request);

        // Assert
        
        response.Should().NotBeNull();
        response.CustomerId.Should().Be(customerId);
        response.Total.Should().Be(2000);

        //response.Items.Should().HaveCount(1);
        //response.Items[0].ProductId.Should().Be(productId);
        //response.Items[0].Quantity.Should().Be(2);

        _orders.Verify(x => x.AddAsync
        (
            It.Is<Order>(o => o.CustomerId == customerId &&
            o.Total == 2000 &&
            o.Status == OrderStatus.Paid &&
            o.Payment != null &&
            o.Payment.OrderId == o.Id),
            It.IsAny<CancellationToken>()), Times.Once
        );

    }
}
