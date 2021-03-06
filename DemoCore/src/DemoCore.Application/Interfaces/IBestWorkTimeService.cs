﻿using DemoCore.Application.ViewModels;
using System;
using System.Collections.Generic;

namespace DemoCore.Application.Interfaces
{
    public interface IBestWorkTimeService: IDisposable
    {
        void Register(BestWorkTimeVM bestWorkVM);
        IEnumerable<BestWorkTimeVM> GetAll();
        BestWorkTimeVM GetById(int id);
        void Update(BestWorkTimeVM bestWorkVM);
        void Remove(int id);
        IEnumerable<SelectedItems> GetSelectedBestWorkTime();
        //IList<CustomerHistoryData> GetAllHistory(Guid id);
    }
}
