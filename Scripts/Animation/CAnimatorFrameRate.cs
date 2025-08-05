using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace EnigmaCore {
    [RequireComponent(typeof(Animator))]
    public class CAnimatorFrameRate : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField][Min(1)] 
        int _delayFrames = 3;
        
        float _lastUpdateTime;
        Animator _animator;

        #endregion <<---------- Properties and Fields ---------->>


        

        #region <<---------- Mono Behaviour ---------->>

        void Awake()
        {
            TryGetComponent(out _animator);
        }

        IEnumerator Start()
        {
            yield return new WaitWhile(() => !_animator.playableGraph.IsValid());
            _animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
        }

        private void Update() {
            _animator.Update(0);
            if (Time.frameCount % ((int)_delayFrames) != 0) return;
            _animator.Update(Time.realtimeSinceStartup - _lastUpdateTime);
            if(_animator.playableGraph.IsValid()) _animator.playableGraph.Evaluate(Time.realtimeSinceStartup - _lastUpdateTime);
            _lastUpdateTime = Time.realtimeSinceStartup;
        }

        #endregion <<---------- Mono Behaviour ---------->>

    }
}