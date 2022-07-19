using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentNotificationUI : UIDocumentBase
    {
        public List<string> notificationInstances = new List<string>();

        VisualElement notificationsPanelContainer;

        protected override void Start()
        {
            base.Start();

            this.notificationsPanelContainer = this.root.Q<VisualElement>("NotificationContainer");

            this.Disable();
        }

        public void Refresh()
        {
            notificationsPanelContainer.Clear();

            foreach (string notification in notificationInstances)
            {
                Label label = new Label();
                label.text = notification;
                label.AddToClassList("notification-label");
                notificationsPanelContainer.Add(label);
            }

            this.Enable();
        }
    }

}
