using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AF
{
    /// <summary>
    /// This class fixes several unity bugs with the scroll view GUI and should be used in scrollable elements (namely lists) 
    /// </summary>
    public class UIDocumentScrollerBase : UIDocumentBase
    {
        Focusable currentRelatedTarget;

        // Use this for initialization
        protected virtual void Start()
        {
            base.Start();

            var scrollView = this.root.Q<ScrollView>();

            var scrollViewChildren = scrollView.Children();

            // Make the scroll bar move with the focused element
            foreach (var child in scrollViewChildren)
            {
                child.RegisterCallback<FocusEvent>(ev =>
                {
                    this.currentRelatedTarget = ev.relatedTarget;

                    scrollView.ScrollTo(child);
                });
            }

            // If last item is focused and we press down, go up the list to the first element
            scrollView.RegisterCallback<KeyDownEvent>(ev =>
            {
                if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S)
                {
                    if (scrollViewChildren.Count() > 0 && this.currentRelatedTarget == scrollViewChildren.Last())
                    {
                        var firstElementOnTheList = scrollViewChildren.First();

                        scrollView.ScrollTo(firstElementOnTheList);
                        firstElementOnTheList.Focus();
                    }
                }
            });
        }

    }
}
