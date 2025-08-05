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

        Stack<View> _viewStack = new();
        int _pauseRequestCount = 0;

        /// <summary>
        /// Called by a View from its OnEnable method.
        /// </summary>
        public void NotifyViewOpened(View openedView)
        {
            // If there's already a view on top, hide it.
            if (_viewStack.TryPeek(out View currentTopView))
            {
                if (currentTopView != openedView) // Avoid hiding itself
                {
                    currentTopView.gameObject.SetActive(false);
                }
            }
            
            // If the view is already in the stack, remove it before pushing to the top.
            // This handles re-opening an existing view.
            var list = new List<View>(_viewStack);
            list.Remove(openedView);
            list.Reverse();
            _viewStack = new Stack<View>(list);
            
            _viewStack.Push(openedView);
            
            // Handle game pausing
            if (openedView.ShouldPauseTheGame)
            {
                _pauseRequestCount++;
                if (_pauseRequestCount == 1) ETime.TimeScale = 0f;
            }
        }

        /// <summary>
        /// Called by a View from its OnDisable method.
        /// </summary>
        public void NotifyViewClosed(View closedView)
        {
            // Only proceed if the view being closed is the one on top of the stack.
            if (_viewStack.Count == 0 || _viewStack.Peek() != closedView) return;

            _viewStack.Pop();

            // Handle game unpausing
            if (closedView.ShouldPauseTheGame)
            {
                _pauseRequestCount--;
                if (_pauseRequestCount == 0) ETime.TimeScale = 1f;
            }

            // Show the next view in the stack, if one exists.
            if (_viewStack.TryPeek(out View previousView))
            {
                previousView.Show();
            }
        }
        
        /// <summary>
        /// Closes all currently open views in the stack until none are left.
        /// This will automatically handle unpausing the game.
        /// </summary>
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