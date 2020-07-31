using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamejam;

namespace gamejam {
    public class Card : MonoBehaviour
    {
        [SerializeField] public bool visible = false;
        [SerializeField] public float flipSpeed = 3f;
        [SerializeField] public float moveSpeed = 2f;
        [SerializeField] public MeshRenderer faceRenderer;
        [SerializeField] public Light dangerBacklight;
        Vector3 _expectedPosition;
        Vector3 _lastPosition;

        bool _dangerLightOn = false;
        public CardFace CurrentFace;
        Vector3 _hiddenRotation = new Vector3(0f, 0f, 0f);
        Vector3 _visibleRotation = new Vector3(0f, 180f, 0f);
        private PuzzleManager puzMgr;
        private GameTop gametop;

        void Awake() {
            _lastPosition = transform.position;
            _expectedPosition = transform.position;
        }

        public void ChangeVisibility(bool _newVisibility, bool muted = false) {
            if (visible != _newVisibility) {
                if (!muted) {
                    gametop.PlaySound("Flip");
                }
                visible = _newVisibility;
            }
        }

        void Start() {
            puzMgr = Object.FindObjectOfType<PuzzleManager>();
            gametop = Object.FindObjectOfType<GameTop>();
        }

        public void SetDestination(Vector3 position) {
            _lastPosition = _expectedPosition;
            _expectedPosition = position;
        }

        // don't use this unless scene is about to trash
        public void DANGER_SetVisibleRotation(Vector3 rotation) {
            _visibleRotation = rotation;
        }
        
        public void ReturnToPreviousDestination() {
            SetDestination(_lastPosition);
        }

        public void AssignFace(CardFace face, bool disabled = false) {
            CurrentFace = face;
            string resourceName = "CardFronts/" + CurrentFace.ToString();
            if (disabled) resourceName += "-GRAY";
            Material m = (Material) Resources.Load(resourceName, typeof(Material));
            faceRenderer.material = m;
        }

        public void ToggleDangerBacklight(bool visible) {
            _dangerLightOn = visible;
        }

        void OnMouseDown(){
            if (visible) return;          
            if (!puzMgr.ReadyToChoose()) return;

            ChangeVisibility(true);
            puzMgr.CardRevealed(this);
        }

        void Update()
        {
            Move();
            FadeLights();
        }

        void FadeLights() {
            float destination = _dangerLightOn ? 60f : 0f;
            dangerBacklight.spotAngle = Mathf.Lerp(dangerBacklight.spotAngle, destination, 4f* Time.deltaTime);
        }

        void Move() {
            Vector3 desiredRotation = visible ? _visibleRotation : _hiddenRotation;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(desiredRotation.x, desiredRotation.y, desiredRotation.z),
                Time.deltaTime * flipSpeed
            );
            transform.position = Vector3.Lerp(transform.position, _expectedPosition, moveSpeed * Time.deltaTime);
        }
    }
}
