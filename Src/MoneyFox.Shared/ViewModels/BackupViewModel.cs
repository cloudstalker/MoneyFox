﻿using Microsoft.OneDrive.Sdk;
using MoneyFox.Shared.Exceptions;
using MoneyFox.Shared.Interfaces;
using MoneyFox.Shared.Resources;
using MvvmCross.Core.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin;

namespace MoneyFox.Shared.ViewModels
{
    public class BackupViewModel : BaseViewModel
    {
        private readonly IDialogService dialogService;
        private readonly IRepositoryManager repositoryManager;

        public BackupViewModel(IRepositoryManager repositoryManager,
            IBackupService backupService,
            IDialogService dialogService)
        {
            this.repositoryManager = repositoryManager;
            this.dialogService = dialogService;

            BackupService = backupService;
        }

        /// <summary>
        ///     The Date when the backup was modified the last time.
        /// </summary>
        public DateTime BackupLastModified { get; private set; }

        /// <summary>
        ///     Prepares the View when loaded.
        /// </summary>
        public MvxCommand LoadedCommand => new MvxCommand(Loaded);

        /// <summary>
        ///     The Backup Service for the current platform.
        ///     On Android this needs to be overwritten with an
        ///     instance with the current activity setup.
        /// </summary>
        public IBackupService BackupService { get; }

        /// <summary>
        ///     Will create a backup of the database and upload it to onedrive
        /// </summary>
        public MvxCommand BackupCommand => new MvxCommand(CreateBackup);

        /// <summary>
        ///     Will download the database backup from onedrive and overwrite the
        ///     local database with the downloaded.
        ///     All datamodels are then reloaded.
        /// </summary>
        public MvxCommand RestoreCommand => new MvxCommand(RestoreBackup);

        /// <summary>
        ///     Indicator if something is in work.
        /// </summary>
        public bool IsLoading { get; private set; }

        private async void Loaded()
        {
            BackupLastModified = await BackupService.GetBackupDate();
        }

        private async void CreateBackup()
        {
            // If login wasn't succesful or overwrite info was dismissed cancel upload
            if (await Login() ||!await ShowOverwriteBackupInfo())
            {
                return;
            }

            IsLoading = true;
            await BackupService.Upload();
            BackupLastModified = DateTime.Now;
            await ShowCompletionNote();
            IsLoading = false;
        }

        private async void RestoreBackup()
        {
            // If login wasn't succesful or overwrite info was dismissed cancel download
            if (await Login() || !await ShowOverwriteDataInfo())
            {
                return;
            }

            IsLoading = true;

            await BackupService.Restore();
            repositoryManager.ReloadData();

            await ShowCompletionNote();
            IsLoading = false;
        }

        private async Task<bool> Login()
        {
            try
            {
                IsLoading = true;
                await BackupService.Login();
                IsLoading = false;
                return true;
            }
            catch (ConnectionException)
            {
                await dialogService.ShowMessage(Strings.LoginFailedTitle, Strings.LoginFailedMessage);
            }
            catch (OneDriveException ex)
            {
                Insights.Report(ex, Insights.Severity.Error);
                await dialogService.ShowMessage(Strings.LoginFailedTitle, Strings.LoginFailedMessage);
            }
            return false;
        }

        private async Task<bool> ShowOverwriteBackupInfo()
            => await dialogService.ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteBackupMessage);

        private async Task<bool> ShowOverwriteDataInfo()
            => await dialogService.ShowConfirmMessage(Strings.OverwriteTitle, Strings.OverwriteDataMessage);

        private async Task ShowCompletionNote()
        {
            await dialogService.ShowMessage(Strings.SuccessTitle, Strings.TaskSuccessfulMessage);
        }
    }
}