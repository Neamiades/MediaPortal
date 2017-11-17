﻿#region usings

using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace MediaService.PL.Models.IdentityModels
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", false)
        {
        }

        //public virtual DbSet<ObjectEntry> ObjectEntries { get; set; }

        //public virtual DbSet<FileEntry> FileEntries { get; set; }

        //public virtual DbSet<DirectoryEntry> DirectoryEntries { get; set; }

        //public virtual DbSet<Tag> Tags { get; set; }

        //public virtual DbSet<UserProfile> UserProfiles { get; set; }

        //public virtual DbSet<ObjectViewers> ObjectViewers { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}