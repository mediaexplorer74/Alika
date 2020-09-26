﻿using Alika.Libs;
using Alika.Libs.VK.Responses;
using Alika.UI.Misc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Alika.UI
{
    /// <summary>
    /// Message box which holds MessageGrid (needed for future features)
    /// </summary>
    [Windows.UI.Xaml.Data.Bindable]
    public class MessageBox : ContentControl
    {
        public MessageGrid message { get; set; }

        public MessageBox(Message msg, int peer_id)
        {
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.HorizontalContentAlignment = msg.from_id == App.vk.user_id ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            this.message = new MessageGrid(msg, peer_id);
            this.Content = (this.message);
        }

        /// <summary>
        /// Grid with avatar & text
        /// </summary>
        [Windows.UI.Xaml.Data.Bindable]
        public class MessageGrid : Grid
        {
            public TextBubble textBubble { get; set; }
            public Avatar avatar;
            public TextBlock time = new TextBlock { 
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = Coloring.InvertedTransparent.Percent(50)
            };

            public MessageGrid(Message msg, int peer_id)
            {
                this.MinWidth = 200;
                this.MaxWidth = 700;

                this.textBubble = new TextBubble(msg, peer_id);

                this.LoadAvatar(msg.from_id);
                var date = msg.date.ToDateTime();
                if(date.Date != DateTime.Today.Date)
                {
                    if(date.Year != DateTime.Today.Year)
                    {
                        this.time.Text = date.ToString("HH:mm, dd.MM.yy");
                    } else this.time.Text = date.ToString("HH:mm, dd.MM");
                }
                else this.time.Text = date.ToString("HH:mm");

                // Changing avatar position if message from current user
                if (msg.from_id == App.vk.user_id)
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    this.ColumnDefinitions.Add(new ColumnDefinition());
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

                    var state = new StackPanel { 
                        VerticalAlignment = VerticalAlignment.Bottom
                    };
                    state.Children.Add(new Image
                    {
                        Width = 15,
                        Height = 15,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Source = new SvgImageSource(new Uri(Utils.AssetTheme(msg.id <= App.cache.GetConversation(msg.peer_id).out_read ? "double_check.svg" : "check.svg")))
                    }); ;
                    state.Children.Add(this.time);

                    Grid.SetColumn(state, 0);
                    Grid.SetColumn(this.textBubble, 1);
                    Grid.SetColumn(this.avatar, 2);

                    this.Children.Add(state);
                }
                else
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                    this.ColumnDefinitions.Add(new ColumnDefinition());
                    this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    Grid.SetColumn(this.avatar, 0);
                    Grid.SetColumn(this.textBubble, 1);
                    Grid.SetColumn(this.time, 2);
                    this.Children.Add(this.time);
                }

                this.Children.Add(this.textBubble);
                this.Children.Add(this.avatar);

                this.RightTapped += (a, b) => new MessageFlyout(msg).ShowAt(this, b.GetPosition(b.OriginalSource as UIElement));
            }

            public void LoadAvatar(int user_id)
            {
                this.avatar = new Avatar(user_id)
                {
                    Height = 40,
                    Width = 40,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Thickness margin = this.avatar.Margin;
                margin.Bottom = this.textBubble.border.Margin.Bottom + (this.textBubble.text.Margin.Bottom / 2);
                this.avatar.Margin = margin;
            }
        }

        /// <summary>
        /// Name,text, attachments & buttons holder
        /// </summary>
        [Windows.UI.Xaml.Data.Bindable]
        public class TextBubble : StackPanel
        {
            public Message message { get; set; }
            public Border border = new Border
            {
                BorderThickness = new Thickness(1),
                Background = App.systemDarkTheme ? Coloring.MessageBox.TextBubble.Dark : Coloring.MessageBox.TextBubble.Light,
                Margin = new Thickness(10, 5, 10, 5),
                MinHeight = 50,
                MinWidth = 50,
                CornerRadius = new CornerRadius(10)
            };
            public RichTextBlock text = new RichTextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                ContextFlyout = null
            };
            public TextBlock name = new TextBlock
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(12, 5, 12, 1)
            };
            public Grid textGrid = new Grid();
            public Grid attachGrid = new Grid();
            public Grid keyboardGrid = new Grid();
            public StackPanel borderContent = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            public int peer_id;

            public TextBubble(Message msg, int peer_id)
            {
                this.message = msg;
                this.peer_id = peer_id;
                this.LoadText();
                this.LoadName();
                this.textGrid.Children.Add(this.text);

                if (this.message.reply_message != null) this.borderContent.Children.Add(new Dialog.Dialog.ReplyMessage(this.message.reply_message)
                {
                    CrossEnabled = false,
                    LineWidth = 1
                });
                if (this.message.text != null && this.message.text.Length > 0) this.borderContent.Children.Add(this.textGrid); // TODO: make service messages support
                if (this.message.attachments.Count > 0)
                {
                    this.borderContent.Children.Add(this.attachGrid);
                    this.LoadAttachments();
                }
                if (this.message.keyboard != null && this.message.keyboard.buttons.Count > 0)
                {
                    this.borderContent.Children.Add(this.keyboardGrid);
                    this.LoadButtons();
                }
                this.border.Child = this.borderContent;

                this.Children.Add(this.name);
                this.Children.Add(this.border);
            }

            public void LoadText()
            {
                string text = this.message.text;
                if (text != null)
                {
                    this.text.Blocks.Clear();
                    List<Match> markdown = new List<Match>();

                    MatchCollection pushes = new Regex(@"\[(id|club)\d+\|[^\]]*]").Matches(text);
                    MatchCollection links = new Regex(@"((http|https)\:\/\/)?[\w]*\.[a-zA-Z]{1,6}(\/[\w\?\-\=\&.]*)*").Matches(text);
                    if (pushes.Count > 0) foreach (Match push in pushes) markdown.Add(push);
                    if (links.Count > 0) foreach (Match link in links) markdown.Add(link);

                    Paragraph p = new Paragraph();
                    if (markdown.Count > 0)
                    {
                        for (int i = 0; i < markdown.Count; i++)
                        {
                            Match m = markdown[i];
                            if (i == 0 && !text.StartsWith(m.Value)) p.Inlines.Add(new Run { Text = text.Substring(0, m.Index) });
                            if (i > 0 && markdown[i - 1].Index != m.Index)
                            {
                                try
                                {
                                    Match prev = markdown[i - 1];
                                    int start = prev.Index + prev.Length;
                                    p.Inlines.Add(new Run { Text = text.Substring(start, m.Index - start) });
                                }
                                catch { }
                            }
                            // TODO: Fix crash on some messagesъtry
                            try
                            {
                                if (!m.Value.Contains("["))
                                {
                                    string lnk = m.Value;
                                    if (!lnk.StartsWith("http://") && !lnk.StartsWith("https://")) lnk = "http://" + lnk;
                                    Hyperlink link = new Hyperlink { NavigateUri = new Uri(lnk) };
                                    link.Inlines.Add(new Run { Text = m.Value });
                                    p.Inlines.Add(link);

                                }
                                else
                                {
                                    Hyperlink link = new Hyperlink { NavigateUri = new Uri("https://vk.com/" + m.Value.Split("|")[0].Replace("[", "")) };
                                    link.Inlines.Add(new Run { Text = m.Value.Split("|")[1].Replace("]", "") });
                                    p.Inlines.Add(link);
                                }
                            }
                            catch { }
                            if (i == markdown.Count - 1 && !text.EndsWith(m.Value)) p.Inlines.Add(new Run { Text = text.Substring(m.Index + m.Length) });
                        }
                    }
                    else p.Inlines.Add(new Run { Text = text });
                    this.text.Blocks.Add(p);
                }
            }

            public void LoadName()
            {
                if (this.message.from_id > 0)
                {
                    if (!App.cache.Users.Exists(u => u.user_id == this.message.from_id)) App.vk.Users.Get(new List<int> { this.message.from_id }, fields: "photo_200");
                    User user = App.cache.Users.Find(u => u.user_id == this.message.from_id);
                    this.name.Text = user.first_name + " " + user.last_name;
                }
                else
                {
                    if (!App.cache.Groups.Exists(g => g.id == this.message.from_id)) App.vk.Groups.GetById(new List<int> { this.message.from_id }, fields: "photo_200");
                    this.name.Text = App.cache.Groups.Find(g => g.id == this.message.from_id).name;
                }
                if (this.message.from_id == App.vk.user_id) this.name.HorizontalTextAlignment = TextAlignment.Right;
            }

            public void LoadAttachments()
            {
                if (this.message.attachments != null && this.message.attachments.Count > 0)
                {
                    this.message.attachments.ForEach((Attachment att) =>
                    {
                        FrameworkElement attach = null;
                        switch (att.type)
                        {
                            case "photo":
                                attach = new MessageAttachment.Photo(att.photo)
                                {
                                    Width = this.Width
                                };
                                break;
                            case "sticker":
                                attach = new MessageAttachment.Sticker(att.sticker);
                                this.border.Background = Coloring.Transparent.Full;
                                break;
                            case "doc":
                                attach = new MessageAttachment.Document(att.document);
                                break;
                            case "audio_message":
                                attach = new MessageAttachment.AudioMessage(att.audio_message);
                                break;
                            case "graffiti":
                                attach = new MessageAttachment.Graffiti(att.graffiti);
                                this.border.Background = Coloring.Transparent.Full;
                                break;
                        }
                        if (attach != null)
                        {
                            attach.Loaded += (a, b) => this.Height += attach.Height;
                            Grid.SetRow(attach, this.attachGrid.RowDefinitions.Count);
                            this.attachGrid.RowDefinitions.Add(new RowDefinition());
                            this.attachGrid.Children.Add(attach);
                        }
                    });
                }
            }

            public void LoadButtons() // TODO: Support for callback buttons?
            {
                if (this.message.keyboard != null && this.message.keyboard.inline)
                {
                    this.message.keyboard.buttons.ForEach((List<Message.Keyboard.Button> buttons) =>
                    {
                        Grid grid = new Grid();
                        Grid.SetRow(grid, this.keyboardGrid.RowDefinitions.Count);
                        this.keyboardGrid.RowDefinitions.Add(new RowDefinition());
                        buttons.ForEach((Message.Keyboard.Button button) =>
                        {
                            Button btn = new Button
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Margin = new Thickness(5),
                                CornerRadius = this.border.CornerRadius
                            };
                            if (button.color != "default") btn.Background = new SolidColorBrush(Coloring.FromHash(Coloring.MessageBox.Keyboard.GetColor(button.color)));
                            btn.Content = new TextBlock
                            {
                                Text = button.action.label,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            btn.Click += (object sender, RoutedEventArgs e) => Task.Factory.StartNew(() => App.vk.Messages.Send(this.peer_id, text: button.action.label, payload: button.action.payload));
                            Grid.SetColumn(btn, grid.ColumnDefinitions.Count);
                            grid.ColumnDefinitions.Add(new ColumnDefinition());
                            grid.Children.Add(btn);
                        });
                        this.keyboardGrid.Children.Add(grid);
                    });
                }
            }
        }

        public class MessageFlyout : MenuFlyout
        {
            public MessageFlyout(Message msg)
            {
                var reply = new MenuFlyoutItem
                {
                    Icon = new FontIcon
                    {
                        Glyph = "\uE8CA"
                    },
                    Text = Utils.LocString("Dialog/Reply")
                };
                reply.Click += (a, b) =>
                {
                    var r = (App.main_page.dialog.Children[0] as Dialog.Dialog).reply_grid;
                    if (r.Content is Dialog.Dialog.ReplyMessage prev && prev.Message.id == msg.id) return;
                    r.Content = new Dialog.Dialog.ReplyMessage(msg);
                };
                this.Items.Add(reply);
            }
        }
    }
}
