using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Allows us to use the Input System to get values for the thumbstick
using UnityEngine.InputSystem;
//Allows us to use the Interaction Toolkit to enable and disable our rays
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRRayInteractor))]
public class TeleportationController: MonoBehaviour
{
    //Used to determine current active state of the Teleportation System
    private static bool _teleportIsActive = false;

    //Creates an enum that will determine if we're using the right or left controller
    public enum ControllerType
    {
        RightHand,
        LeftHand
    }

    //Stores the target controller from the editor
    public ControllerType targetController;

    //References our Input Actions that we are using
    public InputActionAsset inputAction;

    //References the rayInteractor to be enabled/disabled later
    private XRRayInteractor _rayInteractor;

    //References the Teleportation Provider so we can use it to teleport the Player in the event of a succesful teleport call
    private TeleportationProvider _teleportationProvider;


    //Will reference the Thumbstick Input Action when the scene starts up
    private InputAction _thumbstickInputAction;

    //Stores Action for Teleport Mode Activate
    private InputAction _teleportActivate;

    //Stores Action for Teleport Mode Cancel
    private InputAction _teleportCancel;

    void Start()
    {
        //We don't want the rayInteractor to on unless we're using the forward press on the thumbstick so we deactivate it here
        _rayInteractor = GetComponent<XRRayInteractor>();
        _rayInteractor.enabled = false;

        //This will find the Action Map of our target controller for Teleport Mode Activate.
        //It will enable it and then subscribe itself to our OnTeleportActivate function
        Debug.Log("XRI " + targetController.ToString());
        _teleportActivate = inputAction.FindActionMap("XRI " + targetController.ToString() + " Locomotion").FindAction("Teleport Mode Activate");
        _teleportActivate.Enable();
        _teleportActivate.performed += OnTeleportActivate;

        //This will find the Action Map of our target controller for Teleport Mode Cancel.
        //It will enable it and then subscribe itself to our OnTeleportCancel function
        _teleportCancel = inputAction.FindActionMap("XRI " + targetController.ToString() + " Locomotion").FindAction("Teleport Mode Cancel");
        _teleportCancel.Enable();
        _teleportCancel.performed += OnTeleportCancel;


        //We grab this reference so we can use it to tell if the thumbstick is still being pressed
        _thumbstickInputAction = inputAction.FindActionMap("XRI " + targetController.ToString() + " Locomotion").FindAction("Move");
        _thumbstickInputAction.Enable();

        _teleportationProvider = FindObjectOfType<TeleportationProvider>();
    }

    private void OnDestroy()
    {
        _teleportActivate.performed -= OnTeleportActivate;
        _teleportCancel.performed -= OnTeleportCancel;
    }

    //We use the Update function to check for when a teleportation event has occured. 
    //The checks needed to ensure a succesful teleport event are
    //-Teleporting is currently active
    //-The ray currently the active one
    //-The Thumbstick isn't being pressed
    //-The rayInteractor is hitting a valid target
    //If those pass, we make a teleportation request to the Teleport Provider
    void Update()
    {

        if (!_teleportIsActive)
        {
            return;
        }
        if (!_rayInteractor.enabled)
        {
            return;
        }
        if (_thumbstickInputAction.triggered)
        {
            return;
        }
        if (!_rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
        {
            _rayInteractor.enabled = false;
            _teleportIsActive = false;
            return;
        }
        if (raycastHit.collider.gameObject.tag != "Teleport")
        {

            _rayInteractor.enabled = false;
            _teleportIsActive = false;
            return;
        }

        TeleportRequest teleportRequest = new TeleportRequest()
        {
            destinationPosition = raycastHit.point,
        };

        _teleportationProvider.QueueTeleportRequest(teleportRequest);

        _rayInteractor.enabled = false;
        _teleportIsActive = false;
    }

    //This is called when our Teleport Mode Activated action map is triggered
    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        if (!_teleportIsActive)
        {
            _rayInteractor.enabled = true;
            _teleportIsActive = true;
        }

    }

    //This is called when our Teleport Mode Cancel action map is triggered
    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        if (_teleportIsActive && _rayInteractor.enabled == true)
        {
            _rayInteractor.enabled = false;
            _teleportIsActive = false;
        }

    }
}
