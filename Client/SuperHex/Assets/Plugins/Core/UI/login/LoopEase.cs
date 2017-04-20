using UnityEngine;

namespace Sa1.LoopEase {

    public abstract class LoopEase {
        public abstract float update (float deltaTime);
        public abstract void clear ();

        public static SinEase sin () {
            return new SinEase();
        }
    }

    /// <summary>
    ///     ease class, sin and cos is the same with different timer initial value
    ///     ins to find some neccesary of this exsitence
    /// </summary>
    public class SinEase : LoopEase {
        // ease type
        public enum EaseType {
            Increase, Decrease, MiddleIncrease, MiddleDecrease, Rondom
        }

        private float timer = -0.5f * Mathf.PI;// -cos todo
        public float scale { get; set; }

        public float bottom { get; private set; }
        public float top { get; private set; }
        private float a;

        public SinEase init (float min, float max) {
            scale = 1;
            bottom = min;
            top = max;
            a = top - bottom;
            return this;
        }

        public SinEase init (EaseType easeType) {
            switch (easeType) {
                case EaseType.Increase:
                    timer = -0.5f*Mathf.PI;
                    break;
                case EaseType.MiddleIncrease:
                    timer = 0;
                    break;
                case EaseType.Decrease:
                    timer = 0.5f*Mathf.PI;
                    break;
                case EaseType.MiddleDecrease:
                    timer = Mathf.PI;
                    break;
                case EaseType.Rondom:
                    timer = Random.value*2*Mathf.PI;
                    break;
            }
            return this;
        }
        
        public SinEase () {
            init(0, 1);
        }

        // to delete
        public SinEase (float max) {
            init(0, max);
        }

        // to delete
        public SinEase (float min, float max) {
            init(min, max);
        }

        public override float update (float deltaTime) {
            timer += deltaTime/scale;
            return bottom + 0.5f * a * (1 + Mathf.Sin(timer));
        }

        public override void clear () {
            timer = -0.5f*Mathf.PI;
        }
    }

}