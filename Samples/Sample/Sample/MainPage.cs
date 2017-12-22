﻿using Plugin.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace Sample
{
    class MainPage : ContentPage
    {
        static readonly string SEP = new string('-', 30);

        ScrollView root;
        StackLayout layout;

        Button btnPermission, btnSetBadge, btnClearBadge,
            btnTestNotifDefIcon, btnTestNotifCustomIcon,
            btnMultipleMsgs, btnCancelAll, btnVibrate,
            btnTestNotifMetadata;

        Label lblTestNotif, lblBadges, lblOthers,
            lblPermission;

        private void InitComponents()
        {
            // init components
            root = new ScrollView();
            Thickness pagePadding;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    pagePadding = new Thickness(0,20,0,0);
                    break;
                default:
                    pagePadding = new Thickness();
                    break;
            }
            this.Padding = pagePadding;


            var labelMargin = new Thickness(10, 5);

            lblPermission = new Label
            {
                Text = "If you're on iOS, you must request permission before starting",
                Margin = labelMargin
            };
            btnPermission = new Button
            {
                Text = "Request Permission",
                Command = new Command(RequestPermission)
            };
            lblBadges = new Label
            {
                Text = "Set and clear Badge numbers",
                Margin = labelMargin
            };
            btnSetBadge = new Button
            {
                Text = "Set Badge",
                Command = new Command(SetBadge)
            };
            btnClearBadge = new Button
            {
                Text = "Clear Badge",
                Command = new Command(ClearBadge)
            };
            lblTestNotif = new Label
            {
                Text = "Send notifications, Press the buttons below and exit App withing 10 seconds:",
                Margin = labelMargin
            };
            btnTestNotifDefIcon = new Button
            {
                Text = "Notification w/ default icon",
                Command = new Command(TestNotifDefIcon)
            };
            btnTestNotifCustomIcon = new Button
            {
                Text = "Notification w/ custom icon",
                Command = new Command(TestNotifCustomIcon)
            };
            btnTestNotifMetadata = new Button
            {
                Text = "Notification w/ metadata",
                Command = new Command(TestNotifMetadata)
            };
            lblOthers = new Label
            {
                Text = SEP + "  Others  " + SEP,
                HorizontalOptions = LayoutOptions.Center
            };
            btnMultipleMsgs = new Button
            {
                Text = "Multiple Timed Messages (10 messages x 5 seconds apart)",
                Command = new Command(TestMultipleMsgs)
            };
            btnCancelAll = new Button
            {
                Text = "Cancel All Notifications",
                Command = new Command(CancelAll)
            };
            btnVibrate = new Button
            {
                Text = "Vibrate",
                Command = new Command(Vibrate)
            };

            // set layout
            layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,

                Children =
                {
                    lblPermission,
                    btnPermission,
                    lblBadges,
                    btnSetBadge,
                    btnClearBadge,
                    lblTestNotif,
                    btnTestNotifDefIcon,
                    btnTestNotifCustomIcon,
                    btnTestNotifMetadata,
                    lblOthers,
                    btnMultipleMsgs,
                    btnCancelAll,
                    btnVibrate
                }
            };
            root.Content = layout;

            // set content to root
            this.Content = root;
        }


        public MainPage()
        {
            InitComponents();
            Title = "Notifications";
            Notification.DefaultTitle = "Test Title";
            //CrossNotifications.Current.Activated += Notification_Activated;
        }

        private void Notification_Activated(object sender, Notification e)
        {
            Debug.WriteLine("Notification receved! Id: {0}, SendTime: {1}, Metadata count: {2}", e.Id, e.SendTime, e.Metadata.Count);

            if (e.Metadata.Count > 0)
            {
                var metaContent = e.Metadata.Select(p => $"[{p.Key}] = {p.Value}");
                var msg = string.Join("\n", metaContent);

                DisplayAlert("Notif. Metadata", msg, "ok");
            }
        }


        async void RequestPermission()
        {
            var result = await CrossNotifications.Current.RequestPermission();
            btnPermission.Text = result ? "Permission Granted" : "Permission Denied";
        }

        void SetBadge()
        {
            CrossNotifications.Current.SetBadge(new Random().Next(100));
        }

        void ClearBadge()
        {
            CrossNotifications.Current.SetBadge(0);
        }

        void TestNotifDefIcon()
        {
            CrossNotifications.Current.Send(new Notification
            {
                Title = "HELLO!",
                Message = "Hello from the ACR Sample Notification App, you should see the App's icon displayed",
                Vibrate = true,
                When = TimeSpan.FromSeconds(10),
            });
        }

        void TestNotifCustomIcon()
        {
            CrossNotifications.Current.Send(new Notification
            {
                Title = "HELLO!",
                Message = "Hello from the ACR Sample Notification App, you should see a custom icon displayed",
                Vibrate = true,
                When = TimeSpan.FromSeconds(10),
                IconName = "ic_addcard"
            });
        }

        private void TestNotifMetadata()
        {
            var notif = new Notification
            {
                Title = "METADATA!",
                When = TimeSpan.FromSeconds(10),
                Vibrate = true,
                IconName = "ic_addcard"
            };

            notif.SetMetadata("Val_PI", Math.PI.ToString());
            notif.Metadata.Add("SomeKey", "Anything you want!");

            //notif.Message = $"Every notification can have some metadata attatched, like the value of PI: {notif.Metadata["Val_PI"]}";
            notif.Message = $"The value of PI: {notif.Metadata["Val_PI"]}";

            CrossNotifications.Current.Send(notif);
        }

        void TestMultipleMsgs()
        {
            CrossNotifications.Current.Send(new Notification
            {
                Title = "Samples",
                Message = "Starting Sample Schedule Notifications",
                IconName = "ic_addcard"
            });

            for (var i = 1; i < 11; i++)
            {
                var seconds = i * 5;
                var id = CrossNotifications.Current.Send(new Notification
                {
                    Message = $"Message {i}",
                    When = TimeSpan.FromSeconds(seconds)
                });

                //Debug.WriteLine($"Notification ID: {id}");
            }
        }

        void CancelAll()
        {
            CrossNotifications.Current.CancelAll();
        }

        void Vibrate()
        {
            CrossNotifications.Current.Vibrate();
        }
    }
}
