using AutoMapper;
using BackEndTest.DTOs;
using BackEndTest.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organizer.Models;
using Serilog;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackEndTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("GetTransactionsByUser")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByUser()
        {
            try
            {

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var transactions = await _repository.GetTransactionsByUser(currentUserId);
                var transactionsRetorno = _mapper.Map<IEnumerable<TransactionDTO>>(transactions);

                return transactionsRetorno.Any() ? Ok(transactionsRetorno) : BadRequest("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }

        }

        [HttpGet("GetTransactionsByDate")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByDate(DateTime InitialDate, DateTime FinalDate)
        {
            try
            {
                if ((int)((FinalDate - InitialDate).TotalDays / 365.25) > 2)
                    return BadRequest("The date is bigger than 2 years");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var transactions = await _repository.GetTransactionsByDate(InitialDate, FinalDate, currentUserId);
                var transactionsRetorno = _mapper.Map<IEnumerable<TransactionDTO>>(transactions);

                return transactionsRetorno.Any() ? Ok(transactionsRetorno) : BadRequest("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostTransaction(TransactionAddDTO transactionDTO)
        {
            try
            {

                if (transactionDTO == null) return BadRequest("unregistered bank accounts");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transactionAdd = _mapper.Map<Transaction>(transactionDTO);
                transactionAdd.UserId = currentUserId;

                _repository.Add(transactionAdd);

                var response = _mapper.Map<TransactionDTO>(transactionAdd);

                return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }

        }
        [HttpPut]
        public async Task<IActionResult> PutTransaction(int transactionId, TransactionAddDTO transactionDTO)
        {
            try
            {
                var transaction = await _repository.GetTransactionById(transactionId);

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (transaction.UserId != currentUserId)
                    Unauthorized("Not authorized");

                var transactionUpdate = _mapper.Map(transactionDTO, transaction);
                _repository.Update(transactionUpdate);
                var response = _mapper.Map<BankAccountDTO>(transactionUpdate);
                return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            try
            {

                var transactionDelete = await _repository.GetTransactionById(transactionId);
                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (transactionDelete.UserId != currentUserId)
                    Unauthorized("Not authorized");

                _repository.Delete(transactionDelete);

                return await _repository.SaveChanges() ? Ok("Transaction Deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }
    }
}
