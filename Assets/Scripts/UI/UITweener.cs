namespace AshkolTools.UI
{
    using UnityEngine;
    using UnityEngine.Events;

    public enum UIAnimationType
    {
        Move,
        MoveAbsolute,
        Scale,
        ScaleX,
        ScaleY,
        Fade
    }


    public class UITweener : MonoBehaviour
    {
        public GameObject objectToAnimate;

        public UIAnimationType animationType;
        public LeanTweenType easeType;
        public float duration;
        public float delay;

        public bool loop;
        public bool pingpong;

        public bool startPositionOffset;
        public Vector3 from;
        public Vector3 to;

        private LTDescr _tweenObject;

        public bool showOnEnable;
        public bool workOnDisable;
        public bool forceRefresh;

        public void OnEnable()
        {
            if (showOnEnable)
            {
                Show();
            }
        }

        public void Show()
        {
            HandleTween();
        }

        //IEnumerator HandleTweenDelayed()
        //{
        //    float counter = 0;
        //    while (counter < delay)
        //    {
        //        counter += Time.deltaTime;
        //        yield return null;
        //    }
        //    HandleTween();
        //}

        //IEnumerator Activate()
        //{
        //    float counter = 0;
        //    while (counter < delay)
        //    {
        //        counter += Time.deltaTime;
        //        yield return null;
        //    }
        //    objectToAnimate.SetActive(true);
        //}

        public void HandleTween()
        {
            if (objectToAnimate == null)
            {
                objectToAnimate = gameObject;
            }

            switch (animationType)
            {
                case UIAnimationType.Fade:
                    Fade();
                    break;
                case UIAnimationType.Move:
                    Move();
                    break;
                case UIAnimationType.MoveAbsolute:
                    MoveAbsolute();
                    break;
                case UIAnimationType.Scale:
                    Scale();
                    break;
                case UIAnimationType.ScaleX:
                    ScaleX();
                    break;
                case UIAnimationType.ScaleY:
                    ScaleY();
                    break;
                default:
                    break;
            }

            _tweenObject.setDelay(delay);
            _tweenObject.setEase(easeType);

            if (loop)
            {
                _tweenObject.loopCount = int.MaxValue;
            }
            if (pingpong)
            {
                _tweenObject.setLoopPingPong();
            }
        }

        public void HandleTween(float delay)
        {
            if (objectToAnimate == null)
            {
                objectToAnimate = gameObject;
            }

            switch (animationType)
            {
                case UIAnimationType.Fade:
                    Fade();
                    break;
                case UIAnimationType.Move:
                    Move();
                    break;
                case UIAnimationType.MoveAbsolute:
                    MoveAbsolute();
                    break;
                case UIAnimationType.Scale:
                    Scale();
                    break;
                case UIAnimationType.ScaleX:
                    ScaleX();
                    break;
                case UIAnimationType.ScaleY:
                    ScaleY();
                    break;
                default:
                    break;
            }

            _tweenObject.setDelay(delay);
            _tweenObject.setEase(easeType);
            if (forceRefresh)
                _tweenObject.setOnComplete(Refresh);

            if (loop)
            {
                _tweenObject.loopCount = int.MaxValue;
            }
            if (pingpong)
            {
                _tweenObject.setLoopPingPong();
            }
        }

        private void Refresh()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void Fade()
        {
            if (gameObject.GetComponent<CanvasGroup>() == null)
            {
                gameObject.AddComponent<CanvasGroup>();
            }

            if (startPositionOffset)
            {
                objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
            }
            _tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
        }

        public void MoveAbsolute()
        {
            objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
            _tweenObject = LeanTween.move(objectToAnimate, to, duration);
        }

        public void Move()
        {
            objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
            _tweenObject = LeanTween.moveLocal(objectToAnimate, to, duration);
        }

        public void Scale()
        {
            if (startPositionOffset)
            {
                objectToAnimate.GetComponent<RectTransform>().localScale = from;
            }
            _tweenObject = LeanTween.scale(objectToAnimate, to, duration);
        }

        public void ScaleX()
        {
            if (startPositionOffset)
            {
                objectToAnimate.GetComponent<RectTransform>().localScale = from;
            }
            _tweenObject = LeanTween.scaleX(objectToAnimate, to.x, duration);
        }

        public void ScaleY()
        {
            if (startPositionOffset)
            {
                objectToAnimate.GetComponent<RectTransform>().localScale = from;
            }
            _tweenObject = LeanTween.scaleY(objectToAnimate, to.y, duration);
        }

        public void SwapDirection()
        {
            var temp = from;
            from = to;
            to = temp;
        }

        public void Disable()
        {
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                //gameObject.SetActive(false);
            });
        }

        public void DisableImmediate()
        {
            float activeDelay = delay;
            delay = 0f;
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                gameObject.SetActive(false);
            });
            delay = activeDelay;
        }

        public void Disable(UnityAction onCompleteAction)
        {
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                gameObject.SetActive(false);
                onCompleteAction.Invoke();
            });
        }
    }

}
