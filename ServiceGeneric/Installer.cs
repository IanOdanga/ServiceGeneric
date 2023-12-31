﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServiceGeneric
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        #region Service Properties (you can be freely change it)

        /// <summary>
        /// The ServiceName cannot be null or have zero length. Its maximum size is 256 characters. 
        /// It also cannot contain forward or backward slashes, '/' or '\', or characters 
        /// from the ASCII character set with value less than decimal value 32.
        /// </summary>
        private string ServiceName
        {
            get
            {
                var friendlyName = "ServiceGeneric";
                return friendlyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ?
                                    friendlyName.Substring(0, friendlyName.Length - 4) :
                                    friendlyName;
            }
        }

        private string DisplayName
        {
            get
            {
                return ServiceName;
            }
        }

        private string Description
        {
            get
            {
                return ServiceName;
            }
        }

        #endregion

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            serviceInstaller1.ServiceName = ServiceName;
            serviceInstaller1.DisplayName = DisplayName;
            serviceInstaller1.Description = Description;
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            serviceInstaller1.ServiceName = ServiceName;
            serviceInstaller1.DisplayName = DisplayName;
            serviceInstaller1.Description = Description;
        }
    }
}
