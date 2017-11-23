﻿using AutoMapper;
using MediaService.BLL.DTO;
using MediaService.BLL.Interfaces;
using MediaService.PL.Models.IdentityModels.Managers;
using MediaService.PL.Models.ObjectViewModels.DirectoryViewModels;
using MediaService.PL.Models.ObjectViewModels.Enums;
using MediaService.PL.Utils;
using MediaService.PL.Utils.Attributes.ErrorHandler;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MediaService.PL.Controllers
{
    public class DirectoryController : Controller
    {
        #region Fields

        private IUserService _applicationUserService;

        private IDirectoryService _directoryService;

        private IFilesService _filesService;

        private IMapper _mapper;

        private ApplicationUserManager _userManager;

        #endregion

        #region Constructors

        public DirectoryController()
        {
        }

        public DirectoryController(
            ApplicationUserManager userManager,
            IUserService applicationUserService,
            IDirectoryService directoryService,
            IFilesService filesService
        )
        {
            UserManager = userManager;
            ApplicationUserService = applicationUserService;
            DirectoryService = directoryService;
            FilesService = filesService;
        }

        #endregion

        #region Services Properties

        private IMapper Mapper => _mapper ?? (_mapper = MapperModule.GetMapper());

        private ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            set => _userManager = value;
        }

        private IUserService ApplicationUserService
        {
            get => _applicationUserService ?? HttpContext.GetOwinContext().GetUserManager<IUserService>();
            set => _applicationUserService = value;
        }

        private IDirectoryService DirectoryService
        {
            get => _directoryService ?? HttpContext.GetOwinContext().GetUserManager<IDirectoryService>();
            set => _directoryService = value;
        }

        private IFilesService FilesService
        {
            get => _filesService ?? HttpContext.GetOwinContext().GetUserManager<IFilesService>();
            set => _filesService = value;
        }

        #endregion

        #region Actions

        // Get: /Directory/DirectoriesList
        [HttpGet]
        [ErrorHandle(ExceptionType = typeof(DataException), View = "Errors/Error")]
        public async Task<ActionResult> DirectoriesList(DirectoriesListViewModel model)
        {
            try
            {
                var directories = await DirectoryService.GetByParentIdAsync(model.ParentId);
                switch (model.OrderType)
                {
                    case OrderType.BySize:
                    case OrderType.ByName:
                        directories = directories.OrderBy(d => d.Name);
                        break;
                    case OrderType.ByCreationTime:
                        directories = directories.OrderBy(d => d.Created);
                        break;
                    case OrderType.ByUploadingTime:
                        directories = directories.OrderBy(d => d.Downloaded);
                        break;
                }
                return PartialView("_DirectoriesList", directories);
            }
            catch (Exception ex)
            {
                throw new DataException("We can't display your files at this moment, we're sorry, try again later", ex);
            }
        }

        [HttpGet]
        public ActionResult Create(Guid parentId)
        {
            var model = new CreateDirectoryViewModel { ParentId = parentId };

            return PartialView("_CreateDirectory", model);
        }

        // POST: /Directory/Create
        [HttpPost]
        [ErrorHandle(ExceptionType = typeof(DbUpdateException), View = "Errors/Error")]
        public async Task<ActionResult> Create(CreateDirectoryViewModel model)
        {
            try
            {
                if (!await DirectoryService.ExistAsync(model.Name, model.ParentId))
                {
                    var newFolder = Mapper.Map<DirectoryEntryDto>(model);
                    await DirectoryService.AddAsync(newFolder);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                ModelState.AddModelError("Name", "The folder with this name is already exist in this directory");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    "We can't create new folder for you at this moment, we're sorry, try again later", ex);
            }

            //We get here if were some model validation errors
            return PartialView("_CreateDirectory", model);
        }

        [HttpPost]
        [ErrorHandle(ExceptionType = typeof(DataException), View = "Errors/Error")]
        public async Task<ActionResult> Download(DownloadDirectoryViewModel model)
        {
            try
            {
                var zipId = Guid.NewGuid();
                await DirectoryService.DownloadWithJobAsync(model.Id, zipId);
                return Json(new { success = true, zipId, zipName = model.Name }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    "This folder can't be downloaded at this moment, we're sorry, try again later", ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadZip(Guid zipId, string zipName)
        {
            //var zipStream = await DirectoryService.DownloadZip($"{zipId}.zip");
            //if (zipStream.blobExist)
            //{
            //    return File(zipStream.blobStream, "application/zip", $"{zipName}.zip");
            //}
            var link = await FilesService.GetLinkToFileAsync($"{zipId}.zip");
            return link == null
                ? Json(new {success = false}, JsonRequestBehavior.AllowGet)
                : Json(new {success = true, link}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Rename(Guid id, Guid parentId, string name)
        {
            var model = new RenameDirectoryViewModel { Id = id, ParentId = parentId, Name = name };
            return PartialView("_RenameDirectory", model);
        }

        [HttpPost]
        [ErrorHandle(ExceptionType = typeof(DbUpdateException), View = "Errors/Error")]
        public async Task<ActionResult> Rename(RenameDirectoryViewModel model)
        {
            try
            {
                if (!await DirectoryService.ExistAsync(model.Name, model.ParentId))
                {
                    var editedFolder = Mapper.Map<DirectoryEntryDto>(model);
                    await DirectoryService.RenameAsync(editedFolder);
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                ModelState.AddModelError("Name", "The folder with this name is already exist in this directory");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    "This folder can't be renamed at this moment, we're sorry, try again later", ex);
            }

            //We get here if were some model validation errors
            return PartialView("_RenameDirectory", model);
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            var model = new DeleteDirectoryViewModel { Id = id };

            return PartialView("_DeleteDirectory", model);
        }

        [HttpPost]
        [ErrorHandle(ExceptionType = typeof(DbUpdateException), View = "Errors/Error")]
        public async Task<ActionResult> Delete(DeleteDirectoryViewModel model)
        {
            try
            {
                await DirectoryService.DeleteWithJobAsync(model.Id);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    "This folder can't be deleted at this moment, we're sorry, try again later", ex);
                //return PartialView("_DeleteDirectory", model);
            }
        }

        #endregion

        #region Helper Methods

        

        #endregion

        #region Overrided Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_directoryService != null)
                {
                    _directoryService.Dispose();
                    _directoryService = null;
                }

                if (_applicationUserService != null)
                {
                    _applicationUserService.Dispose();
                    _applicationUserService = null;
                }

                if (_filesService != null)
                {
                    _filesService.Dispose();
                    _filesService = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
