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
        const string errorMessage = "Error: {ErrorMessage}";
        const string UnexpectedError = "Unexpected Error";

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

                if (currentUserId == null)
                    return NotFound("User not Found");

                var transactions = await _repository.GetTransactionsByUser(currentUserId);
               

                return transactions.Any() ? Ok(transactions) : NotFound("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
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

                if (currentUserId == null)
                    return NotFound("User not Found");

                var transactions = await _repository.GetTransactionsByDate(InitialDate, FinalDate, currentUserId);
               

                return transactions.Any() ? Ok(transactions) : BadRequest("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostTransaction(TransactionAddDTO transactionDTO)
        {
            try
            {

                if (transactionDTO == null) return BadRequest("object null");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transactionAdd = _mapper.Map<Transaction>(transactionDTO);
                transactionAdd.UserId = currentUserId;

                _repository.Add(transactionAdd);


                return await _repository.SaveChanges() ? Ok(transactionAdd) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }

        }
        [HttpPut]
        public async Task<IActionResult> PutTransaction(int transactionId, TransactionAddDTO transactionDTO)
        {
            try
            {
                var transaction = await _repository.GetTransactionById(transactionId);

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (transaction == null) return NotFound("Transaction not found");

                if (transaction.UserId != currentUserId)
                    return Unauthorized("Not authorized");

                var transactionUpdate = _mapper.Map(transactionDTO, transaction);
                _repository.Update(transactionUpdate);
         
                return await _repository.SaveChanges() ? Ok(transactionUpdate) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            try
            {

                var transactionDelete = await _repository.GetTransactionById(transactionId);
                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (transactionDelete == null) return NotFound("Transaction not found");

                if (transactionDelete.UserId != currentUserId)
                   return Unauthorized("Not authorized");

                _repository.Delete(transactionDelete);

                return await _repository.SaveChanges() ? Ok("Transaction Deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }
    }
}
