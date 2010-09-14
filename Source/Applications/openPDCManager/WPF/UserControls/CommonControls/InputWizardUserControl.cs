﻿//******************************************************************************************************
//  InputWizardUserControl.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/17/2010 - Mehulbhai P Thakkar
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using openPDCManager.Data;
using openPDCManager.Data.BusinessObjects;
using openPDCManager.Data.Entities;
using openPDCManager.ModalDialogs;
using openPDCManager.Utilities;
using openPDCManager.Pages.Devices;
using openPDCManager.Data.ServiceCommunication;

namespace openPDCManager.UserControls.CommonControls
{
    public partial class InputWizardUserControl
    {
        #region [ Members ]

        bool m_bindingDevices;

        #endregion

        #region [ Methods ]

        void Initialize()
        {
            ItemControlDeviceList.SizeChanged += new SizeChangedEventHandler(ItemControlDeviceList_SizeChanged);
        }

        void ItemControlDeviceList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_activityWindow != null && m_bindingDevices)
                m_activityWindow.Close();
        }

        void RetrieveConfigurationFrame()
        {
            SystemMessages sm;
            try
            {
                m_wizardDeviceInfoList = new ObservableCollection<WizardDeviceInfo>(CommonFunctions.RetrieveConfigurationFrame(((App)Application.Current).RemoteStatusServiceUrl, this.ConnectionString(), ((KeyValuePair<int, string>)ComboboxProtocol.SelectedItem).Key));
                if (m_wizardDeviceInfoList.Count > 0)
                    m_bindingDevices = true;
                else
                    m_bindingDevices = false;

                ItemControlDeviceList.ItemsSource = m_wizardDeviceInfoList;
                sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Retrieved Configuration Successfully!", SystemMessage = "", UserMessageType = openPDCManager.Utilities.MessageType.Success }, ButtonType.OkOnly);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.RetrieveConfigurationFrame", ex);
                sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Configuration", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
            }
            sm.Owner = Window.GetWindow(this);
            sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sm.ShowPopup();
            if (m_activityWindow != null && !m_bindingDevices)
                m_activityWindow.Close();
        }

        void GetProtocolIDByAcronym()
        {
            try
            {
                int protocolID = CommonFunctions.GetProtocolIDByAcronym(m_connectionSettings.PhasorProtocol.ToString());
                if (protocolID > 0)
                {
                    foreach (KeyValuePair<int, string> item in ComboboxProtocol.Items)
                    {
                        if (item.Key == protocolID)
                        {
                            ComboboxProtocol.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetProtocolIDByAcronym", ex);
            }            
        }

        void SaveIniFile()
        {           
        }

        void GetExecutingAssemblyPath()
        {
            m_iniFilePath = "nothing"; //CommonFunctions.GetExecutingAssemblyPath();            
        }

        void SaveDevice(Device device, bool isNew, int digitalCount, int analogCount)
        {
            try
            {
                string result = CommonFunctions.SaveDevice(device, isNew, digitalCount, analogCount);
                GetDeviceByAcronym(TextBoxPDCAcronym.Text.Replace(" ", "").ToUpper());
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.SaveDevice", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Save Device Information", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void GetDeviceByAcronym(string acronym)
        {
            try
            {
                Device device = new Device();
                device = CommonFunctions.GetDeviceByAcronym(acronym);
                if (device != null && device.IsConcentrator)
                    m_parentID = device.ID;
                else
                {
                    App app = (App)Application.Current;
                    device = new Device();
                    device.Name = TextBoxPDCName.Text;
                    device.Acronym = TextBoxPDCAcronym.Text;
                    device.IsConcentrator = true;
                    device.VendorDeviceID = ((KeyValuePair<int, string>)ComboboxPDCVendor.SelectedItem).Key == 0 ? (int?)null : ((KeyValuePair<int, string>)ComboboxPDCVendor.SelectedItem).Key;
                    device.AccessID = m_wizardDeviceInfoList.Count > 0 ? m_wizardDeviceInfoList[0].ParentAccessID : 0;
                    device.NodeID = app.NodeValue;
                    device.ParentID = null;
                    device.Longitude = -98.6m;
                    device.Latitude = 37.5m;
                    device.CompanyID = ((KeyValuePair<int, string>)ComboboxCompany.SelectedItem).Key == 0 ? (int?)null : ((KeyValuePair<int, string>)ComboboxCompany.SelectedItem).Key;
                    device.ProtocolID = ((KeyValuePair<int, string>)ComboboxProtocol.SelectedItem).Key == 0 ? (int?)null : ((KeyValuePair<int, string>)ComboboxProtocol.SelectedItem).Key;
                    device.HistorianID = ((KeyValuePair<int, string>)ComboboxHistorian.SelectedItem).Key == 0 ? (int?)null : ((KeyValuePair<int, string>)ComboboxHistorian.SelectedItem).Key;
                    device.InterconnectionID = ((KeyValuePair<int, string>)ComboboxInterconnection.SelectedItem).Key == 0 ? (int?)null : ((KeyValuePair<int, string>)ComboboxInterconnection.SelectedItem).Key;
                    device.ConnectionString = this.ConnectionString();
                    device.TimeZone = string.Empty;
                    device.TimeAdjustmentTicks = 0;
                    device.MeasuredLines = 1;   //m_wizardDeviceInfoList.Count;
                    device.LoadOrder = 0;
                    device.ContactList = string.Empty;
                    device.Enabled = true;
                    device.FramesPerSecond = 30;
                    device.DataLossInterval = 5;
                    device.AllowedParsingExceptions = 10;
                    device.ParsingExceptionWindow = 5;
                    device.DelayedConnectionInterval = 5;
                    device.AllowUseOfCachedConfiguration = true;
                    device.AutoStartDataParsingSequence = true;
                    device.MeasurementReportingInterval = 100000;
                    SaveDevice(device, true, 0, 0);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetDeviceByAcronym", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Device Information by Acronym", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
            //m_client.GetDeviceByAcronymAsync(acronym);
        }

        void SaveWizardConfigurationInfo(string nodeID, ObservableCollection<WizardDeviceInfo> wizardDeviceInfoList, string connectionString,
                int? protocolID, int? companyID, int? historianID, int? interconnectionID, int? parentID, bool skipDisableRealTimeData)
        {
            SystemMessages sm;
            try
            {
                string result = CommonFunctions.SaveWizardConfigurationInfo(nodeID, new List<WizardDeviceInfo>(wizardDeviceInfoList), connectionString, protocolID, companyID, historianID, interconnectionID, parentID, skipDisableRealTimeData);
                sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = result, SystemMessage = string.Empty, UserMessageType = openPDCManager.Utilities.MessageType.Success },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();

                BrowseDevicesUserControl browseDevices = new BrowseDevicesUserControl();
                ((MasterLayoutWindow)Window.GetWindow(this)).ContentFrame.Navigate(browseDevices);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.SaveWizardConfigurationInfo", ex);
                sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Save Configuration Information", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }

            //Update Metadata in the openPDC Service.
            try
            {
                if (historianID != null)
                {
                    string runtimeID = CommonFunctions.GetRuntimeID("Historian", (int)historianID);
                    WindowsServiceClient serviceClient = ((App)Application.Current).ServiceClient;
                    if (serviceClient.Helper.RemotingClient.CurrentState == TVA.Communication.ClientState.Connected)
                        CommonFunctions.SendCommandToWindowsService(serviceClient, "Invoke " + runtimeID + " refreshmetadata");
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("SaveWizardConfigurationInfo.RefreshMetadata", ex);
            }

            nextButtonClicked = false;
            if (m_activityWindow != null)
                m_activityWindow.Close();
        }

        void GetWizardConfigurationInfo(Stream data)
        {
            try
            {
                m_wizardDeviceInfoList = new ObservableCollection<WizardDeviceInfo>(CommonFunctions.GetWizardConfigurationInfo(data));
                if (m_wizardDeviceInfoList.Count > 10)
                    m_bindingDevices = true;
                else
                    m_bindingDevices = false;

                ItemControlDeviceList.ItemsSource = m_wizardDeviceInfoList;                
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetWizardConfigurationInfo", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Parse Configuration File", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
            if (m_activityWindow != null && !m_bindingDevices)
                m_activityWindow.Close();
        }

        void GetConnectionSettings()
        {
            try
            {
                m_connectionSettings = CommonFunctions.GetConnectionSettings(m_connectionFileData);
                if (m_connectionSettings != null)
                {
                    string connectionString = m_connectionSettings.ConnectionString.ToLower();
                    Dictionary<string, string> connectionSettings = connectionString.ParseKeyValuePairs(';', '=', '{', '}');

                    if (connectionSettings.ContainsKey("commandchannel"))
                    {
                        TextBoxAlternateCommandChannel.Text = connectionSettings["commandchannel"];
                        connectionSettings.Remove("commandchannel");
                    }

                    if (connectionSettings.ContainsKey("skipdisablerealtimedata"))
                    {
                        m_skipDisableRealTimeData = Convert.ToBoolean(connectionSettings["skipdisablerealtimedata"]);
                        connectionSettings.Remove("skipdisablerealtimedata");
                    }

                    TextBoxConnectionString.Text = "TransportProtocol=" + m_connectionSettings.TransportProtocol.ToString() + ";" + connectionSettings.JoinKeyValuePairs(';', '=');

                    if (m_connectionSettings.ConnectionParameters != null)
                    {
                        TextBoxConnectionString.Text += ";iniFileName=" + m_connectionSettings.configurationFileName + ";refreshConfigFileOnChange=" + m_connectionSettings.refreshConfigurationFileOnChange.ToString() +
                                    ";parseWordCountFromByte=" + m_connectionSettings.parseWordCountFromByte;
                    }

                    //Select Phasor Protocol type in the combobox based on the protocol in the connection file.
                    GetProtocolIDByAcronym();
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetConnectionSettings", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Parse Connection File", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void GetInterconnections()
        {
            try
            {
                ComboboxInterconnection.ItemsSource = CommonFunctions.GetInterconnections(true);
                if (ComboboxInterconnection.Items.Count > 0)
                    ComboboxInterconnection.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetInterconnections", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Interconnections", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void GetHistorians()
        {
            try
            {
                ComboboxHistorian.ItemsSource = CommonFunctions.GetHistorians(true, true, false);
                if (ComboboxHistorian.Items.Count > 0)
                    ComboboxHistorian.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetHistorians", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Historians", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void GetCompanies()
        {
            try
            {
                ComboboxCompany.ItemsSource = CommonFunctions.GetCompanies(false);
                if (ComboboxCompany.Items.Count > 0)
                    ComboboxCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetCompanies", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Companies", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void GetProtocols()
        {
            try
            {
                ComboboxProtocol.ItemsSource = CommonFunctions.GetProtocols(false);
                if (ComboboxProtocol.Items.Count > 0)
                    ComboboxProtocol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetProtocols", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Protocols", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }            
        }

        void GetVendorDevices()
        {
            try
            {
                m_vendorDeviceList = CommonFunctions.GetVendorDevices(true);
                ComboboxPDCVendor.ItemsSource = m_vendorDeviceList;

                if (ComboboxPDCVendor.Items.Count > 0)
                    ComboboxPDCVendor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetVendorDevices", ex);
                SystemMessages sm = new SystemMessages(new openPDCManager.Utilities.Message() { UserMessage = "Failed to Retrieve Vendor Devices", SystemMessage = ex.Message, UserMessageType = openPDCManager.Utilities.MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }            
        }

#endregion
    }
}
