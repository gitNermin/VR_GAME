using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class RemoteControl : MonoBehaviour
{
    public enum Hand
    {
        Left,
        Right
    }
    public static event Action<Hand, bool> OnEnableCarControls; 

    [SerializeField] private Hand _hand;
    [SerializeField] private GameObject _controllerModel;
    
    private XRRayInteractor _interactor;

    private void Start()
    {
        _interactor = GetComponentInChildren<XRRayInteractor>();
        
        _interactor.selectEntered.AddListener(OnSelectEnter);
        _interactor.selectExited.AddListener(OnSelectExit);
    }
    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if(!args.interactableObject.transform.CompareTag("RemoteControl")) return;
        
        OnEnableCarControls?.Invoke(_hand, true);
        _controllerModel.SetActive(false);
        _interactor.allowAnchorControl = false;
    }
    
    private void OnSelectExit(SelectExitEventArgs args)
    {
        if(!args.interactableObject.transform.CompareTag("RemoteControl")) return;
        
        OnEnableCarControls?.Invoke(_hand, false);
        _controllerModel.SetActive(true);
        _interactor.allowAnchorControl = true;
    }
}
