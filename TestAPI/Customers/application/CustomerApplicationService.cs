namespace Customer.Application
{

    using Customer.Domain.Repository;
    using Customer.Domain.Entity;
    using Common.Application;
    using Customer.Application.Dto;
    using Common.Application.Enumeration;
    using System;
    using Common.Infrastructure.Repository;
    using AutoMapper;
    using System.Collections.Generic;
    using Customers.Domain.Service;
    using System.Linq;
    using Common.Application.Dto;

    public class CustomerApplicationService : ICustomerApplicationService
    {
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _mapper;
        private readonly CustomerDomainService customerDomainService;

        #region public methods

        public CustomerApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            customerDomainService = new CustomerDomainService();
            _iUnitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public CustomerDto getById(long CustomerId)
        {
            var customer = _iUnitOfWork.Customers.GetById(CustomerId);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }

        public long create(CustomerDto customerDto)
        {
            Notification notification = this.validation(customerDto);
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            Customer customer = _mapper.Map<Customer>(customerDto);

            notification = customer.validateSaveCustomer();
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            Customer findCustomer = new Customer();
            findCustomer = _iUnitOfWork.Customers.findByOtherDocumentNumber(customer.DocumentNumber, customer.Id);
            this.customerDomainService.validDoesntExistDocumentNumber(findCustomer);

            findCustomer = _iUnitOfWork.Customers.findByOtherUserName(customer.User, customer.Id);
            this.customerDomainService.validDoesntExistUserCustomer(findCustomer);

            customer.IsActive = true;

            customer.Password = Common.Hash.getHash(customer.Password);

            _iUnitOfWork.Customers.Add(customer);
            _iUnitOfWork.Complete();
            return customer.Id;
        }

        public void deleted(long CustomerId)
        {
            Notification notification = this.validationCustomerId(CustomerId);
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            Customer findCustomer = new Customer(); //_iUnitOfWork.BankAccounts.GetById(Id);
            //this.bankAccountDomainService.validExistBankAccount(findBankAccount);

            _iUnitOfWork.Customers.delete(CustomerId);
            _iUnitOfWork.Complete();
        }

        public void update(CustomerDto customerDto, long CustomerId)
        {
            Notification notification = this.validation(customerDto);
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }
            Customer customer = _mapper.Map<Customer>(customerDto);
            customer.Id = CustomerId;

            validUpdate(customer);

            _iUnitOfWork.Customers.Update(customer);
            _iUnitOfWork.Complete();
        }

        public GridDto getAll(int offset, int limit, string orderBy, string orderDirection)
        {
            List<Customer> customers = _iUnitOfWork.Customers.GetAllWithPaginated(offset, limit, orderBy , orderDirection).ToList();
            GridDto result = new GridDto
            {
                Content = customers,
                TotalRecords = _iUnitOfWork.Customers.CountTotalRecords(),
                CurrentPage = offset,
                PageSize = limit
            };
            return result;
        }

        public CustomerDto findByDocumentNumber(string documentNumber)
        {
            var customer = _iUnitOfWork.Customers.findByDocumentNumber(documentNumber);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }

        #endregion


        #region private methods

        private Notification validUpdate(Customer customer)
        {
            Notification notification = this.validationCustomerId(customer.Id);
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            notification = this.validationIsNull(customer);
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            Customer findCustomer = new Customer(); // _iUnitOfWork.Customers.GetById( customer.Id);            
            this.customerDomainService.validExistCustomer(findCustomer);

            notification = customer.validateSaveCustomer();
            if (notification.hasErrors())
            {
                throw new ArgumentException(notification.errorMessage());
            }

            findCustomer = _iUnitOfWork.Customers.findByOtherDocumentNumber(customer.DocumentNumber, customer.Id);
            this.customerDomainService.validDoesntExistDocumentNumber(findCustomer);

            findCustomer = _iUnitOfWork.Customers.findByOtherUserName(customer.User, customer.Id);
            this.customerDomainService.validDoesntExistUserCustomer(findCustomer);

            return notification;
        }

        private Notification validationCustomerId(long CustomerId)
        {
            Notification notification = new Notification();
            if (CustomerId == 0)
            {
                notification.addError("Id Customer is Required.");
            }
            return notification;
        }

        private Notification validationIsNull(Customer customer)
        {
            Notification notification = new Notification();
            if (customer == null)
            {
                notification.addError("Invalid JSON data in request body.");
            }
            return notification;
        }

        private Notification validation(CustomerDto customerDto)
        {
            Notification notification = new Notification();
            if (customerDto == null || customerDto.requestBodyType == RequestBodyType.Invalid)
            {
                notification.addError("Invalid JSON data in request body.");
            }
            return notification;
        }

        #endregion


    }
}

