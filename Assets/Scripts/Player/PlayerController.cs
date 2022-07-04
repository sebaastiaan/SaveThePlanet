using System;
using System.Collections;
using UnityEngine;

public enum DireccionInput
{
    Null,
    Arriba,
    Izquierda, 
    Derecha,
    Abajo
}

public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float valorSalto = 15f;
    [SerializeField] private float gravedad = 20f;

    [Header("Carril")] 
    [SerializeField] private float posicionCarrilIzquierdo = -3.1f;
    [SerializeField] private float posicionCarrilDerecho = 3.1f;

    public bool EstaSaltando { get; private set; }
    public bool EstaDeslizando { get; private set; }
    
    private DireccionInput direccionInput;
    private Coroutine coroutineDeslizar;
    private CharacterController characterController;
    private PlayerAnimaciones playerAnimaciones;
    private float posicionVertical;
    private int carrilActual;
    private Vector3 direccionDeseada;

    private float controllerRadio;
    private float controllerAltura;
    private float controllerPosicionY;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimaciones = GetComponent<PlayerAnimaciones>();
    }

    private void Start()
    {
        controllerRadio = characterController.radius;
        controllerAltura = characterController.height;
        controllerPosicionY = characterController.center.y;
    }

    private void Update()
    {
        if (GameManager.Instancia.EstadoActual == EstadosDelJuego.Inicio ||
            GameManager.Instancia.EstadoActual == EstadosDelJuego.GameOver)
        {
            return;
        }
        
        DetectarInput();
        ControlarCarriles();
        CalcularMovimientoVertical();
        MoverPersonaje();
    }

    private void MoverPersonaje()
    {
        Vector3 nuevaPos = new Vector3(direccionDeseada.x, posicionVertical, velocidadMovimiento);
        characterController.Move(nuevaPos * Time.deltaTime);
    }

    private void CalcularMovimientoVertical()
    {
        if (characterController.isGrounded)
        {
            EstaSaltando = false;
            posicionVertical = 0f;

            if (EstaDeslizando == false && EstaSaltando == false)
            {
                playerAnimaciones.MostrarAnimacionCorrer();
            }

            if (direccionInput == DireccionInput.Arriba)
            {
                SoundManager.Instancia.ReproducirSonidoFX(SoundManager.Instancia.saltoClip);
                posicionVertical = valorSalto;
                EstaSaltando = true;
                playerAnimaciones.MostrarAnimacionSaltar();
                if (coroutineDeslizar != null)
                {
                    StopCoroutine(coroutineDeslizar);
                    EstaDeslizando = false;
                    ModificarColliderDeslizar(false);
                }
            }

            if (direccionInput == DireccionInput.Abajo)
            {
                if (EstaDeslizando)
                {
                    return;
                }

                if (coroutineDeslizar != null)
                {
                    StopCoroutine(coroutineDeslizar);
                }
                
                DeslizarPersonaje();
            }
        }
        else
        {
            if (direccionInput == DireccionInput.Abajo)
            {
                posicionVertical -= valorSalto;
                DeslizarPersonaje();
            }
        }

        posicionVertical -= gravedad * Time.deltaTime;
    }
    
    private void ControlarCarriles()
    {
        switch (carrilActual)
        {
            case -1:
                LogicaCarrilIzquierdo();
                break;
            case 0:
                LogicaCarrilCentral();
                break;
            case 1:
                LogicaCarrilDerecho();
                break;
        }
    }

    private void LogicaCarrilCentral()
    {
        if (transform.position.x > 0.1f)
        {
            MoverHorizontal(0f, Vector3.left);
        }
        else if (transform.position.x < -0.1f)
        {
            MoverHorizontal(0f, Vector3.right);
        }
        else
        {
            direccionDeseada = Vector3.zero;
        }
    }
    
    private void LogicaCarrilIzquierdo()
    {
        MoverHorizontal(posicionCarrilIzquierdo, Vector3.left);
    }

    private void LogicaCarrilDerecho()
    {
        MoverHorizontal(posicionCarrilDerecho, Vector3.right);
    }
    
    private void MoverHorizontal(float posicionX, Vector3 dirMovimiento)
    {
        float posicionHorizontal = Mathf.Abs(transform.position.x - posicionX);
        if (posicionHorizontal > 0.1f)
        {
            direccionDeseada = Vector3.Lerp(direccionDeseada, dirMovimiento * 20f, Time.deltaTime * 500f);
        }
        else
        {
            direccionDeseada = Vector3.zero;
            transform.position = new Vector3(posicionX, transform.position.y, transform.position.z);
        }
    }

    private void DeslizarPersonaje()
    {
        coroutineDeslizar = StartCoroutine(CODeslizarPersonaje());
    }
    
    private IEnumerator CODeslizarPersonaje()
    {
        SoundManager.Instancia.ReproducirSonidoFX(SoundManager.Instancia.deslizarClip);
        EstaDeslizando = true;
        playerAnimaciones.MostrarAnimacionDeslizar();
        ModificarColliderDeslizar(true);
        yield return new WaitForSeconds(2f);
        EstaDeslizando = false;
        ModificarColliderDeslizar(false);
    }
    
    private void ModificarColliderDeslizar(bool modificar)
    {
        if (modificar)
        {
            characterController.radius = 0.3f;
            characterController.height = 0.6f;
            characterController.center = new Vector3(0f, 0.35f, 0f);
        }
        else
        {
            characterController.radius = controllerRadio;
            characterController.height = controllerAltura;
            characterController.center = new Vector3(0f, controllerPosicionY, 0f);
        }
    }
    
    private void DetectarInput()
    {
        direccionInput = DireccionInput.Null;
        if (Input.GetKeyDown(KeyCode.A))
        {
            direccionInput = DireccionInput.Izquierda;
            carrilActual--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direccionInput = DireccionInput.Derecha;
            carrilActual++;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direccionInput = DireccionInput.Abajo;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            direccionInput = DireccionInput.Arriba;
        }

        carrilActual = Mathf.Clamp(carrilActual, -1, 1);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstaculo"))
        {
            if (GameManager.Instancia.EstadoActual == EstadosDelJuego.GameOver)
            {
                return;
            }
            
            SoundManager.Instancia.ReproducirSonidoFX(SoundManager.Instancia.colisionClip);
            playerAnimaciones.MostrarAnimacionColision();
            GameManager.Instancia.CambiarEstado(EstadosDelJuego.GameOver);
        }
    }
}
