﻿
namespace Transactions.Application
{
    using BankAccount.Domain.Entity;
    using AutoMapper;
    using Common.Application;
    using Common.Application.Enumeration;
    using Common.infrastructure.repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Transactions.Application.Dto;
    using Transactions.Domain.Service;
    using Transactions.Infraestructure;
    using Account.Application.Dto;

    public interface ITransactionApplicationService
    {
         void performCreate(RequestBankTransferDto requestBankTransferDto);
        //void create( BankAccountDto bankAccountDto);
    }
}