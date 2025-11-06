using Application.Dto;
using Application.Services.Interfaces;
using Domain.Model;
using Domain.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TinyLedger.Api.Controllers;

namespace LedgerApi.Tests
{
    public class LedgerControllerTests
    {
        private readonly Mock<ILogger<LedgerController>> _loggerMock;
        private readonly Mock<IBalanceService> _balanceServiceMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<IToDomainAdapter<Transaction, TransactionRequestDto>> _balanceAdapterMock;
        private readonly Mock<IToDtoAdapter<Balance, BalanceDto>> _balanceToDtoAdapterMock;
        private readonly Mock<IToDtoAdapter<Transaction, TransactionResponseDto>> _transactionToDtoAdapterMock;

        private readonly LedgerController _controller;

        public LedgerControllerTests()
        {
            _loggerMock = new Mock<ILogger<LedgerController>>();
            _balanceServiceMock = new Mock<IBalanceService>();
            _transactionServiceMock = new Mock<ITransactionService>();
            _balanceAdapterMock = new Mock<IToDomainAdapter<Transaction, TransactionRequestDto>>();
            _balanceToDtoAdapterMock = new Mock<IToDtoAdapter<Balance, BalanceDto>>();
            _transactionToDtoAdapterMock = new Mock<IToDtoAdapter<Transaction, TransactionResponseDto>>();
            _controller = new LedgerController(_loggerMock.Object, _balanceServiceMock.Object, _transactionServiceMock.Object, _balanceAdapterMock.Object, _balanceToDtoAdapterMock.Object, _transactionToDtoAdapterMock.Object);
        }

        [Fact]
        public async Task GetBalance_ShouldReturnOkWithBalance_WhenCustomerExists()
        {
            //Arrange
            var customerId = 1;
            var balance = new Balance(Guid.NewGuid(), customerId, 100.00m, DateTime.UtcNow);

            var expectedBalance = new BalanceDto
            {
                CustomerId = balance.CustomerId,
                Amount = balance.Amount,
                ModifiedDate = balance.ModifiedDate
            };

            _balanceServiceMock.Setup(s => s.GetCurrentBalanceAsync(customerId))
                .ReturnsAsync(balance);

            _balanceToDtoAdapterMock.Setup(a => a.ConvertToDto(It.IsAny<Balance>()))
                .Returns(expectedBalance);

            //Act
            var result = await _controller.GetBalance(customerId);

            //Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(expectedBalance);

            _balanceToDtoAdapterMock.Verify(a => a.ConvertToDto(It.IsAny<Balance>()), Times.Once);
        }

        [Fact]
        public async Task GetBalance_ShouldThrowException_WhenSomeErrorOcurred()
        {
            //Arrange
            var customerId = 1;
            _balanceServiceMock.Setup(s => s.GetCurrentBalanceAsync(customerId))
                .ThrowsAsync(new Exception("Some error occurred"));

            //Act
            var result = await _controller.GetBalance(customerId);

            //Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!
                        .Contains($"Error getting balance for customer {customerId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTransactions_ShouldReturnOkWithTransactions_WhenCustomerExists()
        {
            //Arrange
            var customerId = 1;
            var transactions = new List<Transaction>
            {
                new Transaction(customerId, 50.00m, DateTime.UtcNow.Date, TransactionType.Deposit),
                new Transaction(customerId, 20.00m, DateTime.UtcNow.Date, TransactionType.Withdrawal)
            };
            transactions[0].SetTransactionId(Guid.NewGuid());
            transactions[1].SetTransactionId(Guid.NewGuid());

            var expectedTransactions = new List<TransactionResponseDto>
            {
                new TransactionResponseDto
                {
                    TransactionId = transactions[0].TransactionId,
                    CustomerId = transactions[0].CustomerId,
                    Amount = transactions[0].Amount,
                    TransactionDate = transactions[0].TransactionDate,
                    TransactionType = transactions[0].TransactionType.ToString()
                },

                new TransactionResponseDto
                {
                    TransactionId = transactions[1].TransactionId,
                    CustomerId = transactions[1].CustomerId,
                    Amount = transactions[1].Amount,
                    TransactionDate = transactions[1].TransactionDate,
                    TransactionType = transactions[1].TransactionType.ToString()
                }
            };

            _transactionServiceMock.Setup(s => s.GetTransactionsHistoryAsync(customerId)).ReturnsAsync(transactions);

            _transactionToDtoAdapterMock.Setup(a => a.ConvertToDto(It.IsAny<Transaction>()))
                .Returns((Transaction t) => expectedTransactions.First(dto => dto.TransactionId == t.TransactionId));

            //Act
            var result = await _controller.GetTransactions(customerId);

            //Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(expectedTransactions);

            _transactionToDtoAdapterMock.Verify(a => a.ConvertToDto(It.IsAny<Transaction>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetTransactions_ShouldReturn500_WhenExceptionIsThrown()
        {
            // Arrange
            int customerId = 1;

            _transactionServiceMock
                .Setup(s => s.GetTransactionsHistoryAsync(customerId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetTransactions(customerId);

            //Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!
                        .Contains($"Error retrieving transaction history for customer {customerId}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task PostTransaction_ShouldReturnOkWithTransactionResponse_WhenTransactionIsSuccessful()
        {
            // Arrange
            var transactionRequestDto = new TransactionRequestDto
            {
                CustomerId = 1,
                Amount = 100.00m,
                TransactionType = "Deposit"
            };

            var transactionDomainModel = new Transaction(transactionRequestDto.CustomerId, transactionRequestDto.Amount, DateTime.UtcNow, TransactionType.Deposit);
            transactionDomainModel.SetTransactionId(Guid.NewGuid());

            var expectedTransactionResponseDto = new TransactionResponseDto
            {
                TransactionId = transactionDomainModel.TransactionId,
                CustomerId = transactionDomainModel.CustomerId,
                Amount = transactionDomainModel.Amount,
                TransactionDate = transactionDomainModel.TransactionDate,
                TransactionType = transactionDomainModel.TransactionType.ToString()
            };

            _balanceAdapterMock.Setup(a => a.ConvertToDomainModel(It.IsAny<TransactionRequestDto>()))
                .Returns(transactionDomainModel);

            _transactionServiceMock.Setup(s => s.RecordTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(transactionDomainModel);

            _transactionToDtoAdapterMock.Setup(a => a.ConvertToDto(It.IsAny<Transaction>()))
                .Returns(expectedTransactionResponseDto);

            // Act
            var result = await _controller.PostTransaction(transactionRequestDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(expectedTransactionResponseDto);
            _balanceAdapterMock.Verify(a => a.ConvertToDomainModel(It.IsAny<TransactionRequestDto>()), Times.Once);
            _transactionToDtoAdapterMock.Verify(a => a.ConvertToDto(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task PostTransaction_ShouldReturn400_WhenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var transactionRequestDto = new TransactionRequestDto
            {
                CustomerId = 1,
                Amount = 200.00m,
                TransactionType = "Withdrawal"
            };

            var transactionDomainModel = new Transaction(transactionRequestDto.CustomerId, transactionRequestDto.Amount, DateTime.UtcNow, TransactionType.Withdrawal);

            _balanceAdapterMock.Setup(a => a.ConvertToDomainModel(It.IsAny<TransactionRequestDto>()))
                .Returns(transactionDomainModel);

            _transactionServiceMock.Setup(s => s.RecordTransactionAsync(It.IsAny<Transaction>()))
                .ThrowsAsync(new InvalidOperationException("Insufficient funds"));

            // Act
            var result = await _controller.PostTransaction(transactionRequestDto);

            // Assert
            var objectResult = result as ObjectResult;
            objectResult.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _balanceAdapterMock.Verify(a => a.ConvertToDomainModel(It.IsAny<TransactionRequestDto>()), Times.Once);
        }
    }
}