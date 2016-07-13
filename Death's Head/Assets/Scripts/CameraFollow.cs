using System;
using UnityEngine;


namespace UnityStandardAssets._2D
{
    public class CameraFollow : MonoBehaviour
    {
        public float xRightMargin = 1f; // Distance in the x axis the player can move before the camera follows.

        public float yMargin = 1f; // Distance in the y axis the player can move before the camera follows.
        public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
        public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.

        public Transform m_Player; // Reference to the player's transform.

		private Vector2 maxXAndY;		// The maximum x and y coordinates the camera can have.
		private float minX;// The minimum x and y coordinates the camera can have.
		private float minY;

		public Transform worldTopRight;
		public Transform worldBottomLeft;


        private void Awake()
        {
            // Setting up the reference.
            m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

		private void Start ()
		{
			worldTopRight = GameObject.Find ("WorldTopRight").GetComponent<Transform> ();
			worldBottomLeft = GameObject.Find ("WorldBottomLeft").GetComponent<Transform> ();

			maxXAndY = worldTopRight.transform.position;
			minY = worldBottomLeft.transform.position.y;
			minX = worldBottomLeft.transform.position.x;
		}


        private bool CheckXMargin()
        {
            return Mathf.Abs(transform.position.x - m_Player.position.x) > xRightMargin;
            /*
            if(PlayerController.Instance.p_State == PlayerController.PlayerStates.DEATH)
            {
                return Mathf.Abs(transform.position.x - m_Player.position.x) > xRightMargin;
            }
            */

            // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
            //return (transform.position.x - m_Player.position.x) < xRightMargin;
        }


        private bool CheckYMargin()
        {
            // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
            return Mathf.Abs(transform.position.y - m_Player.position.y) > yMargin;
        }


        private void Update()
        {
            TrackPlayer();
        }


        private void TrackPlayer()
        {
            // By default the target x and y coordinates of the camera are it's current x and y coordinates.
            float targetX = transform.position.x;
            float targetY = transform.position.y;

			maxXAndY = worldTopRight.transform.position;
			minY = worldBottomLeft.transform.position.y;
                       

            // If the player has moved beyond the x margin...
            if (CheckXMargin())
            {
                // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
                targetX = Mathf.Lerp(transform.position.x, m_Player.position.x, xSmooth*Time.deltaTime);
            }

            // If the player has moved beyond the y margin...
            if (CheckYMargin())
            {
                // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
                targetY = Mathf.Lerp(transform.position.y, m_Player.position.y, ySmooth*Time.deltaTime);
            }

			// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
			targetX = Mathf.Clamp (targetX, minX, maxXAndY.x);

			targetY = Mathf.Clamp (targetY, minY, maxXAndY.y);

			// Set the camera's position to the target position with the same z component.
			transform.position = new Vector3 (targetX, transform.position.y, transform.position.z);


            // Set the camera's position to the target position with the same z component.
            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}
