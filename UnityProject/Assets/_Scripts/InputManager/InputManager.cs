using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[DefaultExecutionOrder(-5)]
public class InputManager : MonoBehaviour
{
    public Inputs playerInputs;
    public static InputManager Instance;

    [SerializeField] public UnityEvent pauseEvent;
    [SerializeField] public UnityEvent interactEvent;
    [SerializeField] public UnityEvent equipableEvent;
    [SerializeField] public UnityEvent anyKeyEvent;
    [SerializeField] public UnityEvent AlexRotateEvent;
    [SerializeField] public UnityEvent<Vector2> movementEvent;

    [SerializeField] public UnityEvent<string> ChangeUIto;

    [SerializeField] public string LastInputName;
    //Modificacion de la clase event para poder pasar en las llamadas vector2
    [System.Serializable]
    public class MyVector2Event : UnityEvent<Vector2>
    {
    }


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        playerInputs = new Inputs();

        pauseEvent = new UnityEvent();
        interactEvent = new UnityEvent();
        equipableEvent = new UnityEvent();
        anyKeyEvent = new UnityEvent();
        AlexRotateEvent = new UnityEvent();
        movementEvent = new UnityEvent<Vector2>();

        ChangeUIto = new UnityEvent<string>();


        playerInputs.ActionMap1.Enable();
        playerInputs.ActionMap1.Pausa.performed += pauseEvent_performed;
        playerInputs.ActionMap1.Interact.performed += Interact_performed;
        playerInputs.ActionMap1.UsarEquipable.performed += UsarEquipable_performed;
        playerInputs.ActionMap1.AnyKey.performed += AnyKey_performed;
        playerInputs.ActionMap1.RotarPiezaAlex.performed += RotarPiezaAlex_performed;
    }

    void Start()
    {
    }
    private void RotarPiezaAlex_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        AlexRotateEvent.Invoke();
    }
    private void AnyKey_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       
        if (obj.control.device.name.Contains("Gamepad") && LastInputName != "Gamepad")
        {
            LastInputName = "Gamepad";
            ChangeUIto.Invoke("Gamepad");
        }
        if (obj.control.device.name.Contains("Keyboard") && LastInputName != "Keyboard")
        {
            LastInputName = "Keyboard";
            ChangeUIto.Invoke("Keyboard");
        }     
        anyKeyEvent.Invoke();
    }

    private void UsarEquipable_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        equipableEvent.Invoke();
    }

    private void Update()
    {
        DetectarUltimoInputDevice();
        //En el caso del input Movement, no podemos usar un performed, por que al mantener la tecla no se actualizaria
        //la llamada, es mejor llamarlo cada frame y comprobar si a habido cambios en el vector
        if (MySceneManager.Instance.isLoading) return;

        if(Input.GetKeyDown(KeyCode.F8))
        {
            {
                HubManager.CustomInputState = GameManager.Instance.state - 1;
                MySceneManager.Instance.NextScene(100, 0, 0, 0);
            }
        }
        else if(Input.GetKeyDown(KeyCode.F9))
        {
            {
                HubManager.CustomInputState = GameManager.Instance.state + 1;
                MySceneManager.Instance.NextScene(100, 0, 0, 0);
            }
        }
        
    }

    private void FixedUpdate()
    {
        movement();
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        interactEvent.Invoke();
    }

    private void pauseEvent_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (pauseEvent == null)
            return;

        pauseEvent.Invoke();
    }

    void movement()
    {
        Vector2 vec = playerInputs.ActionMap1.Movement.ReadValue<Vector2>();
        var Matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));
        var inputChueca = Matrix.MultiplyPoint3x4(new Vector3(vec.x,0f,vec.y));
       
        vec = new Vector2(inputChueca.x, inputChueca.z);
        movementEvent.Invoke(vec);



    }

    private void DetectarUltimoInputDevice()
    {
        
    }

   


}
