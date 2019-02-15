﻿using System;
using System.Collections.Generic;
using static DemoCore.Domain.Core.Enums.EntityStateEnum;

namespace DemoCore.Application.ViewModels
{
    public class DeveloperVM
    {
        public int Id { get; set; }
        public string DescriptionEN { get; set; }
        public string DescriptionPT { get; set; }
        public EntityStateOptions EntityState { get; set; }
        public bool HasChanges { get; set; }
        public bool IsNew { get; protected set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdate { get; set; }
        public ICollection<KnowledgeDeveloperVM> Knowledge { get; set; }
    }
}