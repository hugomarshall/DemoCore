﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DemoCore.Application.Interfaces;
using DemoCore.Application.ViewModels;
using DemoCore.Domain.Commands;
using DemoCore.Domain.Core.Bus;
using DemoCore.Domain.Interfaces;
using DemoCore.Infra.Data.Repositories.EventSourcing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DemoCore.Application.Services
{
    public class WorkAvailabilityService : IWorkAvailabilityService
    {
        private readonly IMapper mapper;
        private readonly IMediatorHandler bus;
        private readonly IEventStoreRepository eventStoreRepository;
        private readonly IWorkAvailabilityRepository waRepository;

        public WorkAvailabilityService(IMapper mapper, IMediatorHandler bus, IEventStoreRepository eventStoreRepository,
            IWorkAvailabilityRepository waRepository)
        {
            this.mapper = mapper;
            this.bus = bus;
            this.eventStoreRepository = eventStoreRepository;
            this.waRepository = waRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public IEnumerable<WorkAvailabilityVM> GetAll()
        {
            return waRepository.GetAll().ProjectTo<WorkAvailabilityVM>(mapper.ConfigurationProvider);
        }

        public WorkAvailabilityVM GetById(int id)
        {
            return mapper.Map<WorkAvailabilityVM>(waRepository.GetById(id));
        }

        public IEnumerable<SelectedItems> GetSelectedWorkAvailability()
        {
            var wa = waRepository.GetAll().ProjectTo<WorkAvailabilityVM>(mapper.ConfigurationProvider);
            var response = new Collection<SelectedItems>();
            foreach (var item in wa)
            {
                response.Add(
                    new SelectedItems
                    {
                        Id = item.Id,
                        DescriptionEN = item.DescriptionEN,
                        DescriptionPT = item.DescriptionPT,
                        Assigned = false,
                    }
                );
            }
            return response;
        }

        public void Register(WorkAvailabilityVM request)
        {
            var registerCommand = mapper.Map<RegisterNewWorkAvailabilityCommand>(request);
            bus.SendCommand(registerCommand);
        }

        public void Remove(int id)
        {
            var removeCommand = new RemoveWorkAvailabilityCommand(id);
            bus.SendCommand(removeCommand);
        }

        public void Update(WorkAvailabilityVM request)
        {
            var updateCommand = mapper.Map<UpdateWorkAvailabilityCommand>(request);
            bus.SendCommand(updateCommand);
        }
    }
}
