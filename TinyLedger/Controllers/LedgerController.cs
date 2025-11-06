using Application.Dto;
using Application.Services.Interfaces;
using Domain.Model;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TinyLedger.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LedgerController : ControllerBase
    {
        private readonly ILogger<LedgerController> _logger;
        private readonly IBalanceService _balanceService;
        private readonly ITransactionService _transactionService;
        private readonly IToDomainAdapter<Transaction, TransactionRequestDto> _toDomainAdapter;
        private readonly IToDtoAdapter<Balance, BalanceDto> _balanceToDtoAdapter;
        private readonly IToDtoAdapter<Transaction, TransactionResponseDto> _transactionToDtoAdapter;

        public LedgerController(ILogger<LedgerController> logger, IBalanceService balanceService, ITransactionService transactionService, IToDomainAdapter<Transaction, TransactionRequestDto> toDomainAdapter, IToDtoAdapter<Balance, BalanceDto> balanceToDtoAdapter, IToDtoAdapter<Transaction, TransactionResponseDto> transactionToDtoAdapter)
        {
            _logger = logger;
            _balanceService = balanceService;
            _transactionService = transactionService;
            _toDomainAdapter = toDomainAdapter;
            _balanceToDtoAdapter = balanceToDtoAdapter;
            _transactionToDtoAdapter = transactionToDtoAdapter;
        }

        //View current balance
        [HttpGet("{customerId}/balance")]
        [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBalance([FromRoute] int customerId)
        {
            try
            {
                var balance = await _balanceService.GetCurrentBalanceAsync(customerId);
                var balanceDto = _balanceToDtoAdapter.ConvertToDto(balance);

                return Ok(balanceDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest,
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting balance for customer {customerId}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while getting the balance" });
            }
        }

        //View transaction history
        [HttpGet("{customerId}/transactions")]
        [ProducesResponseType(typeof(IEnumerable<TransactionRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions([FromRoute] int customerId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsHistoryAsync(customerId);

                var transactionDtos = transactions.Select(t => _transactionToDtoAdapter.ConvertToDto(t));
                return Ok(transactionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transaction history for customer {customerId}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while getting transaction history" });
            }
        }

        //Ability to record money movements (ie: deposits and withdrawals)
        [HttpPost("transaction")]
        [ProducesResponseType(typeof(TransactionRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostTransaction([FromBody] TransactionRequestDto transaction)
        {
            if (transaction.Amount <= 0)
            {
                return BadRequest(new { message = "Transaction amount must be positive" });
            }

            if (transaction.CustomerId <= 0)
            {
                return BadRequest(new { message = "Customer ID must be positive" });
            }

            try
            {
                var transactionModel = _toDomainAdapter.ConvertToDomainModel(transaction);
                var createdTransaction = await _transactionService.RecordTransactionAsync(transactionModel);
                var createdTransactionDto = _transactionToDtoAdapter.ConvertToDto(createdTransaction);

                return Ok(createdTransactionDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest,
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating transaction for customer {transaction.CustomerId}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while creating the transaction" });
            }
        }
    }
}
