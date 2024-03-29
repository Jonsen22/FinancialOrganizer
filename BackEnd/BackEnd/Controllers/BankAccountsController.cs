﻿using System;
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
        const string errorMessage = "Error: {ErrorMessage}";
        const string UnexpectedError = "Unexpected Error";

        public BankAccountsController(IBankAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/BankAccounts
        [HttpGet("GetBankAccountsByUser")]
        public async Task<ActionResult<IEnumerable<BankAccountDTO>>> GetBankAccountsByUser()
        {
            try
            {

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == null)
                    return NotFound("User not Found");

                var bankAccounts = await _repository.GetBankAccountsByUser(currentUserId);
            var bankAccountsRetorno = _mapper.Map<IEnumerable<BankAccountDTO>>(bankAccounts);

            return bankAccountsRetorno.Any() ? Ok(bankAccounts) : NotFound("No registers");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostBankAccount(BankAccountAddDTO bankAccountDTO)
        {
            try
            {

            if (bankAccountDTO == null) return BadRequest("Object null");

            string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bankAccountAdd = _mapper.Map<BankAccount>(bankAccountDTO);
            bankAccountAdd.UserId = currentUserId;

            _repository.Add(bankAccountAdd);

            return await _repository.SaveChanges() ? Ok(bankAccountAdd) : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutBankAccount(int bankAccountId, BankAccountAddDTO bankAccountDTO)
        {
            try
            {
                var bankAccount = await _repository.GetBankAccountById(bankAccountId);

                if (bankAccount == null) return NotFound("Bank Account not found");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (bankAccount.UserId != currentUserId)
                    return Unauthorized("Not authorized");

                var bankAccountUpdate = _mapper.Map(bankAccountDTO, bankAccount);
                _repository.Update(bankAccountUpdate);

                return await _repository.SaveChanges() ? Ok(bankAccountUpdate) : BadRequest("Action not possible");         
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }


        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBankAccount(int bankAccountId)
        {
            try
            {

                var bankAccountDelete = await _repository.GetBankAccountById(bankAccountId);

                if (bankAccountDelete == null) return NotFound("Bank Account not found");

                string? currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (bankAccountDelete.UserId != currentUserId)
                    return Unauthorized("Not authorized");

                _repository.Delete(bankAccountDelete);

                return await _repository.SaveChanges() ? Ok("Bank Account deleted") : BadRequest("Action not possible");
            }
            catch (Exception e)
            {
                Log.Error(e, errorMessage, e.Message);
                return (StatusCode(500, UnexpectedError));
            }


        }
    }
}
