using System;
using EnigmaCore;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class CodeAnimationController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, Tooltip("The Animator component that the graph will drive.")]
        #if ODIN_INSPECTOR
        [Required]
        #endif
        Animator animator;

        [Tooltip("Controls how the animation graph's time is updated.")]
        [SerializeField]
        DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime;

        #if ODIN_INSPECTOR
        [ShowIf("updateMode", DirectorUpdateMode.Manual)]
        #endif
        [Tooltip("The animation will only update once every N frames. Requires Manual Update Mode.")]
        [SerializeField, Min(1)]
        int updateRateInFrames = 5;
        
        // Non-Serialized Fields
        [NonSerialized] PlayableGraph playableGraph;
        [NonSerialized] AnimationPlayableOutput playableOutput;
        [NonSerialized] AnimationClipPlayable currentClipPlayable;
        [NonSerialized] int frameCounter = 0;

        /// <summary>
        /// Gets the PlayableGraph managed by this controller.
        /// </summary>
        public PlayableGraph Graph => playableGraph;

        void Awake()
        {
            if(animator == null) TryGetComponent(out animator).ThrowIfFalse();
            
            playableGraph = PlayableGraph.Create(gameObject.name + " AnimationGraph");
            
            // Set the update mode based on the Inspector setting.
            playableGraph.SetTimeUpdateMode(updateMode);
            
            playableOutput = AnimationPlayableOutput.Create(playableGraph, "AnimationOutput", animator);
        }

        void OnEnable()
        {
            if (playableGraph.IsValid())
                playableGraph.Play();
        }

        void Update()
        {
            // If the mode is not Manual or the graph is invalid, Unity handles it or we do nothing.
            if (updateMode != DirectorUpdateMode.Manual || !playableGraph.IsValid() || !playableGraph.IsPlaying())
            {
                return;
            }

            // If we are in Manual mode, we always apply the stop-motion logic.
            frameCounter++;
            if (frameCounter % updateRateInFrames == 0)
            {
                // Pass the accumulated delta time to keep the animation at the correct speed.
                float accumulatedDeltaTime = Time.deltaTime * updateRateInFrames;
                playableGraph.Evaluate(accumulatedDeltaTime);
            }
        }

        void OnDisable()
        {
            if (playableGraph.IsValid())
                playableGraph.Stop();
        }

        void OnDestroy()
        {
            if (playableGraph.IsValid())
                playableGraph.Destroy();
        }
        
        /// <summary>
        /// Plays a new AnimationClip, replacing any currently playing clip.
        /// </summary>
        /// <param name="clip">The AnimationClip to play.</param>
        public void Play(AnimationClip clip)
        {
            if (!playableGraph.IsValid() || clip == null) return;

            // If a clip is already playing, destroy its playable to clean up.
            if (currentClipPlayable.IsValid())
            {
                currentClipPlayable.Destroy();
            }

            // Create a new playable from the provided AnimationClip.
            currentClipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
            
            // Connect our new clip to the graph's output, making it play.
            playableOutput.SetSourcePlayable(currentClipPlayable);
        }
    }
}