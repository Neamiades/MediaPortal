﻿#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaService.BLL.DTO;
using MediaService.BLL.DTO.Enums;

#endregion

namespace MediaService.BLL.Interfaces
{
    public interface IFileService : IObjectService<FileEntryDto>
    {
        Task AddRangeAsync(IEnumerable<FileEntryDto> files, Guid parentId);

        Task AddTagAsync(Guid fileId, string tagName);

        Task RenameAsync(FileEntryDto editedFileEntryDto);

        Task DeleteAsync(Guid entryId);

        Task DownloadWithJobAsync(IEnumerable<Guid> filesIds, Guid zipId);

        Task DownloadAsync(IEnumerable<Guid> filesIds, Guid zipId);

        Task<(Stream blobStream, string contentType)> DownloadAsync(Guid fileId)

        string GetLinkToZip(string fileName);

        Task<string> GetPublicLinkToFileAsync(Guid fileId, DateTimeOffset expiryTime);

        Task<IEnumerable<FileEntryDto>> SearchFilesAsync(Guid parentId, SearchType searchType, string searchValue);

        Task<string> GetLinkToFileThumbnailAsync(Guid fileId);

        Task GenerateThumbnailsToFilesAsync(IEnumerable<string> filesNames);
    }
}