﻿using Alika.Libs.VK.Responses;
using Microsoft.Toolkit.Uwp.UI.Triggers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Alika.UI
{
    public class ChatsList : ListView
    {
        public ListView chats = new ListView();
        public ChatsList()
        {
            this.LoadChats(0);
        }

        public async void LoadChats(int offset, int count = 50, int start_msg_id = 0)
        {
            await Task.Factory.StartNew(() =>
              {
                  var conversations = App.vk.Messages.GetConversations(count: count, offset: offset, fields: "photo_200,online_info", start_message_id: start_msg_id).conversations;
                  List<ListViewItem> items = new List<ListViewItem>();
                  foreach(GetConversationsResponse.ConversationResponse conv in conversations)
                  {
                      App.UILoop.AddAction(new UITask
                      {
                          Action = () => {
                              if (conv.conversation.peer.id > 2000000000)
                              {
                                  this.Items.Add(new ChatItem(
                                      peer_id: conv.conversation.peer.id,
                                      avatar: conv.conversation.settings.photos?.photo_200,
                                      name: conv.conversation.settings.title,
                                      last_msg: conv.last_message
                                  ));
                              }
                              else if (conv.conversation.peer.id < 0)
                              {
                                  var group = App.cache.GetGroup(conv.conversation.peer.id);
                                  this.Items.Add(new ChatItem(
                                       peer_id: conv.conversation.peer.id,
                                       avatar: group.photo_200,
                                       name: group.name,
                                       last_msg: conv.last_message
                                   ));

                              }
                              else
                              {
                                  var user = App.cache.GetUser(conv.conversation.peer.id);
                                  this.Items.Add(new ChatItem(
                                       peer_id: conv.conversation.peer.id,
                                       avatar: user.photo_200,
                                       name: user.first_name + " " + user.last_name,
                                       last_msg: conv.last_message
                                   ));
                              }
                          },
                          Priority = CoreDispatcherPriority.High
                      });
                  }
              });
        }

        public async void ProcessUpdates(JToken updates)
        {
            await Task.Factory.StartNew(async () =>
              {
                  await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                  {
                      foreach (JToken update in updates)
                      {
                          if ((int)update[0] == 4)
                          {
                              Message msg = new Message(update);
                              foreach (var item in this.Items)
                              {
                                  if (item as ChatItem != null)
                                  {
                                      ChatItem chat = item as ChatItem;
                                      if (chat.peer_id == msg.peer_id)
                                      {
                                          bool first = (this.Items[0] as ChatItem).peer_id == msg.peer_id;
                                          if (!first) try { this.Items.Remove(chat); } catch { }
                                          chat.UpdateMsg(msg);
                                          if (!first) this.Items.Insert(0, chat);
                                          if (this.SelectedIndex != -1)
                                          {
                                              if ((this.SelectedItem as ChatItem).peer_id == msg.peer_id)
                                              {
                                                  // TODO: Scroll to top?
                                              }
                                          }
                                          return;
                                      }
                                  }
                              }
                          };
                      }
                  });
              });
        }
    }
}
