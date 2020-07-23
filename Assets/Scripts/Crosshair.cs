using UnityEngine;
using UnityEngine.UI;

namespace Ui {
    public class Crosshair : MonoBehaviour {
        public Transform playerCamera;
        public Image crosshairImage;
        public Color notFoundColor;
        public Color foundColor;
        public float colorSpeed;

        private void Update() {
            crosshairImage.color = Color.Lerp(crosshairImage.color,
                Physics.Raycast(playerCamera.position, playerCamera.forward) ? foundColor : notFoundColor,
                colorSpeed * Time.deltaTime);
        }
    }
}