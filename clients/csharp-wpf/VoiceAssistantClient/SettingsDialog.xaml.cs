﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace VoiceAssistantClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using VoiceAssistantClient.Settings;

    /// <summary>
    /// Interaction logic for SettingsDialog.xaml.
    /// </summary>
    public partial class SettingsDialog : Window
    {
        private const int UrlHistoryMaxLength = 10;
        private RuntimeSettings settings;

        private bool renderComplete;

        public SettingsDialog(RuntimeSettings settings)
        {
            this.renderComplete = false;

            this.settings = settings;
            (
                this.ConnectionProfileName,
                this.ConnectionProfile,
                this.settings.Profile,
                this.settings.ConnectionProfileNameHistory,
                this.settings.ConnectionProfileHistory) = settings.Get();

            this.CustomSpeechConfig = new CustomSpeechConfiguration(settings.CustomSpeechEndpointId);
            this.VoiceDeploymentConfig = new VoiceDeploymentConfiguration(settings.VoiceDeploymentIds);
            this.WakeWordConfig = new WakeWordConfiguration(settings.WakeWordPath);

            this.InitializeComponent();
            this.DataContext = this;
            this.Owner = App.Current.MainWindow;
        }

        public ConnectionProfile Profile { get; set; }

        public string ConnectionProfileName { get; set; }

        public Dictionary<string, ConnectionProfile> ConnectionProfile { get; set; }

        public string SubscriptionKey { get; set; }

        public string SubscriptionKeyRegion { get; set; }

        public string CustomCommandsAppId { get; set; }

        public string BotId { get; set; }

        public string ConnectionLanguage { get; set; }

        public string LogFilePath { get; set; }

        public string UrlOverride { get; set; }

        public string ProxyHostName { get; set; }

        public string ProxyPortNumber { get; set; }

        public string FromId { get; set; }

        public string WakeWordPath { get; set; }

        public WakeWordConfiguration WakeWordConfig { get; set; }

        public bool WakeWordEnabled { get; set; }

        public CustomSpeechConfiguration CustomSpeechConfig { get; set; }

        public string CustomSpeechEndpointId { get; set; }

        public bool CustomSpeechEnabled { get; set; }

        public VoiceDeploymentConfiguration VoiceDeploymentConfig { get; set; }

        public string VoiceDeploymentIds { get; set; }

        public bool VoiceDeploymentEnabled { get; set; }

        protected override void OnContentRendered(EventArgs e)
        {
            this.WakeWordPathTextBox.Text = this.settings.WakeWordPath ?? string.Empty;
            this.UpdateSaveButtonState();
            this.UpdateCustomSpeechStatus(false);
            this.UpdateVoiceDeploymentIdsStatus(false);
            this.UpdateWakeWordStatus();
            base.OnContentRendered(e);
            this.renderComplete = true;
        }

        protected override void OnActivated(EventArgs e)
        {
            this.ConnectionProfileComboBox.ItemsSource = this.settings.ConnectionProfileNameHistory;
            this.ConnectionProfileComboBox.Text = this.ConnectionProfileName;
            this.SubscriptionKeyTextBox.Text = this.settings.Profile.SubscriptionKey;
            this.SubscriptionRegionTextBox.Text = this.settings.Profile.SubscriptionKeyRegion;
            this.CustomCommandsAppIdTextBox.Text = this.settings.Profile.CustomCommandsAppId;
            this.BotIdTextBox.Text = this.settings.Profile.BotId;
            this.LanguageTextBox.Text = this.settings.Profile.ConnectionLanguage;
            this.LogFileTextBox.Text = this.settings.Profile.LogFilePath;
            this.UrlOverrideTextBox.Text = this.settings.Profile.UrlOverride;
            this.ProxyHost.Text = this.settings.Profile.ProxyHostName;
            this.ProxyPort.Text = this.settings.Profile.ProxyPortNumber;
            this.FromIdTextBox.Text = this.settings.Profile.FromId;
            this.WakeWordPathTextBox.Text = this.settings.Profile.WakeWordPath;
            this.WakeWordEnabledBox.IsChecked = this.settings.Profile.WakeWordEnabled;
            this.CustomSpeechEndpointIdTextBox.Text = this.settings.Profile.CustomSpeechEndpointId;
            this.CustomSpeechEnabledBox.IsChecked = this.settings.Profile.CustomSpeechEnabled;
            this.VoiceDeploymentIdsTextBox.Text = this.settings.Profile.VoiceDeploymentIds;
            this.VoiceDeploymentEnabledBox.IsChecked = this.settings.Profile.VoiceDeploymentEnabled;

            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            this.ConnectionProfileName = this.ConnectionProfileComboBox.Text;
            this.settings.Profile.SubscriptionKey = this.SubscriptionKeyTextBox.Text;
            this.settings.Profile.SubscriptionKeyRegion = this.SubscriptionRegionTextBox.Text;
            this.settings.Profile.CustomCommandsAppId = this.CustomCommandsAppIdTextBox.Text;
            this.settings.Profile.BotId = this.BotIdTextBox.Text;
            this.settings.Profile.ConnectionLanguage = this.LanguageTextBox.Text;
            this.settings.Profile.LogFilePath = this.LogFileTextBox.Text;
            this.settings.Profile.UrlOverride = this.UrlOverrideTextBox.Text;
            this.settings.Profile.ProxyHostName = this.ProxyHost.Text;
            this.settings.Profile.ProxyPortNumber = this.ProxyPort.Text;
            this.settings.Profile.FromId = this.FromIdTextBox.Text;
            this.settings.Profile.WakeWordPath = this.WakeWordPathTextBox.Text;
            this.settings.Profile.WakeWordEnabled = (bool)this.WakeWordEnabledBox.IsChecked;
            this.settings.Profile.CustomSpeechEndpointId = this.CustomSpeechEndpointIdTextBox.Text;
            this.settings.Profile.CustomSpeechEnabled = (bool)this.CustomSpeechEnabledBox.IsChecked;
            this.settings.Profile.VoiceDeploymentIds = this.VoiceDeploymentIdsTextBox.Text;
            this.settings.Profile.VoiceDeploymentEnabled = (bool)this.VoiceDeploymentEnabledBox.IsChecked;

            base.OnDeactivated(e);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.ConnectionProfileComboBox.Text))
            {
                if (this.ConnectionProfile.ContainsKey(this.ConnectionProfileComboBox.Text))
                {
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].SubscriptionKey = this.SubscriptionKeyTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].SubscriptionKeyRegion = this.SubscriptionRegionTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomCommandsAppId = this.CustomCommandsAppIdTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].BotId = this.BotIdTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ConnectionLanguage = this.LanguageTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].LogFilePath = this.LogFileTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].UrlOverride = this.UrlOverrideTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ProxyHostName = this.ProxyHost.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ProxyPortNumber = this.ProxyPort.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].FromId = this.FromIdTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomSpeechEndpointId = this.CustomSpeechEndpointIdTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomSpeechEnabled = (bool)this.CustomSpeechEnabledBox.IsChecked;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].VoiceDeploymentIds = this.VoiceDeploymentIdsTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].VoiceDeploymentEnabled = (bool)this.VoiceDeploymentEnabledBox.IsChecked;
                    var wakeWordConfigPath = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].WakeWordConfig.Path;
                    wakeWordConfigPath = this.WakeWordPathTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].WakeWordPath = this.WakeWordPathTextBox.Text;
                    this.ConnectionProfile[this.ConnectionProfileComboBox.Text].WakeWordEnabled = (bool)this.WakeWordEnabledBox.IsChecked;

                    this.settings.Profile.SubscriptionKey = this.SubscriptionKeyTextBox.Text;
                    this.settings.Profile.SubscriptionKeyRegion = this.SubscriptionRegionTextBox.Text;
                    this.settings.Profile.CustomCommandsAppId = this.CustomCommandsAppIdTextBox.Text;
                    this.settings.Profile.BotId = this.BotIdTextBox.Text;
                    this.settings.Profile.ConnectionLanguage = this.LanguageTextBox.Text;
                    this.settings.Profile.LogFilePath = this.LogFileTextBox.Text;
                    this.settings.Profile.UrlOverride = this.UrlOverrideTextBox.Text;
                    this.settings.Profile.ProxyHostName = this.ProxyHost.Text;
                    this.settings.Profile.ProxyPortNumber = this.ProxyPort.Text;
                    this.settings.Profile.FromId = this.FromIdTextBox.Text;
                    this.settings.Profile.CustomSpeechEndpointId = this.CustomSpeechEndpointIdTextBox.Text;
                    this.settings.Profile.CustomSpeechEnabled = (bool)this.CustomSpeechEnabledBox.IsChecked;
                    this.settings.Profile.VoiceDeploymentIds = this.VoiceDeploymentIdsTextBox.Text;
                    this.settings.Profile.VoiceDeploymentEnabled = (bool)this.VoiceDeploymentEnabledBox.IsChecked;
                    var profileWakeWordPath = this.settings.Profile.WakeWordPath;
                    profileWakeWordPath = this.WakeWordPathTextBox.Text;
                    this.settings.Profile.WakeWordEnabled = (bool)this.WakeWordEnabledBox.IsChecked;
                }
                else
                {
                    this.ConnectionProfile.Add(this.ConnectionProfileName, new ConnectionProfile
                    {
                        SubscriptionKey = this.SubscriptionKey,
                        SubscriptionKeyRegion = this.SubscriptionKeyRegion,
                        CustomCommandsAppId = this.CustomCommandsAppId,
                        BotId = this.BotId,
                        ConnectionLanguage = this.ConnectionLanguage,
                        LogFilePath = this.LogFilePath,
                        UrlOverride = this.UrlOverride,
                        ProxyHostName = this.ProxyHostName,
                        ProxyPortNumber = this.ProxyPortNumber,
                        FromId = this.FromId,
                        WakeWordPath = this.WakeWordPath,
                        WakeWordConfig = this.WakeWordConfig,
                        CustomSpeechConfig = this.CustomSpeechConfig,
                        CustomSpeechEndpointId = this.CustomSpeechEndpointId,
                        CustomSpeechEnabled = this.CustomSpeechEnabled,
                        VoiceDeploymentConfig = this.VoiceDeploymentConfig,
                        VoiceDeploymentIds = this.VoiceDeploymentIds,
                        VoiceDeploymentEnabled = this.VoiceDeploymentEnabled,
                    });
                }

                this.AddConnectionProfileNameIntoHistory(this.ConnectionProfileName);
                this.AddConnectionProfileIntoHistory(this.ConnectionProfile);
                this.settings.Set(
                    this.ConnectionProfileName,
                    this.ConnectionProfile,
                    this.settings.Profile,
                    this.settings.ConnectionProfileNameHistory,
                    this.settings.ConnectionProfileHistory);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.ConnectionProfileComboBox.Text))
            {
                this.ConnectionProfile.Remove(this.ConnectionProfileComboBox.Text);
                this.settings.ConnectionProfileNameHistory.Remove(this.ConnectionProfileComboBox.Text);
                this.ConnectionProfileComboBox.Text = string.Empty;
            }
        }

        private void AddConnectionProfileNameIntoHistory(string connectionProfileName)
        {
            var profileNameHistory = this.settings.ConnectionProfileNameHistory;

            var existingItem = profileNameHistory.FirstOrDefault(item => string.Compare(item, connectionProfileName, StringComparison.OrdinalIgnoreCase) == 0);

            if (existingItem == null)
            {
                profileNameHistory.Insert(0, connectionProfileName);
                if (profileNameHistory.Count == UrlHistoryMaxLength)
                {
                    profileNameHistory.RemoveAt(UrlHistoryMaxLength - 1);
                }
            }
        }

        private void AddConnectionProfileIntoHistory(Dictionary<string, ConnectionProfile> connectionProfile)
        {
            var connectionProfileHistory = this.settings.ConnectionProfileHistory;

            foreach (var item in connectionProfileHistory)
            {
                if (item != null)
                {
                    if (item.ContainsKey(this.ConnectionProfileComboBox.Text))
                    {
                        if (item == null)
                        {
                            connectionProfileHistory.Insert(0, connectionProfile);
                            if (connectionProfileHistory.Count == UrlHistoryMaxLength)
                            {
                                connectionProfileHistory.RemoveAt(UrlHistoryMaxLength - 1);
                            }
                        }
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void WakeWordBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();

            // Filter files by the file extension .table, to find the file downloaded
            // from the Azure web portal for "Speech Customization - Custom Wake Word"
            openDialog.Filter = "Wake word files (*.table)|*.table|All files (*.*)|*.*";

            try
            {
                var fileInfo = new FileInfo(this.WakeWordPathTextBox.Text);
                openDialog.InitialDirectory = fileInfo.DirectoryName;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Debug.WriteLine($"Bad path for initial directory: {ex.Message}");
            }
#pragma warning restore CA1031 // Do not catch general exception types

            bool? result = openDialog.ShowDialog();

            if (result == true)
            {
                this.WakeWordPathTextBox.Text = openDialog.FileName;
            }
        }

        private void WakeWordEnabledBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.UpdateWakeWordStatus();
            }
        }

        private void UpdateSaveButtonState()
        {
            // BUGBUG: The transfer into variables does not seem to be done consistently with these events so we read straight from the controls
            var hasConnectionProfileName = !string.IsNullOrWhiteSpace(this.ConnectionProfileComboBox.Text);
            var hasSubscription = !string.IsNullOrWhiteSpace(this.SubscriptionKeyTextBox.Text);
            var hasRegion = !string.IsNullOrWhiteSpace(this.SubscriptionRegionTextBox.Text);
            var hasUrlOverride = !string.IsNullOrWhiteSpace(this.UrlOverrideTextBox.Text);

            var enableSaveButton = false;
            var enableDeleteButton = false;
            if (!hasConnectionProfileName)
            {
                this.SaveButtonInfoBlock.Text = "You must provide a profile name";
            }
            else if (!hasSubscription)
            {
                this.SaveButtonInfoBlock.Text = "You must provide a speech subscription key.";
            }
            else if (!hasRegion && !hasUrlOverride)
            {
                this.SaveButtonInfoBlock.Text = "You must provide a region or URL override.";
            }
            else if (hasRegion && hasUrlOverride)
            {
                this.SaveButtonInfoBlock.Text = "You must specify only region OR URL override, not both.";
            }
            else
            {
                this.SaveButtonInfoBlock.Text = string.Empty;
                enableSaveButton = true;
                enableDeleteButton = true;

            }

            this.SaveButton.IsEnabled = enableSaveButton;
            this.DeleteProfile.IsEnabled = enableDeleteButton;
        }

        private void CustomSpeechEnabledBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.UpdateCustomSpeechStatus(true);
            }
        }

        private void UpdateCustomSpeechStatus(bool updateLabelOnInvalidContent)
        {
            this.CustomSpeechConfig = new CustomSpeechConfiguration(this.CustomSpeechEndpointIdTextBox.Text);

            if (!this.CustomSpeechConfig.IsValid)
            {
                if (updateLabelOnInvalidContent)
                {
                    this.CustomSpeechEnabledBox.Content = "Invalid endpoint ID format";
                    Debug.WriteLine("Invalid endpoint ID format. It needs to be a GUID in the format ########-####-####-####-############");
                }
                else
                {
                    this.CustomSpeechEnabledBox.Content = "Click to enable";
                }

                this.CustomSpeechEnabled = false;
                this.CustomSpeechEnabledBox.IsChecked = false;
            }
            else if (this.CustomSpeechEnabled)
            {
                this.CustomSpeechEnabledBox.Content = "Custom speech will be used upon next connection";
            }
            else
            {
                this.CustomSpeechEnabledBox.Content = "Click to enable";
            }
        }

        private void CustomSpeechEndpointIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.CustomSpeechEnabled = false;
                this.CustomSpeechEnabledBox.IsChecked = false;
            }

            this.CustomSpeechEnabledBox.Content = "Click to enable";
        }

        private void VoiceDeploymentEnabledBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.UpdateVoiceDeploymentIdsStatus(true);
            }
        }

        private void UpdateVoiceDeploymentIdsStatus(bool updateLabelOnInvalidContent)
        {
            this.VoiceDeploymentConfig = new VoiceDeploymentConfiguration(this.VoiceDeploymentIdsTextBox.Text);

            if (!this.VoiceDeploymentConfig.IsValid)
            {
                if (updateLabelOnInvalidContent)
                {
                    this.VoiceDeploymentEnabledBox.Content = "Invalid voice deployment IDs format";
                    Debug.WriteLine("Invalid voice deployment IDs format. It needs to be a GUID in the format ########-####-####-####-############");
                }
                else
                {
                    this.VoiceDeploymentEnabledBox.Content = "Click to enable";
                }

                this.VoiceDeploymentEnabled = false;
                this.VoiceDeploymentEnabledBox.IsChecked = false;
            }
            else if (this.VoiceDeploymentEnabled)
            {
                this.VoiceDeploymentEnabledBox.Content = "Voice deployment IDs will be used upon next connection";
            }
            else
            {
                this.VoiceDeploymentEnabledBox.Content = "Click to enable";
            }
        }

        private void VoiceDeploymentIdsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.VoiceDeploymentEnabled = false;
                this.VoiceDeploymentEnabledBox.IsChecked = false;
            }

            this.VoiceDeploymentEnabledBox.Content = "Click to enable";
        }

        private void UpdateWakeWordStatus()
        {
            this.WakeWordConfig = new WakeWordConfiguration(this.WakeWordPathTextBox.Text);

            if (!this.WakeWordConfig.IsValid)
            {
                this.WakeWordEnabledBox.Content = "Invalid wake word model file or location";
                this.WakeWordEnabled = false;
                this.WakeWordEnabledBox.IsChecked = false;
            }
            else if (this.WakeWordEnabled)
            {
                this.WakeWordEnabledBox.Content = $"Will listen for the wake word upon next connection";
            }
            else
            {
                this.WakeWordEnabledBox.Content = "Click to enable";
            }
        }

        private void SubscriptionKeyRegionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateSaveButtonState();
        }

        private void SubscriptionKeyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateSaveButtonState();
        }

        private void CustomCommandsAppIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateSaveButtonState();
        }

        private void WakeWordPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.renderComplete)
            {
                this.WakeWordEnabled = false;
                this.WakeWordEnabledBox.IsChecked = false;
            }

            this.WakeWordEnabledBox.Content = "Check to enable";
        }

        private void UrlOverrideTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateSaveButtonState();
        }

        private void ConnectionProfileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.ConnectionProfile.Count == 0)
            {
                this.ConnectionProfileName = this.ConnectionProfileComboBox.Text;
            }

            if (this.ConnectionProfile.Count != 0)
            {
                if (this.ConnectionProfile.ContainsKey(this.ConnectionProfileComboBox.Text))
                {
                    this.SubscriptionKeyTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].SubscriptionKey;
                    this.SubscriptionRegionTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].SubscriptionKeyRegion;
                    this.CustomCommandsAppIdTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomCommandsAppId;
                    this.BotIdTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].BotId;
                    this.LanguageTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ConnectionLanguage;
                    this.LogFileTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].LogFilePath;
                    this.UrlOverrideTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].UrlOverride;
                    this.ProxyHost.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ProxyHostName;
                    this.ProxyPort.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].ProxyPortNumber;
                    this.FromIdTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].FromId;
                    this.CustomSpeechEndpointIdTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomSpeechEndpointId;
                    this.CustomSpeechEnabledBox.IsChecked = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].CustomSpeechEnabled;
                    this.VoiceDeploymentIdsTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].VoiceDeploymentIds;
                    this.VoiceDeploymentEnabledBox.IsChecked = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].VoiceDeploymentEnabled;
                    this.WakeWordPathTextBox.Text = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].WakeWordPath;
                    this.WakeWordEnabledBox.IsChecked = this.ConnectionProfile[this.ConnectionProfileComboBox.Text].WakeWordEnabled;
                }
                else
                {
                    this.SubscriptionKeyTextBox.Text = string.Empty;
                    this.SubscriptionRegionTextBox.Text = string.Empty;
                    this.CustomCommandsAppIdTextBox.Text = string.Empty;
                    this.BotIdTextBox.Text = string.Empty;
                    this.LanguageTextBox.Text = string.Empty;
                    this.LogFileTextBox.Text = string.Empty;
                    this.UrlOverrideTextBox.Text = string.Empty;
                    this.ProxyHost.Text = string.Empty;
                    this.ProxyPort.Text = string.Empty;
                    this.FromIdTextBox.Text = string.Empty;
                    this.CustomSpeechEndpointIdTextBox.Text = string.Empty;
                    this.CustomSpeechEnabledBox.IsChecked = false;
                    this.VoiceDeploymentIdsTextBox.Text = string.Empty;
                    this.VoiceDeploymentEnabledBox.IsChecked = false;
                    this.WakeWordPathTextBox.Text = string.Empty;
                    this.WakeWordEnabledBox.IsChecked = false;
                }
            }
        }
    }
}