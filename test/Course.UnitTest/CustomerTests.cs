using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTest;

public class CustomerTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateCustomer()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Customer customer = new(id, "John Doe", "john.doe@bch.hn");


        Assert.Equal(id, customer.Id);
        Assert.NotEqual(Guid.Empty, customer.Id);
        Assert.Equal("John Doe", customer.FullName);
        Assert.Equal("john.doe@bch.hn", customer.Email);
    }

    [Fact]
    public void Constructor_WhenEmailIsInvalid_ShouldThrowDomainException()
    {
        // Arrange Act
        
        var act = () => new Customer(Guid.NewGuid(), "John Doe", "JohnDoe");

        
        act.Should().Throw<DomainException>().WithMessage("El cliente requiere un correo valido.");

    }



}
