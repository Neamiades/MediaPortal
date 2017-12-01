﻿#region usings

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

#endregion

namespace MediaService.PL.Models.ObjectViewModels.FileViewModels
{
    public class DeleteFileViewModel
    {
        [Required]
        [HiddenInput(DisplayValue = false)]
        public Guid FileId { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public Guid ParentId { get; set; }
    }
}