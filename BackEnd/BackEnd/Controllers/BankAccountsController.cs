using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BackEndTest.DTOs;
using BackEndTest.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organizer.Context;
using Organizer.Models;
using Serilog;

namespace BackEndTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountRepository _repository;
        private readonly IMapper _mapper;

        public BankAccountsController(IBankAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/BankAccounts
        [HttpGet("GetBankAccountsByUser")]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccountsByUser()
        {
            try
            {

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bankAccounts = await _repository.GetBankAccountsByUser(currentUserId);
            var bankAccountsRetorno = _mapper.Map<IEnumerable<BankAccountDTO>>(bankAccounts);

            return bankAccountsRetorno.Any() ? Ok(bankAccountsRetorno) : BadRequest("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostBankAccount(BankAccountAddDTO bankAccountDTO)
        {
            try
            {

            if (bankAccountDTO == null) return BadRequest("unregistered bank accounts");

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bankAccountAdd = _mapper.Map<BankAccount>(bankAccountDTO);
            bankAccountAdd.UserId = currentUserId;

            _repository.Add(bankAccountAdd);

            var response = _mapper.Map<BankAccountDTO>(bankAccountAdd);

            return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutBankAccount(int bankAccountId, BankAccountAddDTO bankAccountDTO)
        {
            try
            {
                var bankAccount = await _repository.GetBankAccountById(bankAccountId);

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (bankAccount.UserId != currentUserId)
                    Unauthorized("Not authorized");

                var bankAccountUpdate = _mapper.Map(bankAccountDTO, bankAccount);
                _repository.Update(bankAccountUpdate);
                var response = _mapper.Map<BankAccountDTO>(bankAccountUpdate);
                return await _repository.SaveChanges() ? Ok(response) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }


        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBankAccount(int bankAccountId)
        {
            try
            {

                var bankAccountDelete = await _repository.GetBankAccountById(bankAccountId);

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (bankAccountDelete.UserId != currentUserId)
                    Unauthorized("Not authorized");

                _repository.Delete(bankAccountDelete);

                return await _repository.SaveChanges() ? Ok("Bank Account deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred: {ErrorMessage}", e.Message);
                return (StatusCode(500, "Unexpected Error"));
            }


        }
    }
}
