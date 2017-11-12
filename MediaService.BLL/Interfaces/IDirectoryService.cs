﻿using System.Threading.Tasks;
using MediaService.BLL.DTO;

namespace MediaService.BLL.Interfaces
{
    public interface IDirectoryService : IObjectService<DirectoryEntryDto>
    {
        Task AddRootDirToUserAsync(string userId);
    }
}
