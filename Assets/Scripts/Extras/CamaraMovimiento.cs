using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraMovimiento : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float alturaDelPersonaje;
    [SerializeField] private float distanciaDelPersonaje;

    private Vector3 miCamaraPos;
    
    private void Start()
    {
        miCamaraPos = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 posPersonaje = GameManager.Instancia.PersonajeActivo.position;
        miCamaraPos.x = Mathf.Lerp(miCamaraPos.x, posPersonaje.x, Time.deltaTime * 10f);
        miCamaraPos.y = Mathf.Lerp(miCamaraPos.y, posPersonaje.y + alturaDelPersonaje, Time.deltaTime * 10f);
        miCamaraPos.z = Mathf.Lerp(miCamaraPos.z, posPersonaje.z + distanciaDelPersonaje, Time.deltaTime * 10f);
        transform.position = miCamaraPos;
    }
}
