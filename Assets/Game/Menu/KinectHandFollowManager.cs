using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectHandFollowManager : MonoBehaviour
{
	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;
	public GameObject OverlayObject;
	public float smoothFactor = 5f;
	private float distanceToCamera = 10f;

	void Start()
	{
		if (OverlayObject)
		{
			distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
		}
	}

	void Update()
	{
		KinectManager manager = KinectManager.Instance;

		if (manager && manager.IsInitialized())
		{
			int iJointIndex = (int)TrackedJoint;
			if (manager.IsUserDetected())
			{
				uint userId = manager.GetPlayer1ID();

				if (manager.IsJointTracked(userId, iJointIndex))
				{
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);

					if (posJoint != Vector3.zero)
					{
						// 3d position to depth
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);

						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);

						float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
						float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;

						//						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
						//						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
						//Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));

						if (OverlayObject)
						{
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						}
					}
				}
			}
		}
	}
}
