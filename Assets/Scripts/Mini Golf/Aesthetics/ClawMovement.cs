using DG.Tweening;
using UnityEngine;

namespace gameracers.MiniGolf.Aesthetics
{
    public class ClawMovement : MonoBehaviour
    {
        [SerializeField] Transform claw1Hinge;
        [SerializeField] Transform claw2Hinge;
        [SerializeField] Transform claw3Hinge;

        [SerializeField] Vector3 claw1Open;
        [SerializeField] Vector3 claw2Open;
        [SerializeField] Vector3 claw3Open;

        [SerializeField] float openDur = 1f;

        Vector3 claw1Close;
        Vector3 claw2Close;
        Vector3 claw3Close;

        bool hasTransform;
        Transform heldTransform;

        [SerializeField] float moveTimer = 1.5f;
        [SerializeField] float dropDelay = .5f;
        float timer = Mathf.Infinity;
        

        void Start()
        {
            claw1Close = new Vector3(270, 90, -90);
            claw1Hinge.eulerAngles = claw1Close;
            claw2Close = claw2Hinge.eulerAngles;
            claw3Close = claw3Hinge.eulerAngles;
        }

        public void MoveToPos(Vector3 pos)
        {
            timer = Time.time;
            transform.DOMove(pos, moveTimer);
        }

        public void OpenClaw()
        {
            claw1Hinge.DOLocalRotate(claw1Open, openDur);
            claw2Hinge.DORotate(claw2Open, openDur);
            claw3Hinge.DORotate(claw3Open, openDur);
            if (hasTransform)
            {
                heldTransform = null;
                hasTransform = false;
            }
        }

        public void CloseClaw()
        {
            claw1Hinge.DORotate(claw1Close, openDur);
            claw2Hinge.DORotate(claw1Close, openDur);
            claw3Hinge.DORotate(claw1Close, openDur);
        }

        public void GrabTransform(Transform player)
        {
            hasTransform = true;
            heldTransform = player;
        }

        private void Update()
        {
            if (hasTransform == true)
            {
                heldTransform.transform.position = transform.position;
            }

            if (timer != Mathf.Infinity)
            {
                if (Time.time - timer > moveTimer + dropDelay)
                {
                    OpenClaw();
                    timer = Mathf.Infinity;
                    GolfEventListener.RoundStart();
                }
            }
        }
    }
}
