using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ChatboxVido.Models;
using MvvmHelpers;
using Newtonsoft.Json;
using ChatboxVido.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using ChatboxVido;
using ChatboxVido.Core;
using Prism.Mvvm;
using System.Web.Script.Serialization;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using static ChatboxVido.Models.ProfileZalo;

namespace ChatboxVido.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        public RelayCommand SendCommand { get; set; }
        public CallZaloAPI callZalo = new CallZaloAPI();
        public ObservableCollection<CrispMessage> CrispMessages { get; set; }
        public ObservableCollection<ContactCrisp> CrispContacts { get; set; }

        public ObservableCollection<MessageZalo> ZaloMessages { get; set; }

        private ContactCrisp _selectContact;
        public ContactCrisp SelectContact
        {
            get { return _selectContact; }
            set
            {
                _selectContact = value;
                OnPropertyChanged();
                getMessage();
                if (_selectContact.SenderId != null)
                {
                    getZaloMessage();
                }
            }

        }
        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            CrispMessages = new ObservableCollection<CrispMessage>();
            ZaloMessages = new ObservableCollection<MessageZalo>();
            CrispContacts = new ObservableCollection<ContactCrisp>();

            callZalo.RefeshToken();
            getContactList();
            sendMessageCrisp();
            sendMessageZalo();
        }

        public void getMessage()
        {
            if (_selectContact != null)
            {
                callZalo.GetApplicationConfig();
                var vidoCrispMessageList = DataProvider.Instance.DB.usp_GetCrispMessagesForChatApplication(_selectContact.ContactId);
                if (CrispMessages.Count() > 0)
                {
                    CrispMessages.Clear();
                }
                foreach (var messages in vidoCrispMessageList)
                {
                    if (messages.Event == "user" || messages.Event == "message:send")
                    {
                        CrispMessages.Add(new CrispMessage
                        {
                            Nickname = messages.ContactName,
                            Content = messages.Content,
                            Event = messages.Event,
                            Timestamp = messages.Timestamp,
                            ImageSource = "	https://www.freeiconspng.com/uploads/person-icon-blue-18.png",
                            FirstMessage = true
                        });
                    }
                    else if (messages.Event == "message:received" || messages.Event == "operator")
                    {
                        CrispMessages.Add(new CrispMessage
                        {
                            Nickname = messages.ContactName,
                            Content = messages.Content,
                            Timestamp = messages.Timestamp,
                            Event = messages.Event,
                            ImageSource = "	https://www.freeiconspng.com/uploads/person-icon-blue-18.png",
                            FirstMessage = false
                        });
                    }
                    //call.SendMessageCrisp(messages.Content, _selectContact.Website_Id, _selectContact.Session_Id, "2d59a230-cc9d-49ab-9024-9b6551e4d17c", "5fa1184f5a60db46fc592f4bc90ed9ec015f384b09095b775e72614e18cdc66c");
                }

            }
        }
        public void getZaloMessage()
        {

            if (_selectContact != null)
            {
                string vidoZaloList = callZalo.getMessageHistory(long.Parse(_selectContact.SenderId)).ToString();
                GetMessageHistory routes_list =
                       JsonConvert.DeserializeObject<GetMessageHistory>(vidoZaloList);
                routes_list.data.Reverse();
                if (ZaloMessages.Count() > 0)
                {
                    ZaloMessages.Clear();
                }

                foreach (var zalo in routes_list.data)
                {
                    if (zalo.src == 0)
                    {
                        CrispMessages.Add(new CrispMessage
                        {
                            Nickname = zalo.to_display_name,
                            Content = zalo.message,
                            Timestamp = zalo.time,
                            ImageSource = "https://noithatlinhngan.vn/wp-content/uploads/2019/12/icon-zalo.png",
                            FirstMessage = false
                        });
                    }
                    else
                    {
                        CrispMessages.Add(new CrispMessage
                        {
                            Nickname = zalo.from_display_name,
                            Content = zalo.message,
                            Timestamp = zalo.time,
                            ImageSource = "https://noithatlinhngan.vn/wp-content/uploads/2019/12/icon-zalo.png",
                            FirstMessage = true
                        });
                    }
                }
            }
        }
        public void getContactList()
        {
            callZalo.GetApplicationConfig();
       
            var vidoContactList = DataProvider.Instance.DB.usp_GetContactListForChatApplication();        
            foreach (var contact in vidoContactList)
            {
                if (contact.Source == "ZALO")
                {
                    var user = callZalo.getProfile(contact.ContactName);
                    if(user.error == 0)
                    {
                        CrispContacts.Add(new ContactCrisp
                        {
                            Username = "[" + contact.Source + "]" + user.data.display_name,
                            SenderId = contact.ContactName,
                            AppId = contact.ServerID,
                            ImageSource = user.data.avatar,
                            LastMessage = contact.LastMessageReceive
                        });

                    }                    
                }
                else if (contact.Source == "CRISP")
                {
                    CrispContacts.Add(new ContactCrisp
                    {
                        Username = "[" + contact.Source + "]" + contact.ContactName,
                        ContactId = contact.ContactID,
                        Session_Id = contact.ContactID,
                        EventName = contact.Eventname,
                        Website_Id = contact.ServerID,
                        ImageSource = "https://crisp.chat/favicon-512x512.png",
                        LastMessage = contact.LastMessageReceive
                    });
                }

            }
        }
        public void sendMessageCrisp()
        {
            callZalo.GetApplicationConfig();
            CallCrispApi sendmessage = new CallCrispApi();

            SendCommand = new RelayCommand(o =>
            {
                CrispMessages.Add(new CrispMessage
                {
                    Content = Message,
                    FirstMessage = false
                });
                sendmessage.SendMessageCrisp(Message, _selectContact.Website_Id, _selectContact.Session_Id, "2d59a230-cc9d-49ab-9024-9b6551e4d17c", "5fa1184f5a60db46fc592f4bc90ed9ec015f384b09095b775e72614e18cdc66c");
                Message = "";
            });
        }
        public void sendMessageZalo()
        {
            callZalo.GetApplicationConfig();
            SendCommand = new RelayCommand(o =>
            {
                callZalo.SendMessage(_selectContact.SenderId, Message);
                CrispMessages.Add(new CrispMessage
                {
                    Content = Message,
                    FirstMessage = false
                });
                Message = "";
            });
        }
    }
}
