﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MediaService.Startup))]
namespace MediaService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
