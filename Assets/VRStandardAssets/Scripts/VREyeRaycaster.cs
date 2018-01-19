using System;
using UnityEngine;
using System.Collections.Generic;

namespace VRStandardAssets.Utils
{
    // In order to interact with objects in the scene
    // this class casts a ray into the scene and if it finds
    // a VRInteractiveItem it exposes it for other classes to use.
    // This script should be generally be placed on the camera.
    public class VREyeRaycaster : MonoBehaviour
    {
        public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.


        [SerializeField] private Transform m_Camera;
        [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
        //[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
        [SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
        [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
        [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
        [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
        [SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

        
        private List<VRInteractiveItem> m_CurrentInteractibles;                //The current interactive item
        private List<VRInteractiveItem> m_LastInteractibles;                   //The last interactive item

		private void Start()
		{
			m_CurrentInteractibles = new List<VRInteractiveItem>();
			m_LastInteractibles = new List<VRInteractiveItem>();
		}
		// Utility for other classes to get the current interactive item
		public List<VRInteractiveItem> CurrentInteractibles
        {
            get { return m_CurrentInteractibles; }
        }

        
        private void OnEnable()
        {
            m_VrInput.OnClick += HandleClick;
            m_VrInput.OnDoubleClick += HandleDoubleClick;
            m_VrInput.OnUp += HandleUp;
            m_VrInput.OnDown += HandleDown;
        }


        private void OnDisable ()
        {
            m_VrInput.OnClick -= HandleClick;
            m_VrInput.OnDoubleClick -= HandleDoubleClick;
            m_VrInput.OnUp -= HandleUp;
            m_VrInput.OnDown -= HandleDown;
        }


        private void Update()
        {
            EyeRaycast();
        }

      
        private void EyeRaycast()
        {
            // Show the debug ray if required
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
            }

            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);

			// Do the raycast forwards to see if we hit an interactive item
			RaycastHit[] hits = Physics.RaycastAll(ray, m_RayLength, ~m_ExclusionLayers);

			m_CurrentInteractibles.Clear();

			if (hits.Length > 0)
            {
				for (int i = 0; i < hits.Length; i++)
				{
					VRInteractiveItem interactible = hits[i].collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
					m_CurrentInteractibles.Add(interactible);
					interactible.Over();

					// If we hit an interactive item and it's not the same as the last interactive item, then call Over
					//if (m_LastInteractibles.Count > 0)
					//{
					//	bool found = false;
					//	for(int j = 0; j < m_LastInteractibles.Count; j++)
					//	{
					//		if (interactible && interactible != m_LastInteractibles[j])
					//		{
					//			found = true;
					//		}
					//	}
					//	if(!found)
					//	{
					//		Debug.Log("Over Called!");
					//	}
					//}

					// Something was hit, set at the hit position.
					//if (m_Reticle)
					//    m_Reticle.SetPosition(hit);

					if (OnRaycasthit != null)
						OnRaycasthit(hits[i]);
				}

				for (int i = 0; i < m_LastInteractibles.Count; i++)
				{
					bool found = false;
					for(int j = 0; j < m_CurrentInteractibles.Count; j++)
					{
						VRInteractiveItem interactible = m_CurrentInteractibles[j];
						if (m_LastInteractibles[i] == interactible)
						{
							found = true;
						}
					}
					if (!found)
					{
						m_LastInteractibles[i].Out();
					}
				}
			}
            else
            {
				// Nothing was hit, deactive the last interactive item.
				for(int i = 0; i < m_LastInteractibles.Count; i++)
				{
					m_LastInteractibles[i].Out();
				}
				m_LastInteractibles.Clear();

                // Position the reticle at default distance.
                //if (m_Reticle)
                //    m_Reticle.SetPosition();
            }
			m_LastInteractibles = new List<VRInteractiveItem>(m_CurrentInteractibles);
		}


		private void DeactiveLastInteractibles(VRInteractiveItem item)
        {
            if (item == null)
                return;

        }


        private void HandleUp()
        {
            if (m_CurrentInteractibles != null)
			{
				for (int i = 0; i < m_CurrentInteractibles.Count; i++)
				{
					m_CurrentInteractibles[i].Up();
				}

			}
		}


        private void HandleDown()
        {
            if (m_CurrentInteractibles != null)
			{
				for (int i = 0; i < m_CurrentInteractibles.Count; i++)
				{
					m_CurrentInteractibles[i].Down();
				}

			}
		}


        private void HandleClick()
        {
            if (m_CurrentInteractibles != null)
			{
				for (int i = 0; i < m_CurrentInteractibles.Count; i++)
				{
					m_CurrentInteractibles[i].Click();
				}

			}
		}

        private void HandleDoubleClick()
        {
            if (m_CurrentInteractibles != null)
			{
				for (int i = 0; i < m_CurrentInteractibles.Count; i++)
				{
					m_CurrentInteractibles[i].DoubleClick();
				}

			}

		}
    }
}