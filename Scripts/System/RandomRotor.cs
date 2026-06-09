using UnityEngine;
using NCDK.Refs;

namespace NCDK
{
    public class RandomRotor : ValidatedMonoBehaviour
    {
        public bool rotateX;
        public bool rotateY = true;
        public float interval = 0.5f;

        [System.NonSerialized] float _timer;
        [System.NonSerialized] int _lastX = -1;
        [System.NonSerialized] int _lastY = -1;

        void Start() => PickRandom();

        void LateUpdate()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                PickRandom();
                _timer = interval;
            }
        }

        void PickRandom()
        {
            Vector3 euler = transform.localEulerAngles;

            if (rotateX)
            {
                int next;
                do { next = Random.Range(0, 4); } while (next == _lastX);
                euler.x = next * 90f;
                _lastX = next;
            }

            if (rotateY)
            {
                int next;
                do { next = Random.Range(0, 4); } while (next == _lastY);
                euler.y = next * 90f;
                _lastY = next;
            }

            transform.localEulerAngles = euler;
        }
    }
}
