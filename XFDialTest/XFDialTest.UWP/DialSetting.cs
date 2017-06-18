using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Xamarin.Forms;
using XFDialTest;
using XFDialTest.UWP;

[assembly: Dependency(typeof(DialSetting))]
namespace XFDialTest.UWP
{
    class DialSetting : IDialSetting
    {
        public void Setting(ObservableCollection<string> items)
        {
            DialSettings(items);
        }

        // Surface Dial の Controller インスタンス
        private RadialController _controller;

        /// <summary>
        /// Dial の操作を記録します
        /// </summary>
        /// <param name="items"></param>
        /// <param name="cont"></param>
        /// <param name="menu"></param>
        /// <param name="event"></param>
        /// <param name="arg"></param>
        private void AddItem(ObservableCollection<string> items, RadialControllerScreenContact cont, RadialControllerMenuItem menu, string @event, string arg)
        {
            items.Insert(0, $"{cont?.Position.ToString()} : {menu?.DisplayText} : {@event} : {arg}");
            if (1000 < items.Count) items.RemoveAt(1000);
        }

        /// <summary>
        /// Dial の操作を記録します
        /// </summary>
        /// <param name="items"></param>
        /// <param name="cont"></param>
        /// <param name="ctrl"></param>
        /// <param name="event"></param>
        /// <param name="arg"></param>
        private void AddItem(ObservableCollection<string> items, RadialControllerScreenContact cont, RadialController ctrl, string @event, string arg)
            => AddItem(items, cont, ctrl?.Menu?.GetSelectedMenuItem(), @event, arg);

        private void DialSettings(ObservableCollection<string> items)
        {
            // コントローラーを取得
            var controller = _controller = RadialController.CreateForCurrentView();

            {// アプリ独自メニューの設定
                var menuItems = new List<RadialControllerMenuItem>(){
                RadialControllerMenuItem.CreateFromKnownIcon("Item Name 1", RadialControllerMenuKnownIcon.InkColor),
                RadialControllerMenuItem.CreateFromKnownIcon("Item Name 2", RadialControllerMenuKnownIcon.PenType) };
                menuItems.ForEach(m => controller.Menu.Items.Add(m));
                // メニューが選択されたイベント
                menuItems.ForEach(m => m.Invoked += (sender, arg) => AddItem(items,
                    null,
                    sender,
                    nameof(RadialControllerMenuItem.Invoked),
                    arg?.ToString()
                    )
                );
            }
            {// 標準メニューの設定
             // ここでは音量とスクロールだけを設定している
                var configration = RadialControllerConfiguration.GetForCurrentView();
                //configration.SetDefaultMenuItems(Enumerable.Empty<RadialControllerSystemMenuItemKind>());
                configration.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, RadialControllerSystemMenuItemKind.Scroll });
            }

            // イベントハンドラの設定（イベントの記録）

            // 標準メニューを選択した際や
            // アプリがフォーカスを失った時にも発生
            controller.ControlLost += (sender, arg) => AddItem(items,
                null,
                sender,
                nameof(RadialController.ControlLost),
                arg?.ToString()
                );

            // Dial を回転したときに発生
            // arg.IsButtonPressed で押し込みながらの回転かを判別できる
            controller.RotationChanged += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.RotationChanged),
                $"{nameof(arg.IsButtonPressed)} : {arg.IsButtonPressed}, {nameof(arg.RotationDeltaInDegrees)} : {arg.RotationDeltaInDegrees}"
                );

            // Dial のクリック（押し込み）時に発生
            // クリック時、ButtonPressed ⇒ ButtonReleased ⇒ ButtonClicked と 3 つが続けて発生
            // 押し込みながら Dial 回転した後でも ButtonClicked が発生する
            controller.ButtonClicked += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ButtonClicked),
                arg?.ToString()
                );

            controller.ButtonHolding += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ButtonHolding),
                arg?.ToString()
                );

            // Dial を押し込んだ際に発生
            controller.ButtonPressed += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ButtonPressed),
                arg?.ToString()
            );

            // Dial を押し込んだ後、離した際に発生
            controller.ButtonReleased += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ButtonReleased),
                arg?.ToString()
                );

            // 標準メニュー選択状態から独自メニューを選択した際や
            // アプリがフォーカスを得た際にも発生
            controller.ControlAcquired += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ControlAcquired),
                arg?.ToString()
                );

            controller.ScreenContactContinued += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ScreenContactContinued),
                arg?.ToString()
                );

            controller.ScreenContactEnded += (sender, arg) => AddItem(items,
                null,
                sender,
                nameof(RadialController.ScreenContactEnded),
                arg?.ToString()
                );

            controller.ScreenContactStarted += (sender, arg) => AddItem(items,
                arg.Contact,
                sender,
                nameof(RadialController.ScreenContactStarted),
                $"{nameof(arg.IsButtonPressed)} : {arg?.IsButtonPressed}, {arg?.ToString()}"
                );
        }



    }
}
