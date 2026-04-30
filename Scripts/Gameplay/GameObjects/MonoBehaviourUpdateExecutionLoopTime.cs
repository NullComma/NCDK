using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Base class to execute logic in a specific update loop.
    /// </summary>
    public abstract class MonoBehaviourUpdateExecutionLoopTime : MonoBehaviour
    {

        [SerializeField] MonoBehaviourExecutionLoop _executionTime;

        void Update()
        {
            if (_executionTime == MonoBehaviourExecutionLoop.Update)
                Execute(Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (_executionTime == MonoBehaviourExecutionLoop.FixedUpdate)
                Execute(Time.fixedDeltaTime);
        }

        void LateUpdate()
        {
            if (_executionTime == MonoBehaviourExecutionLoop.LateUpdate)
                Execute(Time.deltaTime);
        }

        protected abstract void Execute(float deltaTime);
    }
}