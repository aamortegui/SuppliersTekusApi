﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using System.Linq.Dynamic.Core;
using Tekus.Suppliers.WebApi.Domain.Common;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceSuppliersDBContext _context;
        private readonly string[] _allowedOrderServiceFields;


        public ServiceRepository(ServiceSuppliersDBContext context, IConfiguration config)
        {
            _context = context;
            _allowedOrderServiceFields = config
            .GetSection("OrderSettings:AllowedOrderServiceFields")
            .Get<string[]>()!;
        }

        public async Task<Response> GetAllServicesAsync(ServiceFilter serviceFilter)
        {
            Response response = new Response();

            try
            {
                var servicesQuery = _context.Services
                    .Include(x => x.ServiceCountries)
                    .Include(x => x.SupplierServices)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(serviceFilter.Name))
                {
                    servicesQuery = servicesQuery.Where(x => x.Name.Contains(serviceFilter.Name));
                }
                if (serviceFilter.ServiceId != Guid.Empty)
                {
                    servicesQuery = servicesQuery.Where(x => x.Id == serviceFilter.ServiceId);
                }
                if (serviceFilter.Price != null && serviceFilter.Price > 0)
                {
                    servicesQuery = servicesQuery.Where(x => x.PriceHour == serviceFilter.Price);
                }

                string? upperOrderBy = serviceFilter.OrderBy?.ToUpper();
                if (!string.IsNullOrEmpty(serviceFilter.OrderBy) && _allowedOrderServiceFields.Contains(upperOrderBy))
                {
                    var direction = serviceFilter.IsDescending ? "descending" : "ascending";
                    servicesQuery = servicesQuery.OrderBy($"{serviceFilter.OrderBy} {direction}");
                }
                var services = await servicesQuery
                        .Paginate(new Pagination { Page = serviceFilter.Page, RecordsPerPage = serviceFilter.RecordsPerPage })
                        .ToListAsync();

                response.Result = services;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetServiceByIdAsync(Guid id)
        {
            Response response = new Response();
            try
            {
                var service = await _context.Services
                    .Include(x => x.ServiceCountries)
                    .Include(x => x.SupplierServices)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (service == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Service not found";
                }
                response.Result = service;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> CreateServiceAsync(ServiceCreation serviceCreation)
        {
            Response response = new Response();
            var serviceId = Guid.NewGuid();

            var supplierIds = serviceCreation.SupplierServices.Select(s => s.SupplierId).ToList();
            if (!supplierIds.Any())
            {
                response.IsSuccess = false;
                response.Message = "At least one supplier must be provided.";
                return response;
            }

            var countryIds = serviceCreation.ServiceCountries.Select(c => c.CountryId).ToList();
            if (!countryIds.Any())
            {
                response.IsSuccess = false;
                response.Message = "At least one country must be provided.";
                return response;
            }

            //Validate if the service already exists
            var existingService = await _context.Services
                .Where(x => x.Name == serviceCreation.Name)
                .Where(x => x.SupplierServices.Any(ss => supplierIds.Contains(ss.SupplierId)))
                .Where(x => x.ServiceCountries.Any(sc => countryIds.Contains(sc.CountryId)))
                .FirstOrDefaultAsync();

            if (existingService is not null)
            {
                response.IsSuccess = false;
                response.Message = "Service already exists";
                return response;
            }

            //Validate if the supplier exists
            var supplierExists = await _context.Suppliers
                .CountAsync(x => supplierIds.Contains(x.Id)) == supplierIds.Count;

            if (!supplierExists)
            {
                response.IsSuccess = false;
                response.Message = "One or more suppliers not found";
                return response;
            }
            //Validate if the country exists
            var countryExists = await _context.Countries
                .CountAsync(x => countryIds.Contains(x.CountryId)) == countryIds.Count;
            if (!supplierExists)
            {
                response.IsSuccess = false;
                response.Message = "One or more countries not found";
                return response;
            }

            //Create the service
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var serviceEntity = new Service()
                {
                    Id = serviceId,
                    Name = serviceCreation.Name,
                    PriceHour = serviceCreation.PriceHour,
                    SupplierServices = serviceCreation.SupplierServices.Select(ss => new SupplierService
                    {
                        SupplierId = ss.SupplierId,
                        ServiceId = serviceId
                    }).ToList(),
                    ServiceCountries = serviceCreation.ServiceCountries.Select(sc => new ServiceCountry
                    {
                        CountryId = sc.CountryId,
                        ServiceId = serviceId
                    }).ToList()
                };
                await _context.Services.AddAsync(serviceEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.IsSuccess = true;
                response.Result = serviceEntity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task UpdateServiceAsync(Guid id, ServiceCreation serviceCreation)
        {
            var existingService = await _context.Services
                .Include(x => x.ServiceCountries)
                .Include(x => x.SupplierServices)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingService == null)
            {
                throw new Exception("Service not found");
            }
            if (serviceCreation.PriceHour <= 0)
                throw new Exception("PriceHour must be greater than zero");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!string.IsNullOrEmpty(serviceCreation.Name))
                    existingService.Name = serviceCreation.Name;

                existingService.PriceHour = serviceCreation.PriceHour;

                if (serviceCreation.SupplierServices != null && serviceCreation.SupplierServices.Any())
                {
                    var supplierIds = serviceCreation.SupplierServices
                        .Select(ss => ss.SupplierId)
                        .ToList();

                    var validSupplierCount = await _context.Suppliers.CountAsync(s => supplierIds.Contains(s.Id));
                    if (validSupplierCount != supplierIds.Count)
                        throw new Exception("One or more suppliers not found");

                    //Remove existing suppliers that are not in the new list
                    var incomingSupplierIds = serviceCreation.SupplierServices
                        .Select(ss => ss.SupplierId)
                        .ToHashSet();

                    var toRemove = existingService.SupplierServices
                        .Where(ss => !incomingSupplierIds.Contains(ss.SupplierId))
                        .ToList();

                    foreach (var removed in toRemove)
                    {
                        _context.SupplierServices.Remove(removed);
                    }

                    //Add or update suppliers
                    foreach (var incoming in serviceCreation.SupplierServices)
                    {
                        var existing = existingService.SupplierServices
                            .Any(ss => ss.SupplierId == incoming.SupplierId);

                        if (!existing)
                        {
                            existingService.SupplierServices.Add(new SupplierService
                            {
                                SupplierId = incoming.SupplierId,
                                ServiceId = id
                            });
                        }
                    }
                }
                if (serviceCreation.ServiceCountries != null && serviceCreation.ServiceCountries.Any())
                {
                    var countryIds = serviceCreation.ServiceCountries
                        .Select(sc => sc.CountryId)
                        .ToList();

                    var validCountryCount = await _context.Countries.CountAsync(c => countryIds.Contains(c.CountryId));
                    if (validCountryCount != countryIds.Count)
                        throw new Exception("One or more countries not found");

                    //remove existing countries that are not in the new list
                    var incomingCountryIds = serviceCreation.ServiceCountries
                        .Select(sc => sc.CountryId)
                        .ToHashSet();

                    var toRemove = existingService.ServiceCountries
                        .Where(sc => !incomingCountryIds.Contains(sc.CountryId))
                        .ToList();

                    foreach (var removed in toRemove)
                    {
                        _context.ServiceCountries.Remove(removed);
                    }

                    //Add or update countries
                    foreach (var incoming in serviceCreation.ServiceCountries)
                    {
                        var existing = existingService.ServiceCountries
                            .Any(sc => sc.CountryId == incoming.CountryId);

                        if (!existing)
                        {
                            existingService.ServiceCountries.Add(new ServiceCountry
                            {
                                CountryId = incoming.CountryId,
                                ServiceId = id
                            });
                        }
                    }

                }
                var entry = _context.Entry(existingService.SupplierServices.Last());
                Console.WriteLine($"Estado: {entry.State}");

                var entry2 = _context.Entry(existingService.ServiceCountries.Last());
                Console.WriteLine($"Estado2: {entry.State}");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error updating service", ex);
            }
        }
    }
}
