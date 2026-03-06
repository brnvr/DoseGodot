using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Dose.Core.Library.UI
{
    public partial class NotificationCenter(IUI ui) : Node, IEnumerable<Notification>, IUIComponent
    {
        readonly List<Notification> _notifications = [];

        public IUI UI { get; } = ui;

        public void Add(Notification notification)
        {
            _notifications.Add(notification);

            notification.Expired += (e, args) =>
            {
                _notifications.Remove(notification);
                OnUIEvent(UI);
            };

            OnUIEvent(UI);
        }

        public IEnumerator<Notification> GetEnumerator()
        {
            return _notifications.GetEnumerator();
        }

        public void OnUIEvent(IUI ui)
        {
            var notificationsStr = string.Join("\n", _notifications.Select(n => n.Message));

            ui.SetLabelValue("Notifications", notificationsStr);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}