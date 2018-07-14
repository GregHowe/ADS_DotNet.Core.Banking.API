namespace Security.application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Infrastructure.Repository;
    using Customer.Application.Dto;
    using Customers.Domain.Service;
    using Customer.Domain.Entity;


    public class SecurityApplicationService : ISecurityApplicationService
    {
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _mapper;
        private readonly CustomerDomainService customerDomainService;

        public SecurityApplicationService(IUnitOfWork iUnitOfWork, IMapper mapper)
        {   
            customerDomainService = new CustomerDomainService();
            _iUnitOfWork = iUnitOfWork;
            _mapper = mapper;
        }

        public CustomerDto login(CustomerDto customerDto)
        {
            var customerParam = _mapper.Map<Customer>(customerDto);
            customerParam.Password = Common.Hash.getHash(customerDto.Password);
            var findCustomer = _iUnitOfWork.Customers.login(customerParam);

            this.customerDomainService.validExistCustomerLogged(findCustomer);

            CustomerDto customerDtoResult= _mapper.Map<CustomerDto>(findCustomer);
            return customerDtoResult;
        }
    }
}
