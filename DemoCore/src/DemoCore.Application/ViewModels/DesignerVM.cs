﻿using System.ComponentModel.DataAnnotations;
using static DemoCore.Domain.Core.Enums.EntityStateEnum;

namespace DemoCore.Application.ViewModels
{
    public class DesignerVM
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Description EN is required.")]
        [MinLength(3)]
        [MaxLength(500)]
        [Display(Name = "English Description")]
        public string DescriptionEN { get; set; }
        [Required(ErrorMessage = "Description PT is required.")]
        [MinLength(3)]
        [MaxLength(500)]
        [Display(Name = "Portuguese Description")]
        public string DescriptionPT { get; set; }
        public EntityStateOptions EntityState { get; set; }
        //public virtual ICollection<KnowledgeDesignerVM> Knowledge { get; set; }

    }
}