﻿//******************************************************************************************************
//  OutputStreamsUserControl.cs - Gbtc
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
//  07/29/2010 - Mehulbhai P Thakkar
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Windows;
using openPDCManager.ModalDialogs;
using openPDCManager.Utilities;
using openPDCManager.Data;
using openPDCManager.Data.Entities;
using openPDCManager.Data.ServiceCommunication;

namespace openPDCManager.UserControls.OutputStreamControls
{
    public partial class OutputStreamsUserControl
    {
        #region [ Members ]

        WindowsServiceClient serviceClient;

        #endregion

        #region [ Methods ]

        void Initialize()
        {            
            serviceClient = ((App)Application.Current).ServiceClient;         
        }

        void SendInitialize()
        {
            SystemMessages sm;
            try
            {
                if (serviceClient.Helper.RemotingClient.CurrentState == TVA.Communication.ClientState.Connected)
                {
                    string result = CommonFunctions.SendCommandToWindowsService(serviceClient, "Initialize " + Convert.ToInt32(TextBlockRuntimeID.Text));
                    sm = new SystemMessages(new Message() { UserMessage = result, SystemMessage = "", UserMessageType = MessageType.Success }, ButtonType.OkOnly);
                }
                else
                    sm = new SystemMessages(new Message() { UserMessage = "Application is disconnected", SystemMessage = "Connection String: " + ((App)Application.Current).RemoteStatusServiceUrl, UserMessageType = MessageType.Error }, ButtonType.OkOnly);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.SendInitialize", ex);
                sm = new SystemMessages(new Message() { UserMessage = "Failed to Send Initialize Command", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);                
            }
            sm.Owner = Window.GetWindow(this);
            sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sm.ShowPopup();
        }

        void SendUpdateConfiguration(int outputStreamID)
        {
            SystemMessages sm;
            try
            {
                string runtimeID = CommonFunctions.GetRuntimeID("OutputStream", outputStreamID);
                if (serviceClient.Helper.RemotingClient.CurrentState == TVA.Communication.ClientState.Connected)
                {
                    string result = CommonFunctions.SendCommandToWindowsService(serviceClient, "Invoke " + runtimeID + " UpdateConfiguration");
                    sm = new SystemMessages(new Message() { UserMessage = result, SystemMessage = "", UserMessageType = MessageType.Success }, ButtonType.OkOnly);
                }
                else
                    sm = new SystemMessages(new Message() { UserMessage = "Application is disconnected", SystemMessage = "Connection String: " + ((App)Application.Current).RemoteStatusServiceUrl, UserMessageType = MessageType.Error }, ButtonType.OkOnly);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.SendUpdateConfiguration", ex);
                sm = new SystemMessages(new Message() { UserMessage = "Failed to Update Configuration", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);
            }
            sm.Owner = Window.GetWindow(this);
            sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sm.ShowPopup();
        }

        void DisplayRuntimeID()
        {
            TextBlockRuntimeID.Text = CommonFunctions.GetRuntimeID("OutputStream", m_outputStreamID);
        }

        void GetNodes()
        {
            try
            {
                ComboBoxNode.ItemsSource = CommonFunctions.GetNodes(true, false);
                if (ComboBoxNode.Items.Count > 0)
                    ComboBoxNode.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetNodes", ex);
                SystemMessages sm = new SystemMessages(new Message() { UserMessage = "Failed to Retrieve Nodes", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }            
        }

        void GetOutputStreamList()
        {
            try
            {
                ListBoxOutputStreamList.ItemsSource = CommonFunctions.GetOutputStreamList(false, m_nodeValue);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.GetOutputStreamList", ex);
                SystemMessages sm = new SystemMessages(new Message() { UserMessage = "Failed to Retrieve Output Stream List", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);
                sm.Owner = Window.GetWindow(this);
                sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                sm.ShowPopup();
            }
        }

        void SaveOutputStream(OutputStream outputStream, bool isNew)
        {
            SystemMessages sm;
            try
            {
                string result = CommonFunctions.SaveOutputStream(outputStream, isNew);
                ClearForm();
                sm = new SystemMessages(new Message() { UserMessage = result, SystemMessage = string.Empty, UserMessageType = MessageType.Success },
                        ButtonType.OkOnly);

                //Update Metadata in the openPDC Service.
                try
                {                    
                        WindowsServiceClient serviceClient = ((App)Application.Current).ServiceClient;
                        if (serviceClient.Helper.RemotingClient.CurrentState == TVA.Communication.ClientState.Connected)                 
                            CommonFunctions.SendCommandToWindowsService(serviceClient, "ReloadConfig"); //we do this to make sure all statistical measurements are in the system.                 
                }
                catch (Exception ex)
                {
                    CommonFunctions.LogException("SaveOutputStream.RefreshMetadata", ex);
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.SaveOutputStream", ex);
                sm = new SystemMessages(new Message() { UserMessage = "Failed to Save Output Stream Information", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);
            }
            sm.Owner = Window.GetWindow(this);
            sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sm.ShowPopup();
            GetOutputStreamList();
        }

        void DeleteOutputStream(int outputStreamID)
        {
            SystemMessages sm;
            try
            {
                string result = CommonFunctions.DeleteOutputStream(outputStreamID);
                sm = new SystemMessages(new Message() { UserMessage = result, SystemMessage = string.Empty, UserMessageType = MessageType.Success },
                        ButtonType.OkOnly);
            }
            catch (Exception ex)
            {
                CommonFunctions.LogException("WPF.DeleteOutputStream", ex);
                sm = new SystemMessages(new Message() { UserMessage = "Failed to Delete Output Stream Information", SystemMessage = ex.Message, UserMessageType = MessageType.Error },
                        ButtonType.OkOnly);
            }
            sm.Owner = Window.GetWindow(this);
            sm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            sm.ShowPopup();
            GetOutputStreamList();
        }

        #endregion
    }
}
