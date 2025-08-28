using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using System;
using Xunit;

namespace Domain.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Order_WithValidProperties_ShouldSetValuesCorrectly()
    {
        // Arrange & Act
        var order = new Order
        {
            OrderNumber = "ORD-2024-001",
            CustomerId = 1L,
            StoreId = 1L,
            Status = OrderStatus.Pending,
            Subtotal = 299.99m,
            TaxAmount = 30.00m,
            ShippingAmount = 15.00m,
            DiscountAmount = 20.00m,
            TotalAmount = 324.99m,
            Currency = "TRY",
            Notes = "Test order notes"
        };

        // Assert
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be("ORD-2024-001");
        order.CustomerId.Should().Be(1L);
        order.StoreId.Should().Be(1L);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Subtotal.Should().Be(299.99m);
        order.TaxAmount.Should().Be(30.00m);
        order.ShippingAmount.Should().Be(15.00m);
        order.DiscountAmount.Should().Be(20.00m);
        order.TotalAmount.Should().Be(324.99m);
        order.Currency.Should().Be("TRY");
        order.Notes.Should().Be("Test order notes");
    }

    [Fact]
    public void Order_WithDefaultValues_ShouldHaveCorrectDefaults()
    {
        // Arrange & Act
        var order = new Order();

        // Assert
        order.OrderNumber.Should().Be(string.Empty);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Currency.Should().Be("TRY");
        order.Subtotal.Should().Be(0m);
        order.TaxAmount.Should().Be(0m);
        order.ShippingAmount.Should().Be(0m);
        order.DiscountAmount.Should().Be(0m);
        order.TotalAmount.Should().Be(0m);
    }

    [Fact]
    public void Order_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var order = new Order();

        // Assert
        order.Items.Should().NotBeNull();
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Order_WithNullValues_ShouldHandleNullValues()
    {
        // Arrange & Act
        var order = new Order
        {
            Notes = null,
            ConfirmedAt = null,
            ShippedAt = null,
            DeliveredAt = null,
            CancelledAt = null,
            Payment = null,
            Shipment = null
        };

        // Assert
        order.Notes.Should().BeNull();
        order.ConfirmedAt.Should().BeNull();
        order.ShippedAt.Should().BeNull();
        order.DeliveredAt.Should().BeNull();
        order.CancelledAt.Should().BeNull();
        order.Payment.Should().BeNull();
        order.Shipment.Should().BeNull();
    }

    [Fact]
    public void Order_WithDifferentStatuses_ShouldHandleAllStatuses()
    {
        // Arrange & Act
        var pendingOrder = new Order { Status = OrderStatus.Pending };
        var confirmedOrder = new Order { Status = OrderStatus.Confirmed };
        var shippedOrder = new Order { Status = OrderStatus.Shipped };
        var deliveredOrder = new Order { Status = OrderStatus.Delivered };
        var cancelledOrder = new Order { Status = OrderStatus.Cancelled };

        // Assert
        pendingOrder.Status.Should().Be(OrderStatus.Pending);
        confirmedOrder.Status.Should().Be(OrderStatus.Confirmed);
        shippedOrder.Status.Should().Be(OrderStatus.Shipped);
        deliveredOrder.Status.Should().Be(OrderStatus.Delivered);
        cancelledOrder.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Order_WithHighAmounts_ShouldHandleLargeValues()
    {
        // Arrange
        var highAmount = decimal.MaxValue;

        // Act
        var order = new Order
        {
            Subtotal = highAmount,
            TaxAmount = highAmount,
            ShippingAmount = highAmount,
            DiscountAmount = highAmount,
            TotalAmount = highAmount
        };

        // Assert
        order.Subtotal.Should().Be(highAmount);
        order.TaxAmount.Should().Be(highAmount);
        order.ShippingAmount.Should().Be(highAmount);
        order.DiscountAmount.Should().Be(highAmount);
        order.TotalAmount.Should().Be(highAmount);
    }

    [Fact]
    public void Order_WithNegativeAmounts_ShouldHandleNegativeValues()
    {
        // Arrange
        var negativeAmount = -100.00m;

        // Act
        var order = new Order
        {
            DiscountAmount = negativeAmount
        };

        // Assert
        order.DiscountAmount.Should().Be(negativeAmount);
    }

    [Fact]
    public void Order_WithDateTimeValues_ShouldHandleDateTimeProperties()
    {
        // Arrange
        var testDate = DateTime.UtcNow;

        // Act
        var order = new Order
        {
            ConfirmedAt = testDate,
            ShippedAt = testDate,
            DeliveredAt = testDate,
            CancelledAt = testDate
        };

        // Assert
        order.ConfirmedAt.Should().Be(testDate);
        order.ShippedAt.Should().Be(testDate);
        order.DeliveredAt.Should().Be(testDate);
        order.CancelledAt.Should().Be(testDate);
    }

    [Fact]
    public void Order_WithSpecialCharacters_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var specialOrderNumber = "ORD-2024-001-Özel";
        var specialNotes = "Notlar: Türkçe karakterler, özel semboller @#$%^&*()";

        // Act
        var order = new Order
        {
            OrderNumber = specialOrderNumber,
            Notes = specialNotes
        };

        // Assert
        order.OrderNumber.Should().Be(specialOrderNumber);
        order.Notes.Should().Be(specialNotes);
    }
}
