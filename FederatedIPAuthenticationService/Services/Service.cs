using FederatedIPAuthenticationService.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Services
{
    public abstract class Service : IService
    {
        protected IServices Services { get; private set; }
        public void InitService(IServices services)
        {
            Services = services;
            Init();
        }
        public Service() { }
        protected virtual void Init() { }

    }
}
