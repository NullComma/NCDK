using EnigmaCore.DependecyInjection;

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnigmaCore.UI
{
    public class ViewManager
    {
        /// <summary>
        /// Gets the View that is currently at the top of the navigation stack.
        /// Returns null if no views are open.
        /// </summary>
        public View CurrentTopView => _viewStack.TryPeek(out var view) ? view : null;

        [Inject] [NonSerialized] protected PauseManager _pauseManager;
        [Inject] [NonSerialized] protected CBlockingEventsManager _blockingEventsManager;
        [NonSerialized] Stack<View> _viewStack = new();

        public void NotifyViewOpened(View openedView)
        {
            // If there's already a view on top, hide it.
            var currentTopView = CurrentTopView;
            if (currentTopView != openedView)
            {
                if (currentTopView != null)
                {
                    currentTopView.Hide();
                }
            }
            _viewStack.Push(openedView);
            
            _blockingEventsManager.MenuRetainable.Retain(openedView);

            // Handle game pausing
            if (openedView.ShouldPauseTheGame)
            {
                _pauseManager.Retain(openedView);
            }
            Debug.Log($"View opened: {openedView.CGetNameSafe()}");
        }
        
        public void NotifyViewShown(View shownView)
        {
          
            Debug.Log($"View shown: {shownView.CGetNameSafe()}");
        }

        public void NotifyViewHidden(View hiddenView)
        {
            Debug.Log($"View Hidden: {hiddenView.CGetNameSafe()}");

        }

        public void NotifyViewClosed(View closedView)
        {
            // Only proceed if the view being closed is the one on top of the stack.
            if (_viewStack.Peek() != closedView) return;

            _viewStack.Pop();
            _blockingEventsManager?.MenuRetainable.Release(closedView);

            // Handle game unpausing
            if (closedView.ShouldPauseTheGame)
            {
                _pauseManager.Release(closedView);
            }

            // Show the next view in the stack, if one exists.
            if (_viewStack.TryPeek(out View previousView))
            {
                previousView.Show();
            }
            Debug.Log($"View closed: {closedView.CGetNameSafe()}");
        }

        public void CloseAllViews()
        {
            // Keep closing the top-most view until the stack is empty.
            while (_viewStack.Count > 0)
            {
                // Get the view at the top of the stack without removing it yet.
                View topView = _viewStack.Peek();
                
                // Tell the view to close itself (gameObject.SetActive(false)).
                // This will trigger its OnDisable, which then calls NotifyViewClosed,
                // which safely pops it from the stack and shows the next one.
                // This creates a clean, cascading close effect.
                topView.Close();
            }
            Debug.Log("All views have been closed.");
        }
    }
}