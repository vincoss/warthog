﻿using Shot.Licensing.Sample_XamarinForms.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace Shot.Licensing.Sample_XamarinForms.ViewModels
{
    public class LicenseViewModel : BaseViewModel
    {
        private Guid _key;
        private readonly ILicenseService _licenseService;
        private readonly IApplicationContext _ctx;
        public LicenseViewModel(ILicenseService licenseService, IApplicationContext ctx)
        {
            if (licenseService == null)
            {
                throw new ArgumentNullException(nameof(licenseService));
            }
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            _licenseService = licenseService;
            _ctx = ctx;

            ActivateCommand = new Command(OnActivateCommand, OnCanActivateCommand);
            PropertyChanged += LicenseViewModel_PropertyChanged;
        }

        public async override void Initialize()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }
                
                IsBusy = true;
               
                _key = Guid.Empty;
                RegisterKey = null;
                ErrorMessage = null;
                LicenseKey = null;
                ShowError = false;
                LicenseType = LicenseGlobals.Get().ToString();
                AppId = _ctx.GetValueOrDefault(LicenseGlobals.AppId, null);

                var result = await _licenseService.ValidateAsync();
                if(result.Successful)
                {
                    LicenseGlobals.Set(AppLicense.Full);
                    LicenseKey = result.License.Id.ToString();
                    LicenseType = LicenseGlobals.Get().ToString();
                }
                else
                {
                    ShowLicenseError(result);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        #region Private methods

        private void LicenseViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RegisterKey))
            {
                _key = Guid.Empty;
                ErrorMessage = null;

                if (string.IsNullOrEmpty(RegisterKey) == false)
                {
                    Guid id;
                    Guid.TryParse(RegisterKey, out id);

                    if (id == Guid.Empty)
                    {
                        ErrorMessage = "Invalid license key."; // TODO: localize or call result form service
                    }
                    _key = id;
                }
            }

            ShowError = string.IsNullOrWhiteSpace(ErrorMessage) == false;

            ((Command)ActivateCommand).ChangeCanExecute();
        }

        private void ShowLicenseError(LicenseResult result)
        {
            var sb = new StringBuilder();

            foreach (var f in result.Failures)
            {
                sb.AppendLine(f.Code); // TODO: localize
                sb.AppendLine(f.Message); // TODO: localize
                sb.AppendLine(f.HowToResolve); // TODO: localize
            }

            if (result.Exception != null)
            {
                sb.AppendLine(result.Exception.Message); // TODO: generic failure instead
            }

            ErrorMessage = sb.Length > 0 ? sb.ToString() : null;
        }

        #endregion

        #region Command methods

        private async void OnActivateCommand()
        {
            try
            {
                if (IsBusy)
                {
                    return;
                }

                IsBusy = true;
                var result =  await _licenseService.RegisterAsync(_key);
                if(result.Successful)
                {
                    await _ctx.SetLicenseKeyAsync(result.License.Id.ToString());
                    LicenseGlobals.Set(AppLicense.Full);// TODO: remove
                    Initialize();
                }
                else
                {
                    ShowLicenseError(result);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool OnCanActivateCommand()
        {
            return _key != Guid.Empty;
        }

        #endregion

        #region Commands

        public ICommand ActivateCommand { get; private set; }

        #endregion

        #region Properties

        private string _registerKey;

        public string RegisterKey
        {
            get { return _registerKey; }
            set { SetProperty(ref _registerKey, value); }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        private bool _showError;

        public bool ShowError
        {
            get { return _showError; }
            set { SetProperty(ref _showError, value); }
        }

        private string _appId;

        public string AppId
        {
            get { return _appId; }
            set { SetProperty(ref _appId, value); }
        }

        private string _licenseKey;

        public string LicenseKey
        {
            get { return _licenseKey; }
            set { SetProperty(ref _licenseKey, value); }
        }

        private string _licenseType;

        public string LicenseType
        {
            get { return _licenseType; }
            set { SetProperty(ref _licenseType, value); }
        }

        #endregion
    }
}